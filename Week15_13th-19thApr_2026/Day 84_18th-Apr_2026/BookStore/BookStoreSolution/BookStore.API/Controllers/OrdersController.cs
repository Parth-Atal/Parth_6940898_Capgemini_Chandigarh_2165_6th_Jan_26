using System.Security.Claims;
using Asp.Versioning;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrdersController(IOrderService orderService) => _orderService = orderService;

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> PlaceOrder(OrderCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var order = await _orderService.PlaceOrderAsync(userId, dto);
        return Ok(ApiResponse<OrderResponseDto>.Ok(order, "Order placed."));
    }

    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetMyOrders()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var orders = await _orderService.GetUserOrdersAsync(userId);
        return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orders));
    }

    [HttpGet("all")] [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orders));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetOrder(int id)
    {
        var order = await _orderService.GetOrderDetailsAsync(id);
        if (order == null) return NotFound(ApiResponse.Fail("Order not found.", 404));
        return Ok(ApiResponse<OrderResponseDto>.Ok(order));
    }

    [HttpPatch("{id}/status")] [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse>> UpdateStatus(int id, [FromBody] string status)
    {
        var updated = await _orderService.UpdateOrderStatusAsync(id, status);
        if (!updated) return NotFound(ApiResponse.Fail("Order not found.", 404));
        return Ok(ApiResponse.Ok("Status updated."));
    }
}