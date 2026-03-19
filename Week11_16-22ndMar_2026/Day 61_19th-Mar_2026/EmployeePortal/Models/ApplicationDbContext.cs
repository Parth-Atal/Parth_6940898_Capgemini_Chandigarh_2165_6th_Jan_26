using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Models
{
    public class ApplicationDbContext
    {
        public DbSet<Employee> Employees { get; set; }
    }
}
