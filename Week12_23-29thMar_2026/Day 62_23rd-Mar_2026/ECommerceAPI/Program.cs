using ECommerceAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Services
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer(); // ✅ REQUIRED
            builder.Services.AddSwaggerGen();           // ✅ Swagger

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // Middleware
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}