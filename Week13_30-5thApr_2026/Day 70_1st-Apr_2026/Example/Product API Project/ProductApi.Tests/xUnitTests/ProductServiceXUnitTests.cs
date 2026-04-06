using Moq;
using ProductApi.Models;
using ProductApi.Repositories;
using ProductApi.Services;
using Xunit;

namespace ProductApi.Tests.xUnitTests;

/// <summary>
/// xUnit Tests for ProductService using Moq
/// </summary>
public class ProductServiceXUnitTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductService _service;

    public ProductServiceXUnitTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _service = new ProductService(_mockRepo.Object);
    }

    [Fact]
    public async Task AddProduct_ShouldCallRepositoryOnce()
    {
        // Arrange
        var product = new Product { Name = "Laptop", Price = 75000 };

        // Act
        await _service.Add(product);

        // Assert — verify repo.Add was called exactly once
        _mockRepo.Verify(r => r.Add(product), Times.Once);
    }

    [Fact]
    public async Task GetProductById_WhenExists_ReturnsProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Mouse", Price = 1500 };
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(product);

        // Act
        var result = await _service.GetProductByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Mouse", result.Name);
    }

    [Fact]
    public async Task GetProductById_WhenNotExists_ReturnsNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetById(99)).ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetProductByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAll_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop" },
            new Product { Id = 2, Name = "Phone" }
        };
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(products);

        // Act
        var result = await _service.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
    }
}
