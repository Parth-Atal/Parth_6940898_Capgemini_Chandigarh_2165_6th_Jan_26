using Microsoft.EntityFrameworkCore;
using UserApi.Models;

namespace UserApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
    }
}