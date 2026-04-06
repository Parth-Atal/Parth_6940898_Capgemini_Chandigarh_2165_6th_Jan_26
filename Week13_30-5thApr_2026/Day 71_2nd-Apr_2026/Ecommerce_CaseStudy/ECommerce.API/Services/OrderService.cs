public class OrderService : IOrderService
{
    private readonly IOrderRepository _repo;

    public OrderService(IOrderRepository repo)
    {
        _repo = repo;
    }

    public async Task PlaceOrder(Order order)
    {
        // Business logic
        order.OrderDate = DateTime.Now;

        await _repo.Add(order);
    }

    public async Task<List<Order>> GetAllOrders()
    {
        return await _repo.GetAll();
    }
}