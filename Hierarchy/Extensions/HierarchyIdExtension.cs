using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hierarchy.Extensions
{
    public static class HierarchyIdExtension
    {
        public static int GetNextRootId(this HierarchyId element)
        {
            string stringId = element.ToString();

            //provjeri da li je element root
            //root primjer -> /3/
            if (stringId.Count(opt => opt == '/') != 2)
                throw new ArgumentException("Element nije root");

            string cleanId = stringId.Substring(1, stringId.Length - 2);

            if (int.TryParse(cleanId, out int finalId))
                return finalId + 1;

            throw new ArgumentException("Element nije root");
        }
    }
}
