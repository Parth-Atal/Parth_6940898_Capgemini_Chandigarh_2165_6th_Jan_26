using EcommerceAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace EcommerceAPI.Tests;

public class AuthTests
{
    private static AuthController CreateController()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"]      = "THIS_IS_A_SECURE_KEY_1234567890123456",
                ["Jwt:Issuer"]   = "EcommerceAPI",
                ["Jwt:Audience"] = "EcommerceUsers"
            })
            .Build();

        return new AuthController(config);
    }

    [Fact]
    public void Login_ReturnsOk_WithToken()
    {
        var controller = CreateController();

        var result = controller.Login("admin");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);

        // Verify the response has a token property
        var json = System.Text.Json.JsonSerializer.Serialize(ok.Value);
        Assert.Contains("token", json);
    }

    [Fact]
    public void Login_ReturnsBadRequest_WhenUsernameEmpty()
    {
        var controller = CreateController();

        var result = controller.Login("");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Login_ReturnsBadRequest_WhenUsernameWhitespace()
    {
        var controller = CreateController();

        var result = controller.Login("   ");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Login_TokenIsNonEmpty()
    {
        var controller = CreateController();

        var result = controller.Login("testuser");

        var ok = Assert.IsType<OkObjectResult>(result);

        // Serialize and check token is not empty
        var json = System.Text.Json.JsonSerializer.Serialize(ok.Value);
        Assert.DoesNotContain("\"token\":\"\"", json);
        Assert.DoesNotContain("\"token\":null", json);
    }
}
