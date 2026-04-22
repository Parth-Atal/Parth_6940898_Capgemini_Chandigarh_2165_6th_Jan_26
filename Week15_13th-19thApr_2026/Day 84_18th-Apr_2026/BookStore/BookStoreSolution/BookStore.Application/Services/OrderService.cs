using AutoMapper;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;

namespace BookStore.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IEmailService _emailService;
    private readonly IGenericRepository<User> _userRepo;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepo, IBookRepository bookRepo,
        IEmailService emailService, IGenericRepository<User> userRepo, IMapper mapper)
    {
        _orderRepo = orderRepo;
        _bookRepo = bookRepo;
        _emailService = emailService;
        _userRepo = userRepo;
        _mapper = mapper;
    }

    public async Task<OrderResponseDto> PlaceOrderAsync(int userId, OrderCreateDto dto)
    {
        var order = new Order { UserId = userId };

        foreach (var item in dto.Items)
        {
            var book = await _bookRepo.GetByIdAsync(item.BookId)
                ?? throw new KeyNotFoundException($"Book {item.BookId} not found.");

            if (book.Stock < item.Qty)
                throw new InvalidOperationException($"Insufficient stock for \"{book.Title}\".");

            book.Stock -= item.Qty;
            _bookRepo.Update(book);

            if (book.Stock < 5)
                await _emailService.SendLowStockAlertAsync(book.Title, book.Stock);

            order.OrderItems.Add(new OrderItem
            {
                BookId = item.BookId,
                Qty = item.Qty,
                Price = book.Price
            });
        }

        order.TotalAmount = order.OrderItems.Sum(oi => oi.Price * oi.Qty);
        await _orderRepo.AddAsync(order);
        await _orderRepo.SaveChangesAsync();

        var saved = await _orderRepo.GetOrderWithItemsAsync(order.OrderId);
        
        // Send email to customer
        var customer = await _userRepo.GetByIdAsync(userId);
        if (customer != null)
        {
            await _emailService.SendOrderConfirmationAsync(customer.Email, order.OrderId, order.TotalAmount);
        }

        // Send email to admins
        var admins = (await _userRepo.FindAsync(u => u.Role.RoleName == "Admin")).ToList();
        foreach (var admin in admins)
        {
            await _emailService.SendOrderReceivedToAdminAsync(admin.Email, order.OrderId, customer?.FullName ?? "Unknown", order.TotalAmount);
        }

        return _mapper.Map<OrderResponseDto>(saved!);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetUserOrdersAsync(int userId)
    {
        var orders = await _orderRepo.GetOrdersByUserAsync(userId);
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepo.GetAllOrdersWithDetailsAsync();
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<OrderResponseDto?> GetOrderDetailsAsync(int orderId)
    {
        var order = await _orderRepo.GetOrderWithItemsAsync(orderId);
        return order == null ? null : _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null) return false;
        
        order.Status = status;
        _orderRepo.Update(order);
        await _orderRepo.SaveChangesAsync();

        // Send email to customer about status change
        var customer = await _userRepo.GetByIdAsync(order.UserId);
        if (customer != null)
        {
            await _emailService.SendOrderStatusUpdateToCustomerAsync(customer.Email, orderId, customer.FullName, status);
        }

        return true;
    }
}