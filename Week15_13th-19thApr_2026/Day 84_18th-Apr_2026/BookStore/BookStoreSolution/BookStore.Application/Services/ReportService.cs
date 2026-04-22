using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;

namespace BookStore.Application.Services;

public class ReportService : IReportService
{
    private readonly IOrderRepository _orderRepo;

    public ReportService(IOrderRepository orderRepo) => _orderRepo = orderRepo;

    public async Task<SalesReportDto> GetSalesReportAsync()
    {
        var orders = (await _orderRepo.GetAllOrdersWithDetailsAsync()).ToList();

        var topBooks = orders
            .SelectMany(o => o.OrderItems)
            .GroupBy(oi => oi.Book?.Title ?? "Unknown")
            .Select(g => new TopSellingBookDto
            {
                Title = g.Key,
                TotalSold = g.Sum(x => x.Qty),
                Revenue = g.Sum(x => x.Price * x.Qty)
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(10)
            .ToList();

        return new SalesReportDto
        {
            TotalOrders = orders.Count,
            TotalRevenue = orders.Sum(o => o.TotalAmount),
            TopBooks = topBooks
        };
    }
}