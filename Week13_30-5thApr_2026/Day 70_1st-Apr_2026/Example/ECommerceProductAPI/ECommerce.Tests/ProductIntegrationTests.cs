using Xunit;
using ECommerce.API.Data;
using ECommerce.API.Models;
using ECommerce.API.Repositories;
using ECommerce.API.Services;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Tests
{
    public class ProductIntegrationTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        //  Each test gets a fresh InMemory DB
        public ProductIntegrationTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        //  Test 1: Add product saves to database
        [Fact]
        public async Task AddProduct_ShouldSaveToDatabase()
        {
            // Arrange
            using var context = new AppDbContext(_options);
            var repo = new ProductRepository(context);
            var service = new ProductService(repo);

            // Act
            await service.AddProduct(new Product { Name = "Tablet", Price = 20000 });

            // Assert
            var result = await context.Products.ToListAsync();
            Assert.Single(result);                        // must have 1 product
            Assert.Equal("Tablet", result[0].Name);       // name must match
        }

        // Test 2: GetAll returns all products from database
        [Fact]
        public async Task GetAllProducts_ShouldReturnAllFromDatabase()
        {
            // Arrange
            using var context = new AppDbContext(_options);
            context.Products.AddRange(
                new Product { Name = "Laptop", Price = 50000 },
                new Product { Name = "Phone",  Price = 30000 }
            );
            await context.SaveChangesAsync();

            var repo = new ProductRepository(context);
            var service = new ProductService(repo);

            // Act
            var result = await service.GetAllProducts();

            // Assert
            Assert.Equal(2, result.Count);                // must have 2 products
        }

        //  Test 3: GetProduct returns correct product from database
        [Fact]
        public async Task GetProduct_ShouldReturnCorrectProduct()
        {
            // Arrange
            using var context = new AppDbContext(_options);
            var added = new Product { Name = "Laptop", Price = 50000 };
            context.Products.Add(added);
            await context.SaveChangesAsync();

            var repo = new ProductRepository(context);
            var service = new ProductService(repo);

            // Act
            var result = await service.GetProduct(added.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Laptop", result!.Name);
        }

        // Test 4: Delete removes product from database
        [Fact]
        public async Task DeleteProduct_ShouldRemoveFromDatabase()
        {
            // Arrange
            using var context = new AppDbContext(_options);
            var product = new Product { Name = "Laptop", Price = 50000 };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var repo = new ProductRepository(context);
            var service = new ProductService(repo);

            // Act
            await service.DeleteProduct(product.Id);

            // Assert
            var result = await context.Products.ToListAsync();
            Assert.Empty(result);                         // must be empty
        }
    }
}