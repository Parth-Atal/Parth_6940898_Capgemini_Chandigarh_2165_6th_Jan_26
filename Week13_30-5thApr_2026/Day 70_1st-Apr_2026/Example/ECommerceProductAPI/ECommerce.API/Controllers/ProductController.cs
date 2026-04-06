using ECommerce.API.Models;
using ECommerce.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductController(ProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllProducts();
            return Ok(products);                           // 200
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetProduct(id);
            if (product == null)
                return NotFound();                         // 404
            return Ok(product);                            // 200
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Product product)
        {
            await _service.AddProduct(product);
            return CreatedAtAction(nameof(GetById),
                new { id = product.Id }, product);         // 201
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product product)
        {
            if (id != product.Id)
                return BadRequest();                       // 400
            await _service.UpdateProduct(product);
            return NoContent();                            // 204
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteProduct(id);
            return NoContent();                            // 204
        }
    }
}