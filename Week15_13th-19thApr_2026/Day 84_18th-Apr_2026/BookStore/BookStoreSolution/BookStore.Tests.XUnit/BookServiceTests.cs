using AutoMapper;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Application.MappingProfiles;
using BookStore.Application.Services;
using BookStore.Domain.Entities;
using BookStore.Shared;
using Moq;

namespace BookStore.Tests.XUnit;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly IMapper _mapper;
    private readonly BookService _service;

    public BookServiceTests()
    {
        _bookRepoMock = new Mock<IBookRepository>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _service = new BookService(_bookRepoMock.Object, _mapper);
    }

    [Fact]
    public async Task GetBookById_ExistingId_ReturnsBookDto()
    {
        var book = new Book { BookId = 1, Title = "Test Book", ISBN = "123-456", Price = 299, Stock = 10, Category = new Category { Name = "Fiction" }, Author = new Author { Name = "Test Author" }, Publisher = new Publisher { Name = "Test Publisher" } };
        _bookRepoMock.Setup(r => r.GetBookWithDetailsAsync(1)).ReturnsAsync(book);
        var result = await _service.GetBookByIdAsync(1);
        Assert.NotNull(result);
        Assert.Equal("Test Book", result.Title);
    }

    [Fact]
    public async Task GetBookById_NonExistingId_ReturnsNull()
    {
        _bookRepoMock.Setup(r => r.GetBookWithDetailsAsync(999)).ReturnsAsync((Book?)null);
        var result = await _service.GetBookByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task SoftDeleteBook_ExistingId_ReturnsTrue()
    {
        var book = new Book { BookId = 1, Title = "To Delete", IsDeleted = false };
        _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _bookRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        var result = await _service.SoftDeleteBookAsync(1);
        Assert.True(result);
        Assert.True(book.IsDeleted);
    }

    [Fact]
    public async Task SoftDeleteBook_NonExistingId_ReturnsFalse()
    {
        _bookRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Book?)null);
        var result = await _service.SoftDeleteBookAsync(999);
        Assert.False(result);
    }

    [Fact]
    public async Task GetBooks_ReturnsPagedResult()
    {
        var paged = new PaginatedResult<Book>
        {
            Items = new List<Book>
            {
                new() { BookId = 1, Title = "Book A", Category = new Category { Name = "C1" }, Author = new Author { Name = "A1" }, Publisher = new Publisher { Name = "P1" } },
                new() { BookId = 2, Title = "Book B", Category = new Category { Name = "C2" }, Author = new Author { Name = "A2" }, Publisher = new Publisher { Name = "P2" } }
            },
            TotalCount = 2, PageNumber = 1, PageSize = 10
        };
        _bookRepoMock.Setup(r => r.GetPagedBooksAsync(1, 10, null, null)).ReturnsAsync(paged);
        var result = await _service.GetBooksAsync(1, 10, null, null);
        Assert.Equal(2, result.Items.Count);
    }
}