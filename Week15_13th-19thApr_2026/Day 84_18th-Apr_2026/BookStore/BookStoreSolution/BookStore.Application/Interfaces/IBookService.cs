using BookStore.Application.DTOs;
using BookStore.Shared;

namespace BookStore.Application.Interfaces;

public interface IBookService
{
    Task<PaginatedResult<BookDto>> GetBooksAsync(int page, int pageSize, string? search, int? categoryId);
    Task<BookDto?> GetBookByIdAsync(int id);
    Task<BookDto> CreateBookAsync(BookCreateDto dto);
    Task<bool> UpdateBookAsync(int id, BookUpdateDto dto);
    Task<bool> SoftDeleteBookAsync(int id);
}