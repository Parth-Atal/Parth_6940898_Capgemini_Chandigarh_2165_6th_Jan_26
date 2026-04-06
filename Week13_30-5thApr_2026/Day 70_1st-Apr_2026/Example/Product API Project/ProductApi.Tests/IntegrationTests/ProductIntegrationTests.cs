using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Models;
using ProductApi.Repositories;
using ProductApi.Services;
using Xunit;

namespace ProductApi.Tests.IntegrationTests;

/// <summary>
/// Integration tests using InMemory Database — no mocking needed
/// Tests the full Repository → Service chain
/// </summary>
public class ProductIntegrationTests
{
    private AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetProducts_AfterAdding_ReturnsSingleProduct()
    {
        // Arrange
        using var context = CreateInMemoryContext("Test_GetSingle");
        context.Products.Add(new Product { Name = "Phone", Price = 20000 });
        context.SaveChanges();

        var repo = new ProductRepository(context);
        var service = new ProductService(repo);

        // Act
        var result = await service.GetAll();

        // Assert
        Assert.Single(result);
        Assert.Equal("Phone", result[0].Name);
    }

    [Fact]
    public async Task GetProductById_WhenExists_ReturnsCorrectProduct()
    {
        // Arrange
        using var context = CreateInMemoryContext("Test_GetById");
        context.Products.Add(new Product { Id = 10, Name = "Tablet", Price = 35000 });
        context.SaveChanges();

        var repo = new ProductRepository(context);
        var service = new ProductService(repo);

        // Act
        var result = await service.GetProductByIdAsync(10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Tablet", result.Name);
    }

    [Fact]
    public async Task GetProductById_WhenNotExists_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext("Test_GetById_Null");
        var repo = new ProductRepository(context);
        var service = new ProductService(repo);

        // Act
        var result = await service.GetProductByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddMultipleProducts_ReturnsAll()
    {
        // Arrange
        using var context = CreateInMemoryContext("Test_AddMultiple");
        var repo = new ProductRepository(context);
        var service = new ProductService(repo);

        // Act
        await service.Add(new Product { Name = "Laptop", Price = 75000 });
        await service.Add(new Product { Name = "Mouse", Price = 1500 });
        await service.Add(new Product { Name = "Keyboard", Price = 3000 });

        var all = await service.GetAll();

        // Assert
        Assert.Equal(3, all.Count);
    }

    [Fact]
    public async Task DeleteProduct_RemovesFromDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext("Test_Delete");
        context.Products.Add(new Product { Id = 5, Name = "Monitor", Price = 15000 });
        context.SaveChanges();

        var repo = new ProductRepository(context);
        var service = new ProductService(repo);

        // Act
        var deleted = await service.Delete(5);
        var allProducts = await service.GetAll();

        // Assert
        Assert.True(deleted);
        Assert.Empty(allProducts);
    }
}
