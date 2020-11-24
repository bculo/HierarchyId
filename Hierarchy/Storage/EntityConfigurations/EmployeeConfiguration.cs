using Hierarchy.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hierarchy.Storage.EntityConfigurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(opt => opt.Id);
            builder.Property(opt => opt.Id)
                .ValueGeneratedOnAdd();

            builder.Property(opt => opt.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(opt => opt.HierarchyId)
                .IsRequired();
            builder.HasIndex(opt => opt.HierarchyId)
                .IsUnique();

            builder.HasData(new Employee[]
            {
                new Employee { Id = 1, Name = "Lidia Brewer", HierarchyId = HierarchyId.Parse("/1/") },
                new Employee { Id = 2, Name = "Hannah Wicks", HierarchyId = HierarchyId.Parse("/2/") },
                new Employee { Id = 3, Name = "Sheridan Perkins", HierarchyId = HierarchyId.Parse("/1/1/") },
                new Employee { Id = 4, Name = "Zakaria Bailey", HierarchyId = HierarchyId.Parse("/1/2/") },
                new Employee { Id = 5, Name = "Albert Woodward", HierarchyId = HierarchyId.Parse("/1/2/1/") },
                new Employee { Id = 6, Name = "Arron Mcdaniel", HierarchyId = HierarchyId.Parse("/1/2/2/") }
            });

            builder.ToTable(nameof(Employee));
        }
    }
}
