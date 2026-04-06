using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Controllers;
using InventoryAPI.Services;
using InventoryAPI.Models;
using System.Threading.Tasks;

namespace InventoryAPI.Tests.xUnit
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductController(_mockService.Object);
        }

        [Fact]
        public async Task GetProduct_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Laptop", Price = 1000 };

            _mockService
                .Setup(s => s.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.GetProduct(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);

            Assert.Equal(1, returnedProduct.Id);
            Assert.Equal("Laptop", returnedProduct.Name);
        }

        [Fact]
        public async Task GetProduct_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetProductByIdAsync(99))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _controller.GetProduct(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}