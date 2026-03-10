using BooksApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            

            var builder = WebApplication.CreateBuilder(args);

            var cs1 = builder.Configuration.GetConnectionString("cs1");
            builder.Services.AddDbContext<BookdbContext>(options => options.UseSqlServer(cs1));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=BooksModels}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
