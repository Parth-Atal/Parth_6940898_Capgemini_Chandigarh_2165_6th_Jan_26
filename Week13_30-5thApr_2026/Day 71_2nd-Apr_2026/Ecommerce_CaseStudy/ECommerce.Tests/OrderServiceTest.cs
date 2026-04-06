using Moq;

public class OrderServiceTest
{
    private readonly Mock<IOrderRepository> _repoMock;
    private readonly OrderService _service;

    public OrderServiceTest()
    {
        _repoMock = new Mock<IOrderRepository>();
        _service = new OrderService(_repoMock.Object);
    }

    [Fact]
    public async Task PlaceOrder_ShouldCallRepo()
    {
        var order = new Order();

        await _service.PlaceOrder(order);

        _repoMock.Verify(r => r.Add(order), Times.Once);
    }
}