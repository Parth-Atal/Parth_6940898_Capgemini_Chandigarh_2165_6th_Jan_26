using EcommerceAPI.Controllers;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EcommerceAPI.Tests;

public class ProductTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>Creates a fresh in-memory DbContext for each test.</summary>
    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new AppDbContext(options);
    }

    // ── GetAll ────────────────────────────────────────────────────────────────

    [Fact]
    public void GetAll_ReturnsOkWithAllProducts()
    {
        using var context = CreateContext(nameof(GetAll_ReturnsOkWithAllProducts));
        context.Products.AddRange(
            new Product { Id = 1, Name = "Laptop",  Price = 999.99m, Category = "Electronics" },
            new Product { Id = 2, Name = "T-Shirt", Price = 19.99m,  Category = "Clothing"    }
        );
        context.SaveChanges();

        var controller = new ProductsController(context);
        var result = controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value);
        Assert.Equal(2, products.Count());
    }

    [Fact]
    public void GetAll_ReturnsEmptyList_WhenNoProducts()
    {
        using var context = CreateContext(nameof(GetAll_ReturnsEmptyList_WhenNoProducts));

        var controller = new ProductsController(context);
        var result = controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value);
        Assert.Empty(products);
    }

    // ── GetById ───────────────────────────────────────────────────────────────

    [Fact]
    public void GetById_ReturnsOk_WhenProductExists()
    {
        using var context = CreateContext(nameof(GetById_ReturnsOk_WhenProductExists));
        context.Products.Add(new Product { Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics" });
        context.SaveChanges();

        var controller = new ProductsController(context);
        var result = controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var product = Assert.IsType<Product>(ok.Value);
        Assert.Equal("Laptop", product.Name);
    }

    [Fact]
    public void GetById_ReturnsNotFound_WhenProductMissing()
    {
        using var context = CreateContext(nameof(GetById_ReturnsNotFound_WhenProductMissing));

        var controller = new ProductsController(context);
        var result = controller.GetById(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // ── GetByCategory ─────────────────────────────────────────────────────────

    [Fact]
    public void GetByCategory_ReturnsMatchingProducts()
    {
        using var context = CreateContext(nameof(GetByCategory_ReturnsMatchingProducts));
        context.Products.AddRange(
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics" },
            new Product { Id = 2, Name = "Phone",  Price = 499.99m, Category = "Electronics" },
            new Product { Id = 3, Name = "Shirt",  Price = 29.99m,  Category = "Clothing"    }
        );
        context.SaveChanges();

        var controller = new ProductsController(context);
        var result = controller.GetByCategory("Electronics");

        var ok = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value);
        Assert.Equal(2, products.Count());
        Assert.All(products, p => Assert.Equal("Electronics", p.Category));
    }

    [Fact]
    public void GetByCategory_IsCaseInsensitive()
    {
        using var context = CreateContext(nameof(GetByCategory_IsCaseInsensitive));
        context.Products.Add(new Product { Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics" });
        context.SaveChanges();

        var controller = new ProductsController(context);
        var result = controller.GetByCategory("electronics");   // lowercase

        var ok = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value);
        Assert.Single(products);
    }

    [Fact]
    public void GetByCategory_ReturnsEmpty_WhenCategoryNotFound()
    {
        using var context = CreateContext(nameof(GetByCategory_ReturnsEmpty_WhenCategoryNotFound));
        context.Products.Add(new Product { Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics" });
        context.SaveChanges();

        var controller = new ProductsController(context);
        var result = controller.GetByCategory("NonExistent");

        var ok = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value);
        Assert.Empty(products);
    }

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_ReturnsCreatedAtAction_AndPersistsProduct()
    {
        using var context = CreateContext(nameof(Create_ReturnsCreatedAtAction_AndPersistsProduct));
        var controller = new ProductsController(context);

        var newProduct = new Product { Name = "Mouse", Price = 29.99m, Category = "Electronics" };
        var result = controller.Create(newProduct);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var product = Assert.IsType<Product>(created.Value);
        Assert.Equal("Mouse", product.Name);
        Assert.Equal(1, context.Products.Count());
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public void Update_ReturnsNoContent_WhenProductExists()
    {
        using var context = CreateContext(nameof(Update_ReturnsNoContent_WhenProductExists));
        context.Products.Add(new Product { Id = 1, Name = "OldName", Price = 10m, Category = "OldCat" });
        context.SaveChanges();

        var controller = new ProductsController(context);
        var updated = new Product { Name = "NewName", Price = 20m, Category = "NewCat" };
        var result = controller.Update(1, updated);

        Assert.IsType<NoContentResult>(result);

        var product = context.Products.Find(1);
        Assert.Equal("NewName", product!.Name);
        Assert.Equal(20m, product.Price);
        Assert.Equal("NewCat", product.Category);
    }

    [Fact]
    public void Update_ReturnsNotFound_WhenProductMissing()
    {
        using var context = CreateContext(nameof(Update_ReturnsNotFound_WhenProductMissing));
        var controller = new ProductsController(context);

        var result = controller.Update(999, new Product { Name = "X", Price = 1m, Category = "Y" });

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    [Fact]
    public void Delete_ReturnsNoContent_AndRemovesProduct()
    {
        using var context = CreateContext(nameof(Delete_ReturnsNoContent_AndRemovesProduct));
        context.Products.Add(new Product { Id = 1, Name = "ToDelete", Price = 5m, Category = "Misc" });
        context.SaveChanges();

        var controller = new ProductsController(context);
        var result = controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(0, context.Products.Count());
    }

    [Fact]
    public void Delete_ReturnsNotFound_WhenProductMissing()
    {
        using var context = CreateContext(nameof(Delete_ReturnsNotFound_WhenProductMissing));
        var controller = new ProductsController(context);

        var result = controller.Delete(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
