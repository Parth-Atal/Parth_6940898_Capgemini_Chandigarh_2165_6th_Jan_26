using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;
using ProductApi.Services;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _service.GetAll();
        return Ok(products);
    }

    /// <summary>
    /// Get a product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _service.GetProductByIdAsync(id);
        if (product == null)
            return NotFound(new { Message = $"Product with ID {id} not found." });

        return Ok(product);
    }

    /// <summary>
    /// Add a new product
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Product product)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.Add(product);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Product product)
    {
        if (id != product.Id)
            return BadRequest(new { Message = "ID mismatch." });

        var updated = await _service.Update(product);
        if (!updated)
            return NotFound(new { Message = $"Product with ID {id} not found." });

        return Ok(new { Message = "Product updated successfully." });
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.Delete(id);
        if (!deleted)
            return NotFound(new { Message = $"Product with ID {id} not found." });

        return Ok(new { Message = "Product deleted successfully." });
    }
}
