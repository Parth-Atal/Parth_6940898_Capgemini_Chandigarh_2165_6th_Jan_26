using System.Text.Json;
using BookStore.Web.Models;
using BookStore.Web.Services;
using BookStore.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers;

public class BooksController : Controller
{
    private readonly ApiService _api;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };
    public BooksController(ApiService api) => _api = api;

    public async Task<IActionResult> Index(int page = 1, string? search = null)
    {
        var result = await _api.GetAsync<PaginatedResult<BookListItem>>($"/api/v1/books?page={page}&pageSize=12&search={search}");
        ViewBag.Search = search; ViewBag.Page = page;
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await _api.GetAsync<BookListItem>($"/api/v1/books/{id}");
        if (book == null) return NotFound();
        return View(book);
    }

    [HttpPost]
    public IActionResult AddToCart(int bookId, string title, decimal price)
    {
        var cart = GetCart();
        var existing = cart.Find(c => c.BookId == bookId);
        if (existing != null) existing.Qty++; else cart.Add(new CartItem { BookId = bookId, Title = title, Price = price, Qty = 1 });
        SaveCart(cart);
        return RedirectToAction("Cart");
    }

    public IActionResult Cart() => View(GetCart());

    [HttpPost]
    public async Task<IActionResult> Checkout()
    {
        var cart = GetCart();
        if (cart.Count == 0) return RedirectToAction("Cart");
        var items = cart.Select(c => new { c.BookId, c.Qty }).ToList();
        var response = await _api.PostAsync("/api/v1/orders", new { Items = items });
        if (response.Success) { SaveCart(new List<CartItem>()); TempData["Message"] = "Order placed successfully!"; return RedirectToAction("Index", "Orders"); }
        TempData["Error"] = response.Message;
        return RedirectToAction("Cart");
    }

    private List<CartItem> GetCart()
    {
        var json = HttpContext.Session.GetString("Cart");
        return string.IsNullOrEmpty(json) ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(json, JsonOpts) ?? new();
    }
    private void SaveCart(List<CartItem> cart) => HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
}