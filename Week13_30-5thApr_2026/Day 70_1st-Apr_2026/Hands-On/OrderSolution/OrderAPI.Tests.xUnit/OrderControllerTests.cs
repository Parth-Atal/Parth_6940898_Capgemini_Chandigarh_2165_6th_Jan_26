using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Controllers;
using InventoryAPI.Services;
using InventoryAPI.Models;
using System.Threading.Tasks;

namespace InventoryAPI.Tests.xUnit
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _mockService;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _mockService = new Mock<IOrderService>();
            _controller = new OrderController(_mockService.Object);
        }

        [Fact]
        public async Task PlaceOrder_ValidOrder_ReturnsCreated()
        {
            // Arrange
            var order = new Order { Id = 1, ProductName = "Phone", Quantity = 2 };

            _mockService
                .Setup(s => s.PlaceOrderAsync(order))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.PlaceOrder(order);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(order, createdResult.Value);
        }

        [Fact]
        public async Task PlaceOrder_InvalidOrder_ReturnsBadRequest()
        {
            // Arrange
            var order = new Order { Id = 2, ProductName = "Phone", Quantity = 0 };

            _mockService
                .Setup(s => s.PlaceOrderAsync(order))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.PlaceOrder(order);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}