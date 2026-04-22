using BookStore.Domain.Entities;
using BookStore.Shared;

namespace BookStore.Application.Interfaces;

public interface IBookRepository : IGenericRepository<Book>
{
    Task<PaginatedResult<Book>> GetPagedBooksAsync(int page, int pageSize, string? search, int? categoryId);
    Task<Book?> GetBookWithDetailsAsync(int id);
}