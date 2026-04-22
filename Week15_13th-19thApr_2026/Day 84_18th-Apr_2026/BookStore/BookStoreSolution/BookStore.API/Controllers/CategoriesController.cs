using Asp.Versioning;
using BookStore.Application.DTOs;
using BookStore.Infrastructure.Data;
using BookStore.Domain.Entities;
using BookStore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly BookStoreDbContext _context;
    public CategoriesController(BookStoreDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetAll()
    {
        var categories = await _context.Categories.Select(c => new CategoryDto { CategoryId = c.CategoryId, Name = c.Name }).ToListAsync();
        return Ok(ApiResponse<List<CategoryDto>>.Ok(categories));
    }

    [HttpPost] [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse>> Create(CategoryDto dto)
    {
        _context.Categories.Add(new Category { Name = dto.Name });
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Ok("Category created."));
    }

    [HttpGet("authors")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetAuthors()
    {
        var authors = await _context.Authors.Select(a => new { a.AuthorId, a.Name }).ToListAsync();
        return Ok(ApiResponse<List<object>>.Ok(authors.Cast<object>().ToList()));
    }

    [HttpGet("publishers")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetPublishers()
    {
        var publishers = await _context.Publishers.Select(p => new { p.PublisherId, p.Name }).ToListAsync();
        return Ok(ApiResponse<List<object>>.Ok(publishers.Cast<object>().ToList()));
    }
}