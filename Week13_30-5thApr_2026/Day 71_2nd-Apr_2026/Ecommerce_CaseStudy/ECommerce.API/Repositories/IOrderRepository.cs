public interface IOrderRepository
{
    Task<List<Order>> GetAll();
    Task Add(Order order);
}