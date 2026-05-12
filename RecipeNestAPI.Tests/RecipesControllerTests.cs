using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace RecipeNestAPI.Tests;

// Unit tests for RecipesController. Covers the chef-filter endpoint
// in particular, which is critical because the dashboard relies on
// it to show only the active chef's recipes.
public class RecipesControllerTests
{
    [Fact]
    public async Task GetRecipes_ReturnsAllSeededRecipes()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new RecipesController(db);

        var result = await controller.GetRecipes();

        var recipes = Assert.IsAssignableFrom<IEnumerable<Recipe>>(result.Value);
        Assert.Equal(3, recipes.Count());
    }

    [Fact]
    public async Task GetRecipesByChef_ReturnsOnlyThatChefsRecipes()
    {
        // Chef 1 has two recipes in the seed, chef 2 has one.
        using var db = TestDbContextFactory.Create();
        var controller = new RecipesController(db);

        var chef1Recipes = (await controller.GetRecipesByChef(1)).Value!.ToList();
        var chef2Recipes = (await controller.GetRecipesByChef(2)).Value!.ToList();

        Assert.Equal(2, chef1Recipes.Count);
        Assert.Single(chef2Recipes);
        Assert.All(chef1Recipes, r => Assert.Equal(1, r.ChefId));
        Assert.All(chef2Recipes, r => Assert.Equal(2, r.ChefId));
    }

    [Fact]
    public async Task GetRecipesByChef_WithUnknownChef_ReturnsEmptyList()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new RecipesController(db);

        var result = await controller.GetRecipesByChef(999);

        // We deliberately return an empty list (not a 404) so the
        // dashboard can render "no recipes yet" cleanly for new chefs.
        var recipes = Assert.IsAssignableFrom<IEnumerable<Recipe>>(result.Value);
        Assert.Empty(recipes);
    }

    [Fact]
    public async Task CreateRecipe_AddsToDatabase()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new RecipesController(db);
        var newRecipe = new Recipe
        {
            Title = "Tiramisu",
            Description = "Classic Italian dessert",
            Ingredients = "Mascarpone, espresso, ladyfingers, cocoa",
            Instructions = "Layer and chill",
            Category = "Dessert",
            ChefId = 1
        };

        var result = await controller.CreateRecipe(newRecipe);

        // 201 Created is the expected response and the new recipe
        // should be persisted with a generated id.
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var recipe = Assert.IsType<Recipe>(created.Value);
        Assert.True(recipe.Id > 0);
        Assert.Equal(4, db.Recipes.Count());
    }

    [Fact]
    public async Task UpdateRecipe_PersistsChanges()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new RecipesController(db);
        var existing = (await db.Recipes.FindAsync(1))!;
        existing.Title = "Renamed";

        var result = await controller.UpdateRecipe(1, existing);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal("Renamed", (await db.Recipes.FindAsync(1))!.Title);
    }

    [Fact]
    public async Task UpdateRecipe_WithMismatchedIds_ReturnsBadRequest()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new RecipesController(db);
        var recipe = new Recipe { Id = 2, Title = "X", ChefId = 1 };

        var result = await controller.UpdateRecipe(1, recipe);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteRecipe_RemovesFromDatabase()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new RecipesController(db);

        var result = await controller.DeleteRecipe(1);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(await db.Recipes.FindAsync(1));
        Assert.Equal(2, db.Recipes.Count());
    }
}
