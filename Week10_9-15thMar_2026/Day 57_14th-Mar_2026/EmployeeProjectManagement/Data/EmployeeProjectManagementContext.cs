using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmployeeProjectManagement.Models;

namespace EmployeeProjectManagement.Data
{
    public class EmployeeProjectManagementContext : DbContext
    {
        public EmployeeProjectManagementContext (DbContextOptions<EmployeeProjectManagementContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeProjectManagement.Models.Department> Department { get; set; } = default!;
        public DbSet<EmployeeProjectManagement.Models.Employee> Employee { get; set; } = default!;
        public DbSet<EmployeeProjectManagement.Models.Project> Project { get; set; } = default!;
        public DbSet<EmployeeProjectManagement.Models.EmployeeProject> EmployeeProject { get; set; } = default!;
    }
}
