using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EfCoreCodeFirstApproachDemo01.Models
{
    public class EmployeeDBContext : DbContext
    {
        public EmployeeDBContext(DbContextOptions<EmployeeDBContext> options) : base(options)
        {
        }
        public DbSet<EmployeeModel> employees { get; set; }
    }
}
