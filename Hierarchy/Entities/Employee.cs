using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hierarchy.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public HierarchyId HierarchyId { get; set; }
        public HierarchyId OldHierarchyId { get; set; }
    }
}
