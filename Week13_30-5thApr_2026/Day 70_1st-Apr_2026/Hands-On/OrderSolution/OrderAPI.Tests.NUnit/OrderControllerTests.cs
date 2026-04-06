using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Controllers;
using InventoryAPI.Services;
using InventoryAPI.Models;
using System.Threading.Tasks;

namespace InventoryAPI.Tests.NUnit
{
    public class OrderControllerTests
    {
        private Mock<IOrderService> _mockService;
        private OrderController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IOrderService>();
            _controller = new OrderController(_mockService.Object);
        }

        [Test]
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
            Assert.That(result, Is.InstanceOf<CreatedResult>());

            var createdResult = result as CreatedResult;
            var returnedOrder = createdResult?.Value as Order;

            Assert.That(returnedOrder, Is.Not.Null);
            Assert.That(returnedOrder.Id, Is.EqualTo(1));
        }

        [Test]
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
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }
    }
}