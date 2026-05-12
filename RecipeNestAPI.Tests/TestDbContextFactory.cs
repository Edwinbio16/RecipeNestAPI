using Microsoft.EntityFrameworkCore;

namespace RecipeNestAPI.Tests;

// TestDbContextFactory builds an isolated, in-memory AppDbContext for
// each test. Every test gets its own database (named with a Guid)
// which means tests cannot interfere with each other and can run in
// any order safely. The factory also pre-seeds a small set of chefs
// and recipes that match the production seed shape so tests can
// exercise realistic queries.
public static class TestDbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var db = new AppDbContext(options);

        // Hash on demand using a low work factor (4) so the BCrypt
        // call costs almost nothing per test. The production code
        // uses the default cost of 11; cost only affects how slow
        // the verification is, not whether the hash is valid.
        var hash1 = BCrypt.Net.BCrypt.HashPassword("test_password_1", workFactor: 4);
        var hash2 = BCrypt.Net.BCrypt.HashPassword("test_password_2", workFactor: 4);

        db.Chefs.AddRange(
            new Chef
            {
                Id = 1,
                FullName = "Test Chef One",
                Email = "one@test.com",
                Password = hash1,
                Bio = "Bio one",
                ProfileImageUrl = "https://example.com/one.jpg",
                Specialty = "Italian"
            },
            new Chef
            {
                Id = 2,
                FullName = "Test Chef Two",
                Email = "two@test.com",
                Password = hash2,
                Bio = "Bio two",
                ProfileImageUrl = "https://example.com/two.jpg",
                Specialty = "Japanese"
            }
        );

        db.Recipes.AddRange(
            new Recipe
            {
                Id = 1, Title = "Pasta", Description = "Classic pasta",
                Ingredients = "Pasta, sauce", Instructions = "Cook",
                Category = "Dinner", ChefId = 1
            },
            new Recipe
            {
                Id = 2, Title = "Pizza", Description = "Margherita",
                Ingredients = "Dough, sauce", Instructions = "Bake",
                Category = "Main", ChefId = 1
            },
            new Recipe
            {
                Id = 3, Title = "Ramen", Description = "Tonkotsu",
                Ingredients = "Broth, noodles", Instructions = "Simmer",
                Category = "Dinner", ChefId = 2
            }
        );

        db.SaveChanges();
        return db;
    }
}
