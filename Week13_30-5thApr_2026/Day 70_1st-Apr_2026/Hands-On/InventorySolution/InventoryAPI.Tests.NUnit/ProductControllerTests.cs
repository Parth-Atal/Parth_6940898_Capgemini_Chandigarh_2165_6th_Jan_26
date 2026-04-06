using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Controllers;
using InventoryAPI.Services;
using InventoryAPI.Models;
using System.Threading.Tasks;

namespace InventoryAPI.Tests.NUnit
{
    public class ProductControllerTests
    {
        private Mock<IProductService> _mockService;
        private ProductController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductController(_mockService.Object);
        }

        [Test]
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
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okResult = result as OkObjectResult;
            var returnedProduct = okResult?.Value as Product;

            Assert.That(returnedProduct, Is.Not.Null);
            Assert.That(returnedProduct.Id, Is.EqualTo(1));
            Assert.That(returnedProduct.Name, Is.EqualTo("Laptop"));
        }

        [Test]
        public async Task GetProduct_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetProductByIdAsync(99))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _controller.GetProduct(99);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}