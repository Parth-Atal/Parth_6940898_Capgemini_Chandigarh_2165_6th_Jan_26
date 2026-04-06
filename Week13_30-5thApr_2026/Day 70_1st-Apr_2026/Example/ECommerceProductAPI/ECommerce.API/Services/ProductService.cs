using ECommerce.API.Models;
using ECommerce.API.Repositories;

namespace ECommerce.API.Services
{
    public class ProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _repo.GetAll();
        }

        public async Task<Product?> GetProduct(int id)
        {
            return await _repo.GetById(id);
        }

        public async Task AddProduct(Product product)
        {
            await _repo.Add(product);
        }

        public async Task UpdateProduct(Product product)
        {
            await _repo.Update(product);
        }

        public async Task DeleteProduct(int id)
        {
            await _repo.Delete(id);
        }
    }
}