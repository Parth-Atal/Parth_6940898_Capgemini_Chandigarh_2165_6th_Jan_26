using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponseDto> PlaceOrderAsync(int userId, OrderCreateDto dto);
    Task<IEnumerable<OrderResponseDto>> GetUserOrdersAsync(int userId);
    Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();
    Task<OrderResponseDto?> GetOrderDetailsAsync(int orderId);
    Task<bool> UpdateOrderStatusAsync(int orderId, string status);
}