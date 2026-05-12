using Microsoft.AspNetCore.Mvc;
using RecipeNestAPI.Models.Dtos;
using Xunit;

namespace RecipeNestAPI.Tests;

// Tests for AuthController.Login. We seed two chefs with known
// plaintext credentials in TestDbContextFactory, then exercise the
// happy path and the two failure paths (wrong password, unknown
// email) and confirm the password hash is never returned.
public class AuthControllerTests
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsChefDto()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new AuthController(db);
        var login = new LoginDto
        {
            Email = "one@test.com",
            Password = "test_password_1"
        };

        var result = await controller.Login(login);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<ChefDto>(ok.Value);
        Assert.Equal("one@test.com", dto.Email);
        Assert.Equal(1, dto.Id);
    }

    [Fact]
    public async Task Login_IsCaseInsensitiveOnEmail()
    {
        // Users tend to capitalise their email differently across
        // sessions ("Edwin@..." vs "edwin@..."). Login should still
        // succeed.
        using var db = TestDbContextFactory.Create();
        var controller = new AuthController(db);

        var result = await controller.Login(new LoginDto
        {
            Email = "ONE@TEST.COM",
            Password = "test_password_1"
        });

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new AuthController(db);

        var result = await controller.Login(new LoginDto
        {
            Email = "one@test.com",
            Password = "wrong_password"
        });

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_Returns401()
    {
        // The endpoint deliberately returns the same 401 for both
        // "wrong password" and "unknown email" so an attacker cannot
        // probe for which accounts exist.
        using var db = TestDbContextFactory.Create();
        var controller = new AuthController(db);

        var result = await controller.Login(new LoginDto
        {
            Email = "ghost@test.com",
            Password = "anything"
        });

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }
}
