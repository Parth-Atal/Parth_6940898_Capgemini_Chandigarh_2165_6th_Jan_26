using Moq;
using Xunit;
using ECommerce.API.Models;
using ECommerce.API.Repositories;
using ECommerce.API.Services;

namespace ECommerce.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly ProductService _service;

        // xUnit Constructor setup
        public ProductServiceTests()
        {
            _mockRepo = new Mock<IProductRepository>();
            _service = new ProductService(_mockRepo.Object);
        }

        // Test 1: GetAllProducts returns list
        [Fact]
        public async Task GetAllProducts_ReturnsList()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Price = 50000 },
                new Product { Id = 2, Name = "Phone",  Price = 30000 }
            };

            _mockRepo.Setup(x => x.GetAll()).ReturnsAsync(products);

            // Act
            var result = await _service.GetAllProducts();

            // Assert
            Assert.Equal(2, result.Count);        // must have 2 products
        }

        //  Test 2: GetProduct returns correct product
        [Fact]
        public async Task GetProduct_ReturnsProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Phone", Price = 30000 };

            _mockRepo.Setup(x => x.GetById(1)).ReturnsAsync(product);

            // Act
            var result = await _service.GetProduct(1);

            // Assert
            Assert.NotNull(result);               // must not be null
            Assert.Equal("Phone", result.Name);   // name must match
            Assert.Equal(30000, result.Price);    // price must match
        }

        //  Test 3: GetProduct returns null when not found
        [Fact]
        public async Task GetProduct_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetById(99)).ReturnsAsync((Product?)null);

            // Act
            var result = await _service.GetProduct(99);

            // Assert
            Assert.Null(result);                  // must be null
        }

        //  Test 4: AddProduct calls repository once
        [Fact]
        public async Task AddProduct_CallsRepositoryOnce()
        {
            // Arrange
            var product = new Product { Id = 3, Name = "Tablet", Price = 20000 };

            _mockRepo.Setup(x => x.Add(product)).Returns(Task.CompletedTask);

            // Act
            await _service.AddProduct(product);

            // Assert — verify Add was called exactly once
            _mockRepo.Verify(x => x.Add(product), Times.Once);
        }

        //  Test 5: UpdateProduct calls repository once
        [Fact]
        public async Task UpdateProduct_CallsRepositoryOnce()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Updated Laptop", Price = 60000 };

            _mockRepo.Setup(x => x.Update(product)).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateProduct(product);

            // Assert
            _mockRepo.Verify(x => x.Update(product), Times.Once);
        }

        // Test 6: DeleteProduct calls repository once
        [Fact]
        public async Task DeleteProduct_CallsRepositoryOnce()
        {
            // Arrange
            _mockRepo.Setup(x => x.Delete(1)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteProduct(1);

            // Assert
            _mockRepo.Verify(x => x.Delete(1), Times.Once);
        }
    }
}