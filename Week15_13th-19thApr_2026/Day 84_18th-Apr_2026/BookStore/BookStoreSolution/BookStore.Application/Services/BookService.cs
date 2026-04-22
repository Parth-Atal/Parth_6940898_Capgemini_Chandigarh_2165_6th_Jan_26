using AutoMapper;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Shared;

namespace BookStore.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepo;
    private readonly IMapper _mapper;

    public BookService(IBookRepository bookRepo, IMapper mapper)
    {
        _bookRepo = bookRepo;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<BookDto>> GetBooksAsync(int page, int pageSize, string? search, int? categoryId)
    {
        var result = await _bookRepo.GetPagedBooksAsync(page, pageSize, search, categoryId);
        return new PaginatedResult<BookDto>
        {
            Items = _mapper.Map<List<BookDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<BookDto?> GetBookByIdAsync(int id)
    {
        var book = await _bookRepo.GetBookWithDetailsAsync(id);
        return book == null ? null : _mapper.Map<BookDto>(book);
    }

    public async Task<BookDto> CreateBookAsync(BookCreateDto dto)
    {
        var book = _mapper.Map<Book>(dto);
        await _bookRepo.AddAsync(book);
        await _bookRepo.SaveChangesAsync();
        var created = await _bookRepo.GetBookWithDetailsAsync(book.BookId);
        return _mapper.Map<BookDto>(created!);
    }

    public async Task<bool> UpdateBookAsync(int id, BookUpdateDto dto)
    {
        var book = await _bookRepo.GetByIdAsync(id);
        if (book == null) return false;
        _mapper.Map(dto, book);
        _bookRepo.Update(book);
        await _bookRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteBookAsync(int id)
    {
        var book = await _bookRepo.GetByIdAsync(id);
        if (book == null) return false;
        book.IsDeleted = true;
        _bookRepo.Update(book);
        await _bookRepo.SaveChangesAsync();
        return true;
    }
}