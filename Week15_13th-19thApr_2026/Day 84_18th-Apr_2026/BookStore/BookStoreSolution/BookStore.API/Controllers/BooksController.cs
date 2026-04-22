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
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    public BooksController(IBookService bookService) => _bookService = bookService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResult<BookDto>>>> GetBooks([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] int? categoryId = null)
    {
        var result = await _bookService.GetBooksAsync(page, pageSize, search, categoryId);
        return Ok(ApiResponse<PaginatedResult<BookDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<BookDto>>> GetBook(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null) return NotFound(ApiResponse.Fail("Book not found.", 404));
        return Ok(ApiResponse<BookDto>.Ok(book));
    }

    [HttpPost] [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<BookDto>>> CreateBook(BookCreateDto dto)
    {
        var book = await _bookService.CreateBookAsync(dto);
        return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, ApiResponse<BookDto>.Ok(book, "Book created."));
    }

    [HttpPut("{id}")] [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse>> UpdateBook(int id, BookUpdateDto dto)
    {
        var updated = await _bookService.UpdateBookAsync(id, dto);
        if (!updated) return NotFound(ApiResponse.Fail("Book not found.", 404));
        return Ok(ApiResponse.Ok("Book updated."));
    }

    [HttpDelete("{id}")] [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse>> DeleteBook(int id)
    {
        var deleted = await _bookService.SoftDeleteBookAsync(id);
        if (!deleted) return NotFound(ApiResponse.Fail("Book not found.", 404));
        return Ok(ApiResponse.Ok("Book deleted."));
    }
}