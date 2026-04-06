using InventoryAPI.Models;

namespace InventoryAPI.Services
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int id);
    }
}