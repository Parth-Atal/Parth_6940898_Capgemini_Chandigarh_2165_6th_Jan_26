using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ Public API — Get all products
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_context.Products.ToList());
    }

    // ✅ Route Parameter + Constraint — Get by ID
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null) return NotFound(new { message = $"Product with ID {id} not found." });

        return Ok(product);
    }

    // ✅ Filtering — Get by category
    [HttpGet("category/{category}")]
    public IActionResult GetByCategory(string category)
    {
        var products = _context.Products
            .Where(p => p.Category.ToLower() == category.ToLower())
            .ToList();

        return Ok(products);
    }

    // 🔐 Admin Only — Create product
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Create([FromBody] Product product)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Products.Add(product);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // 🔐 Admin Only — Update product
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Product product)
    {
        var existing = _context.Products.Find(id);
        if (existing == null) return NotFound(new { message = $"Product with ID {id} not found." });

        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Category = product.Category;

        _context.SaveChanges();
        return NoContent();
    }

    // 🔐 Admin Only — Delete product
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null) return NotFound(new { message = $"Product with ID {id} not found." });

        _context.Products.Remove(product);
        _context.SaveChanges();
        return NoContent();
    }
}
