using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// RecipesController exposes the RESTful endpoints used to manage recipe
// records. It supports full CRUD plus a filter endpoint so the dashboard
// can show only the recipes belonging to the selected chef.
[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RecipesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/recipes
    // Returns every recipe in the database. Used by the public Recipes page.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
    {
        return await _context.Recipes.ToListAsync();
    }

    // GET: api/recipes/{id}
    // Returns a single recipe by primary key.
    [HttpGet("{id}")]
    public async Task<ActionResult<Recipe>> GetRecipe(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);

        if (recipe == null)
        {
            return NotFound();
        }

        return recipe;
    }

    // GET: api/recipes/chef/{chefId}
    // Returns all recipes belonging to a specific chef. Used by the chef
    // profile page (to show "their" recipes) and by the dashboard recipe
    // management section.
    [HttpGet("chef/{chefId}")]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipesByChef(int chefId)
    {
        return await _context.Recipes
            .Where(r => r.ChefId == chefId)
            .ToListAsync();
    }

    // POST: api/recipes
    // Creates a new recipe. The ChefId field links it to the chef who
    // submitted it via the dashboard.
    [HttpPost]
    public async Task<ActionResult<Recipe>> CreateRecipe(Recipe recipe)
    {
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
    }

    // PUT: api/recipes/{id}
    // Updates an existing recipe.
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRecipe(int id, Recipe updatedRecipe)
    {
        if (id != updatedRecipe.Id)
        {
            return BadRequest("Route id and body id must match.");
        }

        _context.Entry(updatedRecipe).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Recipes.AnyAsync(r => r.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/recipes/{id}
    // Removes a recipe from the database.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);

        if (recipe == null)
        {
            return NotFound();
        }

        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
