using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProductApi.Controllers;
using ProductApi.Models;
using ProductApi.Services;

namespace ProductApi.Tests.NUnitTests;

/// <summary>
/// NUnit Tests for ProductsController using Moq
/// Same scenarios as xUnit — demonstrates how to write the same tests in NUnit
/// </summary>
[TestFixture]
public class ProductControllerNUnitTests
{
    private Mock<IProductService> _mockService = null!;
    private ProductsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IProductService>();
        _controller = new ProductsController(_mockService.Object);
    }

    // ── Test 1: Product exists → 200 OK ─────────────────────────────────────
    [Test]
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
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        var returnedProduct = (Product)okResult.Value!;
        Assert.That(returnedProduct.Id, Is.EqualTo(1));
        Assert.That(returnedProduct.Name, Is.EqualTo("Laptop"));
    }

    // ── Test 2: Product does NOT exist → 404 Not Found ───────────────────────
    [Test]
    public async Task GetProduct_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _mockService
            .Setup(s => s.GetProductByIdAsync(99))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _controller.GetProduct(99);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    // ── Test 3: GetAll → 200 OK with list ───────────────────────────────────
    [Test]
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
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        var returnedList = (List<Product>)okResult.Value!;
        Assert.That(returnedList.Count, Is.EqualTo(2));
    }

    // ── Test 4: Add product → 201 Created ────────────────────────────────────
    [Test]
    public async Task AddProduct_ReturnsCreatedResult()
    {
        // Arrange
        var newProduct = new Product { Id = 3, Name = "Keyboard", Price = 3000 };
        _mockService.Setup(s => s.Add(newProduct)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Add(newProduct);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
        _mockService.Verify(s => s.Add(newProduct), Times.Once);
    }

    // ── Test 5: Delete existing product → 200 OK ────────────────────────────
    [Test]
    public async Task DeleteProduct_WhenExists_ReturnsOk()
    {
        _mockService.Setup(s => s.Delete(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // ── Test 6: Delete non-existing product → 404 ────────────────────────────
    [Test]
    public async Task DeleteProduct_WhenNotExists_ReturnsNotFound()
    {
        _mockService.Setup(s => s.Delete(99)).ReturnsAsync(false);

        var result = await _controller.Delete(99);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }
}
