using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Data;
using ECommerceAPI.Models;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            var data = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .ToList();

            return Ok(data);
        }

        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
            order.OrderDate = DateTime.Now;

            _context.Orders.Add(order);
            _context.SaveChanges();

            return Ok(order);
        }
    }
}