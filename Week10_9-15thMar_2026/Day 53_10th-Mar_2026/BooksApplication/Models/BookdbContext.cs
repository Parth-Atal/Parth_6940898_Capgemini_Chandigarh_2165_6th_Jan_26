using Microsoft.EntityFrameworkCore;

namespace BooksApplication.Models { 

    public class BookdbContext : DbContext
    {
        public BookdbContext(DbContextOptions<BookdbContext> options) : base(options)
        {
        }
        public DbSet<BooksModel> books { get; set; }
    }

}