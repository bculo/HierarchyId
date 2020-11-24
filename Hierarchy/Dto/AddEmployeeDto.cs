using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hierarchy.Dto
{
    public class AddEmployeeDto
    {
        [Required]
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
