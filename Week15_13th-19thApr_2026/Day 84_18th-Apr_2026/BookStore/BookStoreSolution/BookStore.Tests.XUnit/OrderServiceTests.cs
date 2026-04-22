using AutoMapper;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Application.MappingProfiles;
using BookStore.Application.Services;
using BookStore.Domain.Entities;
using Moq;

namespace BookStore.Tests.XUnit;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly IMapper _mapper;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _orderRepoMock = new Mock<IOrderRepository>();
        _bookRepoMock = new Mock<IBookRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _service = new OrderService(_orderRepoMock.Object, _bookRepoMock.Object, _emailServiceMock.Object, _mapper);
    }

    [Fact]
    public async Task PlaceOrder_ValidItems_ReducesStock()
    {
        var book = new Book { BookId = 1, Title = "Test", Price = 100, Stock = 10 };
        _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _orderRepoMock.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        _orderRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _orderRepoMock.Setup(r => r.GetOrderWithItemsAsync(It.IsAny<int>()))
            .ReturnsAsync(new Order { OrderId = 1, TotalAmount = 200, Status = "Pending", User = new User { FullName = "Test" }, OrderItems = new List<OrderItem> { new() { BookId = 1, Qty = 2, Price = 100, Book = book } } });

        var dto = new OrderCreateDto { Items = new List<OrderItemCreateDto> { new() { BookId = 1, Qty = 2 } } };
        var result = await _service.PlaceOrderAsync(1, dto);
        Assert.Equal(200, result.TotalAmount);
        Assert.Equal(8, book.Stock);
    }

    [Fact]
    public async Task PlaceOrder_InsufficientStock_ThrowsException()
    {
        var book = new Book { BookId = 1, Title = "Limited", Price = 50, Stock = 1 };
        _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        var dto = new OrderCreateDto { Items = new List<OrderItemCreateDto> { new() { BookId = 1, Qty = 5 } } };
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.PlaceOrderAsync(1, dto));
    }

    [Fact]
    public async Task PlaceOrder_BookNotFound_ThrowsKeyNotFound()
    {
        _bookRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Book?)null);
        var dto = new OrderCreateDto { Items = new List<OrderItemCreateDto> { new() { BookId = 999, Qty = 1 } } };
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.PlaceOrderAsync(1, dto));
    }
}