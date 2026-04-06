public interface IOrderService
{
    Task PlaceOrder(Order order);
    Task<List<Order>> GetAllOrders();
}