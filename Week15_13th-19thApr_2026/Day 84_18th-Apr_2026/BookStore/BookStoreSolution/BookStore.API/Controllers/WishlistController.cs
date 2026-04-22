using System.Security.Claims;
using Asp.Versioning;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Data;
using BookStore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly BookStoreDbContext _context;
    public WishlistController(BookStoreDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetMyWishlist()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var items = await _context.Wishlists.Include(w => w.Book).ThenInclude(b => b.Author).Where(w => w.UserId == userId)
            .Select(w => new { w.Book.BookId, w.Book.Title, w.Book.Price, Author = w.Book.Author.Name }).ToListAsync();
        return Ok(ApiResponse<List<object>>.Ok(items.Cast<object>().ToList()));
    }

    [HttpPost("{bookId}")]
    public async Task<ActionResult<ApiResponse>> Add(int bookId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (await _context.Wishlists.AnyAsync(w => w.UserId == userId && w.BookId == bookId))
            return BadRequest(ApiResponse.Fail("Already in wishlist."));
        _context.Wishlists.Add(new Wishlist { UserId = userId, BookId = bookId });
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Ok("Added to wishlist."));
    }

    [HttpDelete("{bookId}")]
    public async Task<ActionResult<ApiResponse>> Remove(int bookId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var item = await _context.Wishlists.FindAsync(userId, bookId);
        if (item == null) return NotFound(ApiResponse.Fail("Not found.", 404));
        _context.Wishlists.Remove(item);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Ok("Removed from wishlist."));
    }
}