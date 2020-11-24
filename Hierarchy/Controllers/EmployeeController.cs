using Hierarchy.Dto;
using Hierarchy.Entities;
using Hierarchy.Extensions;
using Hierarchy.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hierarchy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public EmployeeController(EmployeeDbContext context)

        {
            _context = context;
        }

        /// <summary>
        /// Dohvati sve zaposlenike iz baze
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async  Task<IActionResult> Get()
        {
            return Ok(await _context.Employees
                .Select(i => new EmployeeDto
                {
                    Id = i.Id,
                    Hierarchy = i.HierarchyId.ToString(),
                    Level = i.HierarchyId.GetLevel(),
                    Name = i.Name
                })
                .ToListAsync()
            );
        }

        [HttpGet("level/{level}")]
        public async Task<IActionResult> GetAllEmployeesOnLevel(int level)
        {
            return Ok(await _context.Employees
                .Where(i => i.HierarchyId.GetLevel() == level)
                .Select(i => new EmployeeDto
                {
                    Id = i.Id,
                    Hierarchy = i.HierarchyId.ToString(),
                    Level = i.HierarchyId.GetLevel(),
                    Name = i.Name
                })
                .ToListAsync()
            );
        }

        [HttpGet("firstparent/{id}")]
        public async Task<IActionResult> GetParentOfInstance(int id)
        
        {
            var instance = await _context.Employees.FindAsync(id);

            if (instance is null)
                return NotFound();

            //Ako nema parenta vraca se null
            var parent = await _context.Employees
                .FirstOrDefaultAsync(i => i.HierarchyId == instance.HierarchyId.GetAncestor(1));

            if (parent is null)
                return Ok("Nema roditelja");

            return Ok(new EmployeeDto
            {
                Id = parent.Id,
                Hierarchy = parent.HierarchyId.ToString(),
                Level = parent.HierarchyId.GetLevel(),
                Name = parent.Name
            });
        }

        [HttpGet("ancestors/{id}")]
        public async Task<IActionResult> GetAllAncestorsOfInstance(int id)

        {
            var instance = await _context.Employees.FindAsync(id);

            if (instance is null)
                return NotFound();

            var ancestors = await _context.Employees
                .Where(e => instance.HierarchyId.IsDescendantOf(e.HierarchyId) && e.Id != id)
                .Select(i => new EmployeeDto
                {
                    Id = i.Id,
                    Hierarchy = i.HierarchyId.ToString(),
                    Level = i.HierarchyId.GetLevel(),
                    Name = i.Name
                })
                .ToListAsync();

            return Ok(ancestors);
        }


        [HttpGet("firstchildrens/{id}")]
        public async Task<IActionResult> GetFirstChildrensOfInstance(int id)

        {
            var instance = await _context.Employees.FindAsync(id);

            if (instance is null)
                return NotFound();

            var ancestors = await _context.Employees
                .Where(e => e.HierarchyId.GetAncestor(1) == instance.HierarchyId)
                .Select(i => new EmployeeDto
                {
                    Id = i.Id,
                    Hierarchy = i.HierarchyId.ToString(),
                    Level = i.HierarchyId.GetLevel(),
                    Name = i.Name
                })
                .ToListAsync();

            return Ok(ancestors);
        }

        [HttpGet("allchildrens/{id}")]
        public async Task<IActionResult> GetAllChildrensOfInstance(int id)

        {
            var instance = await _context.Employees.FindAsync(id);

            if (instance is null)
                return NotFound();

            var ancestors = await _context.Employees
                .Where(e => e.HierarchyId.IsDescendantOf(instance.HierarchyId) && id != e.Id)
                .Select(i => new EmployeeDto
                {
                    Id = i.Id,
                    Hierarchy = i.HierarchyId.ToString(),
                    Level = i.HierarchyId.GetLevel(),
                    Name = i.Name
                })
                .ToListAsync();

            return Ok(ancestors);
        }


        [HttpPost("add")]
        public async Task<IActionResult> AddNewInstance([FromBody] AddEmployeeDto newInstance)

        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (newInstance.ParentId.HasValue) //dodavanje postojecem nodu u stablu
            {
                //dohvati roditelja nove instance
                Employee parent = await _context.Employees.FindAsync(newInstance.ParentId.Value);

                if (parent is null) //roditelj ne postoji
                    return NotFound();

                //dohvati zadnje dodano dijete danog roditelja
                var lastAddedItemOfParent = await _context.Employees
                    .Where(x => x.HierarchyId.GetAncestor(1) == parent.HierarchyId)
                    .OrderByDescending(x => x.HierarchyId)
                    .FirstOrDefaultAsync();

                //kreiraj novu instancu koristenjem HierarchyId roditelja i zadnjeg djeteta (ako postoji)
                var newInstnace = new Employee
                {
                    Name = newInstance.Name,

                    //Treba poceti sa HierarchyId roditelja i treba biti veci od zadnjeg djeteta
                    //tipa ako roditelj pocinje sa 3, a zadnje dodano dijete je 3/1, onda ce novo dijete imati 3/2
                    HierarchyId = parent.HierarchyId.GetDescendant(lastAddedItemOfParent?.HierarchyId ?? null, null)
                };

                //dodaj u kolekciju i spremi u bazu
                _context.Employees.Add(newInstnace);

                await _context.SaveChangesAsync();

                //NewChild treba imati veci HierarchyId od LastAddedChild u slucaju da je LastAddedChild != null
                //Ako je LastAddedChild null znaci da je dodano prvo dijete za danog roditelja
                return Ok(new
                {
                    LastAddedChild = (lastAddedItemOfParent != null) ? new EmployeeDto
                    {
                        Id = lastAddedItemOfParent.Id,
                        Hierarchy = lastAddedItemOfParent.HierarchyId.ToString(),
                        Level = lastAddedItemOfParent.HierarchyId.GetLevel(),
                        Name = lastAddedItemOfParent.Name
                    } : null,
                    NewChild = new EmployeeDto
                    {
                        Id = newInstnace.Id,
                        Hierarchy = newInstnace.HierarchyId.ToString(),
                        Level = newInstnace.HierarchyId.GetLevel(),
                        Name = newInstnace.Name
                    }
                });
            }
            else
            {
                //dohvati zadnje dodano dijete na prvoj razini
                var lastItemInCurrentLevel = _context.Employees
                  .Where(x => x.HierarchyId.GetLevel() == 1)
                  .OrderByDescending(x => x.HierarchyId)
                  .FirstOrDefault();

                if (lastItemInCurrentLevel is null) //dodajemo prvi root element
                {
                    var newInstnace = new Employee
                    {
                        Name = newInstance.Name,
                        HierarchyId = HierarchyId.Parse("/1/")
                    };

                    _context.Employees.Add(newInstnace);

                    await _context.SaveChangesAsync();

                    return Ok(new EmployeeDto
                    {
                        Id = newInstnace.Id,
                        Hierarchy = newInstnace.HierarchyId.ToString(),
                        Level = newInstnace.HierarchyId.GetLevel(),
                        Name = newInstnace.Name
                    });
                }

                //DRUGI SOLUTION
                //Kao root moguce je koristiti primary key novog dodanog elementa (losa praksa)
                //Tipa ako je primary key 10 onda ce HierarchyId biti /10/ i brojevi nece ici redom


                //postoji vec root element i dodajemo novog
                //nije ni ovo dobro rjesenje. Ako dode do dva zahtjeva u isto vrijeme jedan ce proci a jedan nece

                var nextHierarchyId = lastItemInCurrentLevel.HierarchyId.GetNextRootId();

                var newInstnace2 = new Employee
                {
                    Name = newInstance.Name,
                    HierarchyId = HierarchyId.Parse($"/{nextHierarchyId}/")
                };

                _context.Employees.Add(newInstnace2);

                await _context.SaveChangesAsync();

                return Ok(new EmployeeDto
                {
                    Id = newInstnace2.Id,
                    Hierarchy = newInstnace2.HierarchyId.ToString(),
                    Level = newInstnace2.HierarchyId.GetLevel(),
                    Name = newInstnace2.Name
                });
            }
        }
    }
}






