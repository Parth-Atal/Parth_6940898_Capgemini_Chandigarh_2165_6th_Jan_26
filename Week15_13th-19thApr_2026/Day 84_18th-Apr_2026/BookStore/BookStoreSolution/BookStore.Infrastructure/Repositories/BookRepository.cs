using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Data;
using BookStore.Shared;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(BookStoreDbContext context) : base(context) { }

    public async Task<PaginatedResult<Book>> GetPagedBooksAsync(int page, int pageSize, string? search, int? categoryId)
    {
        var query = _dbSet.Include(b => b.Category).Include(b => b.Author).Include(b => b.Publisher).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(b => b.Title.Contains(search) || b.Author.Name.Contains(search));

        if (categoryId.HasValue)
            query = query.Where(b => b.CategoryId == categoryId.Value);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(b => b.BookId).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedResult<Book> { Items = items, TotalCount = total, PageNumber = page, PageSize = pageSize };
    }

    public async Task<Book?> GetBookWithDetailsAsync(int id)
    {
        return await _dbSet.Include(b => b.Category).Include(b => b.Author).Include(b => b.Publisher).FirstOrDefaultAsync(b => b.BookId == id);
    }
}