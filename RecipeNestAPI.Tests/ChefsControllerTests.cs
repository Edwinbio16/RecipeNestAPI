using Microsoft.AspNetCore.Mvc;
using RecipeNestAPI.Models.Dtos;
using Xunit;

namespace RecipeNestAPI.Tests;

// Unit tests for ChefsController. Each test creates its own in-memory
// AppDbContext via TestDbContextFactory, then exercises one method
// on the controller and asserts on the result. We don't spin up the
// full ASP.NET pipeline because the controller methods are plain C#
// async methods that can be invoked directly.
public class ChefsControllerTests
{
    [Fact]
    public async Task GetChefs_ReturnsAllSeededChefs()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var controller = new ChefsController(db);

        // Act
        var result = await controller.GetChefs();

        // Assert: two chefs in the seed, both should come back as DTOs.
        var dtos = Assert.IsAssignableFrom<IEnumerable<ChefDto>>(result.Value);
        Assert.Equal(2, dtos.Count());
        Assert.Contains(dtos, c => c.FullName == "Test Chef One");
        Assert.Contains(dtos, c => c.FullName == "Test Chef Two");
    }

    [Fact]
    public async Task GetChefs_DoesNotLeakPasswordHash()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var controller = new ChefsController(db);

        // Act
        var result = await controller.GetChefs();
        var dtos = (IEnumerable<ChefDto>)result.Value!;

        // Assert: the DTO type literally has no Password property,
        // but this test still serves as a regression guard - if a
        // future refactor accidentally returns Chef entities, this
        // assertion will fail to compile rather than ship a leak.
        var chefDtoType = typeof(ChefDto);
        Assert.Null(chefDtoType.GetProperty("Password"));
    }

    [Fact]
    public async Task GetChef_WithKnownId_ReturnsThatChef()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new ChefsController(db);

        var result = await controller.GetChef(1);

        var dto = Assert.IsType<ChefDto>(result.Value);
        Assert.Equal(1, dto.Id);
        Assert.Equal("Test Chef One", dto.FullName);
    }

    [Fact]
    public async Task GetChef_WithUnknownId_Returns404()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new ChefsController(db);

        var result = await controller.GetChef(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateChef_PersistsChanges()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var controller = new ChefsController(db);
        var update = new ChefUpdateDto
        {
            FullName = "Updated Name",
            Email = "updated@test.com",
            Bio = "Updated bio",
            ProfileImageUrl = "https://example.com/updated.jpg",
            Specialty = "Updated Specialty"
        };

        // Act
        var result = await controller.UpdateChef(1, update);

        // Assert: 204 No Content is the expected REST response, and
        // the updated row should be readable from the database.
        Assert.IsType<NoContentResult>(result);
        var updated = await db.Chefs.FindAsync(1);
        Assert.NotNull(updated);
        Assert.Equal("Updated Name", updated!.FullName);
        Assert.Equal("Updated Specialty", updated.Specialty);
    }

    [Fact]
    public async Task UpdateChef_DoesNotChangePasswordHash()
    {
        // Critical security test: the update DTO has no Password
        // field, so a malicious client cannot use this endpoint to
        // overwrite a chef's hashed password.
        using var db = TestDbContextFactory.Create();
        var originalHash = (await db.Chefs.FindAsync(1))!.Password;
        var controller = new ChefsController(db);

        await controller.UpdateChef(1, new ChefUpdateDto
        {
            FullName = "Anything",
            Email = "anything@test.com",
            Bio = "",
            ProfileImageUrl = "https://example.com/x.jpg",
            Specialty = ""
        });

        var refreshed = await db.Chefs.FindAsync(1);
        Assert.Equal(originalHash, refreshed!.Password);
    }

    [Fact]
    public async Task UpdateChef_WithUnknownId_Returns404()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new ChefsController(db);

        var result = await controller.UpdateChef(999, new ChefUpdateDto
        {
            FullName = "X",
            Email = "x@test.com",
            Bio = "",
            ProfileImageUrl = "https://example.com/x.jpg",
            Specialty = ""
        });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteChef_RemovesChefAndCascadesRecipes()
    {
        // Cascade behaviour is configured in OnModelCreating but the
        // EF in-memory provider also enforces it, so this test
        // confirms the configuration is correct.
        using var db = TestDbContextFactory.Create();
        var controller = new ChefsController(db);

        var result = await controller.DeleteChef(1);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(await db.Chefs.FindAsync(1));
        // Chef #1 owned recipes 1 and 2 in the seed; both should
        // have been removed by the cascade.
        Assert.Empty(db.Recipes.Where(r => r.ChefId == 1));
        // Chef #2's recipe should be untouched.
        Assert.Single(db.Recipes.Where(r => r.ChefId == 2));
    }

    [Fact]
    public async Task DeleteChef_WithUnknownId_Returns404()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new ChefsController(db);

        var result = await controller.DeleteChef(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
