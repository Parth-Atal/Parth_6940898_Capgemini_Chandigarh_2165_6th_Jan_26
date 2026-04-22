using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces;

public interface IReportService
{
    Task<SalesReportDto> GetSalesReportAsync();
}