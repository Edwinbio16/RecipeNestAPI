using Microsoft.EntityFrameworkCore;

// AppDbContext is the Entity Framework Core gateway to the SQLite
// database. It configures the Chef -> Recipe relationship and seeds
// the database with realistic sample data on first run.
//
// Important note about the seeded passwords: the values below are
// pre-computed BCrypt hashes (cost 11). The plaintext credentials
// for development and demo purposes are documented in README.md.
// The hashes were generated externally so that EF Core's seed
// mechanism (which runs at model-building time) can use a stable
// constant - regenerating hashes on every startup would defeat
// EF's change detection.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Chef> Chefs { get; set; }
    public DbSet<Recipe> Recipes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // One chef -> many recipes. Cascade delete keeps the database
        // free of orphaned recipes whenever a chef is removed.
        modelBuilder.Entity<Recipe>()
            .HasOne<Chef>()
            .WithMany()
            .HasForeignKey(r => r.ChefId)
            .OnDelete(DeleteBehavior.Cascade);

        // A unique index on Email prevents two chefs from registering
        // with the same address, which is essential because the login
        // endpoint looks up chefs by email.
        modelBuilder.Entity<Chef>()
            .HasIndex(c => c.Email)
            .IsUnique();

        // --- Seed data ----------------------------------------------------
        // BCrypt hashes below correspond to:
        //   marco@recipenest.com  -> "marco_pass_2026"
        //   amina@recipenest.com  -> "amina_pass_2026"
        //   hiro@recipenest.com   -> "hiro_pass_2026"
        modelBuilder.Entity<Chef>().HasData(
            new Chef
            {
                Id = 1,
                FullName = "Chef Marco Rossi",
                Email = "marco@recipenest.com",
                Password = "$2b$11$5fKScmySqz48SGaoMD.ShuNbTML7LylcxRqYvbNlL3/rEzLBcxtoS",
                Bio = "Italian chef with over 15 years of experience specialising in handmade pasta, regional Italian classics, and Mediterranean flavours.",
                ProfileImageUrl = "https://i.pravatar.cc/400?img=68",
                Specialty = "Italian Cuisine"
            },
            new Chef
            {
                Id = 2,
                FullName = "Chef Amina Yusuf",
                Email = "amina@recipenest.com",
                Password = "$2b$11$1eUHW9F2dirb/li5SUJThOqy1MF6Au4vmc.t4muzIzt0.Iv8gCJrm",
                Bio = "Creative chef focusing on healthy, modern African recipes that reimagine traditional dishes for the contemporary kitchen.",
                ProfileImageUrl = "https://i.pravatar.cc/400?img=49",
                Specialty = "Healthy African Fusion"
            },
            new Chef
            {
                Id = 3,
                FullName = "Chef Hiro Tanaka",
                Email = "hiro@recipenest.com",
                Password = "$2b$11$CE0BrOUXXp5Tj9T5W8jkoOniOkesniBrQy1CHB0.k8DFB8DpVbwV2",
                Bio = "Tokyo-trained chef passionate about Japanese home cooking, ramen, and the principles of seasonal eating.",
                ProfileImageUrl = "https://i.pravatar.cc/400?img=12",
                Specialty = "Japanese Cuisine"
            }
        );

        modelBuilder.Entity<Recipe>().HasData(
            new Recipe
            {
                Id = 1,
                Title = "Creamy Chicken Pasta",
                Description = "A rich and creamy pasta dish perfect for a quick weeknight dinner.",
                Ingredients = "Chicken breast, tagliatelle, double cream, garlic, parmesan, parsley",
                Instructions = "Boil pasta until al dente. Pan-fry seasoned chicken until golden. Add garlic, then cream and parmesan. Combine with pasta and finish with parsley.",
                Category = "Dinner",
                ChefId = 1
            },
            new Recipe
            {
                Id = 2,
                Title = "Classic Margherita Pizza",
                Description = "A simple Neapolitan pizza with tomato, mozzarella, and fresh basil.",
                Ingredients = "Pizza dough, San Marzano tomatoes, fresh mozzarella, basil, olive oil, salt",
                Instructions = "Stretch the dough. Spread crushed tomatoes, top with mozzarella. Bake at the highest oven setting until the crust is charred. Finish with basil and olive oil.",
                Category = "Main Course",
                ChefId = 1
            },
            new Recipe
            {
                Id = 3,
                Title = "Spicy Jollof Rice",
                Description = "A flavourful West African rice dish cooked in a rich tomato and pepper sauce.",
                Ingredients = "Long-grain rice, tomatoes, scotch bonnet, red bell pepper, onion, stock, bay leaves",
                Instructions = "Blend tomatoes, peppers and onion. Simmer the sauce with spices. Add washed rice and stock, cover, and cook on low until tender.",
                Category = "Main Course",
                ChefId = 2
            },
            new Recipe
            {
                Id = 4,
                Title = "Suya-Spiced Grilled Chicken",
                Description = "Smoky, peppery grilled chicken inspired by Nigerian street food.",
                Ingredients = "Chicken thighs, suya spice, peanut powder, paprika, ginger, garlic, vegetable oil",
                Instructions = "Coat chicken in spice rub and oil. Marinate for an hour. Grill over high heat until charred and cooked through. Serve with sliced onion and tomato.",
                Category = "Dinner",
                ChefId = 2
            },
            new Recipe
            {
                Id = 5,
                Title = "Tonkotsu Ramen",
                Description = "A rich, milky pork bone broth ramen finished with chashu pork and a soft-boiled egg.",
                Ingredients = "Pork bones, ramen noodles, chashu pork, soy tare, soft-boiled egg, spring onion, nori",
                Instructions = "Simmer pork bones for several hours until the broth turns milky. Cook noodles, assemble in a bowl with tare and broth, and top with chashu, egg, and garnishes.",
                Category = "Dinner",
                ChefId = 3
            }
        );
    }
}
