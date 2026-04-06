using InventoryAPI.Models;

namespace InventoryAPI.Services
{
    public interface IOrderService
    {
        Task<bool> PlaceOrderAsync(Order order);
    }
}