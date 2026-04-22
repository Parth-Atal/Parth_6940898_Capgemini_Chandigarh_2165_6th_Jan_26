using BookStore.API.Controllers;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BookStore.Tests.NUnit;

[TestFixture]
public class BooksControllerTests
{
    private Mock<IBookService> _bookServiceMock = null!;
    private BooksController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _bookServiceMock = new Mock<IBookService>();
        _controller = new BooksController(_bookServiceMock.Object);
    }

    [Test]
    public async Task GetBooks_ReturnsOk()
    {
        var paged = new PaginatedResult<BookDto> { Items = new List<BookDto> { new() { BookId = 1, Title = "Book 1", AuthorName = "A1", CategoryName = "C1", PublisherName = "P1" } }, TotalCount = 1, PageNumber = 1, PageSize = 10 };
        _bookServiceMock.Setup(s => s.GetBooksAsync(1, 10, null, null)).ReturnsAsync(paged);
        var result = await _controller.GetBooks();
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task GetBook_NonExisting_ReturnsNotFound()
    {
        _bookServiceMock.Setup(s => s.GetBookByIdAsync(999)).ReturnsAsync((BookDto?)null);
        var result = await _controller.GetBook(999);
        Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task DeleteBook_Existing_ReturnsOk()
    {
        _bookServiceMock.Setup(s => s.SoftDeleteBookAsync(1)).ReturnsAsync(true);
        var result = await _controller.DeleteBook(1);
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
    }
}