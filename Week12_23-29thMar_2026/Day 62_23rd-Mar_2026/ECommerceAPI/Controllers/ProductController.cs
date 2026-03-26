using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.DTOs;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Products.ToList());
        }

        [HttpPost]
        public IActionResult Create(ProductDTO dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return Ok(product);
        }

        // Search
        [HttpGet("search")]
        public IActionResult Search(string name)
        {
            var data = _context.Products
                .Where(p => p.Name.Contains(name))
                .ToList();

            return Ok(data);
        }

        // Pagination
        [HttpGet("paged")]
        public IActionResult GetPaged(int page = 1, int pageSize = 5)
        {
            var data = _context.Products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(data);
        }
    }
}