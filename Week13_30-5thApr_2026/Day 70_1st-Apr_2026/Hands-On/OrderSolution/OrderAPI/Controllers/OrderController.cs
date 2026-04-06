using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Services;
using InventoryAPI.Models;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(Order order)
        {
            var result = await _orderService.PlaceOrderAsync(order);

            if (result)
            {
                return Created("api/order", order); // 201
            }

            return BadRequest(); // 400
        }
    }
}