using ECommerce.API.Models;

namespace ECommerce.API.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAll();
        Task<Product?> GetById(int id);
        Task Add(Product product);
        Task Update(Product product);
        Task Delete(int id);
    }
}