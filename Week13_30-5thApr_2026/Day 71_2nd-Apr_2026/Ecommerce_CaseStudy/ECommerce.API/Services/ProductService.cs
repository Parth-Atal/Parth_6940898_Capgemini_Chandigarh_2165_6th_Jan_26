using Microsoft.EntityFrameworkCore;

public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetProducts()
    {
        return await _context.Products
            .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
            .ToListAsync();
    }

    public async Task<Product?> GetProduct(int id)
    {
        return await _context.Products
            .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> AddProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> UpdateProduct(int id, Product product)
    {
        var existing = await _context.Products.FindAsync(id);
        if (existing == null) return false;

        existing.Name = product.Name;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProduct(int id)
    {
        var existing = await _context.Products.FindAsync(id);
        if (existing == null) return false;

        _context.Products.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}