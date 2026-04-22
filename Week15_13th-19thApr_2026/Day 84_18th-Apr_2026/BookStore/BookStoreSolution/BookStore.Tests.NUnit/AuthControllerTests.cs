using BookStore.API.Controllers;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Security.Cryptography;
using System.Text;

namespace BookStore.Tests.NUnit;

[TestFixture]
public class AuthControllerTests
{
    private BookStoreDbContext _context = null!;
    private Mock<ITokenService> _tokenServiceMock = null!;
    private AuthController _controller = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<BookStoreDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _context = new BookStoreDbContext(options);
        _tokenServiceMock = new Mock<ITokenService>();
        _controller = new AuthController(_context, _tokenServiceMock.Object);
        _context.Roles.Add(new Role { RoleId = 2, RoleName = "Customer" });
        _context.SaveChanges();
    }

    [TearDown]
    public void TearDown() { _context.Database.EnsureDeleted(); _context.Dispose(); }

    [Test]
    public async Task Register_NewUser_ReturnsOk()
    {
        var dto = new UserRegisterDto { FullName = "Test User", Email = "test@example.com", Password = "Password1!", Phone = "9876543210" };
        var result = await _controller.Register(dto);
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes("Password1!")));
        _context.Users.Add(new User { FullName = "Existing", Email = "dup@example.com", PasswordHash = hash, Phone = "1234567890", RoleId = 2 });
        await _context.SaveChangesAsync();
        var dto = new UserRegisterDto { FullName = "New", Email = "dup@example.com", Password = "Password1!", Phone = "9999999999" };
        var result = await _controller.Register(dto);
        Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var dto = new UserLoginDto { Email = "nobody@test.com", Password = "wrong" };
        var result = await _controller.Login(dto);
        Assert.That(result.Result, Is.TypeOf<UnauthorizedObjectResult>());
    }
}