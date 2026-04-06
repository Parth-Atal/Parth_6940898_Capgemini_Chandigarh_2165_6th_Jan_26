using ProductApi.Models;
using ProductApi.Repositories;

namespace ProductApi.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<Product>> GetAll() =>
        await _repo.GetAll();

    public async Task<Product?> GetProductByIdAsync(int id) =>
        await _repo.GetById(id);

    public async Task Add(Product product) =>
        await _repo.Add(product);

    public async Task<bool> Update(Product product) =>
        await _repo.Update(product);

    public async Task<bool> Delete(int id) =>
        await _repo.Delete(id);
}
