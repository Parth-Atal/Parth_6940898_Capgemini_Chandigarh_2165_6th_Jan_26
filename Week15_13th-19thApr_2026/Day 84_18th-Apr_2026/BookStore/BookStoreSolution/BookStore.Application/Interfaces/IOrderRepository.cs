using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId);
    Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync();
    Task<Order?> GetOrderWithItemsAsync(int orderId);
}