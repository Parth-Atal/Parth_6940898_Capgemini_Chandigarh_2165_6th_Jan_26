using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductApi.Controllers;
using ProductApi.Models;
using ProductApi.Services;
using Xunit;

namespace ProductApi.Tests.xUnitTests;

/// <summary>
/// xUnit Tests for ProductsController using Moq
/// Case Study 1: Validates GetProduct returns 200 OK or 404 Not Found
/// </summary>
public class ProductControllerXUnitTests
{
    private readonly Mock<IProductService> _mockService;
    private readonly ProductsController _controller;

    public ProductControllerXUnitTests()
    {
        _mockService = new Mock<IProductService>();
        _controller = new ProductsController(_mockService.Object);
    }

    // ── Test 1: Product exists → 200 OK ─────────────────────────────────────
    [Fact]
    public async Task GetProduct_WhenProductExists_ReturnsOkWithProduct()
    {
        // Arrange
        var expectedProduct = new Product { Id = 1, Name = "Laptop", Price = 75000 };
        _mockService
            .Setup(s => s.GetProductByIdAsync(1))
            .ReturnsAsync(expectedProduct);

        // Act
        var result = await _controller.GetProduct(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProduct = Assert.IsType<Product>(okResult.Value);
        Assert.Equal(1, returnedProduct.Id);
        Assert.Equal("Laptop", returnedProduct.Name);
        Assert.Equal(75000, returnedProduct.Price);
    }

    // ── Test 2: Product does NOT exist → 404 Not Found ───────────────────────
    [Fact]
    public async Task GetProduct_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _mockService
            .Setup(s => s.GetProductByIdAsync(99))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _controller.GetProduct(99);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    // ── Test 3: GetAll → 200 OK with list ───────────────────────────────────
    [Fact]
    public async Task GetAll_ReturnsOkWithProductList()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 75000 },
            new Product { Id = 2, Name = "Mouse", Price = 1500 }
        };
        _mockService.Setup(s => s.GetAll()).ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedList = Assert.IsType<List<Product>>(okResult.Value);
        Assert.Equal(2, returnedList.Count);
    }

    // ── Test 4: Add product → 201 Created ────────────────────────────────────
    [Fact]
    public async Task AddProduct_ReturnsCreatedResult()
    {
        // Arrange
        var newProduct = new Product { Id = 3, Name = "Keyboard", Price = 3000 };
        _mockService.Setup(s => s.Add(newProduct)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Add(newProduct);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
        _mockService.Verify(s => s.Add(newProduct), Times.Once);
    }

    // ── Test 5: Delete existing product → 200 OK ────────────────────────────
    [Fact]
    public async Task DeleteProduct_WhenExists_ReturnsOk()
    {
        // Arrange
        _mockService.Setup(s => s.Delete(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    // ── Test 6: Delete non-existing product → 404 ────────────────────────────
    [Fact]
    public async Task DeleteProduct_WhenNotExists_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.Delete(99)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(99);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
