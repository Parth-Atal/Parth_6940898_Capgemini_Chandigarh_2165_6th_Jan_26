using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Models;

namespace ProductApi.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAll() =>
        await _context.Products.ToListAsync();

    public async Task<Product?> GetById(int id) =>
        await _context.Products.FindAsync(id);

    public async Task Add(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Update(Product product)
    {
        var existing = await _context.Products.FindAsync(product.Id);
        if (existing == null) return false;

        existing.Name = product.Name;
        existing.Price = product.Price;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}
