using Asp.Versioning;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/books")]
[ApiController]
public class BooksV2Controller : ControllerBase
{
    private readonly IBookService _bookService;
    public BooksV2Controller(IBookService bookService) => _bookService = bookService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResult<BookDto>>>> GetBooks([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, [FromQuery] int? categoryId = null)
    {
        var result = await _bookService.GetBooksAsync(page, pageSize, search, categoryId);
        return Ok(ApiResponse<PaginatedResult<BookDto>>.Ok(result, "v2 response"));
    }
}