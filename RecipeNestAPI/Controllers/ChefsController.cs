using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeNestAPI.Models.Dtos;

// ChefsController exposes the RESTful endpoints used to manage chef
// records. All responses use ChefDto rather than the raw Chef entity
// so the BCrypt password hash is never serialised to the client.
[ApiController]
[Route("api/[controller]")]
public class ChefsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ChefsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/chefs
    // Returns every chef as a list of ChefDto. Used by the Chefs List
    // page and by any dropdown that needs the catalogue of chefs.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChefDto>>> GetChefs()
    {
        var chefs = await _context.Chefs.ToListAsync();
        return chefs.Select(ChefDto.FromChef).ToList();
    }

    // GET: api/chefs/{id}
    // Returns a single chef as ChefDto. Used by the public profile
    // page and by the dashboard when loading the profile form.
    [HttpGet("{id}")]
    public async Task<ActionResult<ChefDto>> GetChef(int id)
    {
        var chef = await _context.Chefs.FindAsync(id);

        if (chef == null)
        {
            return NotFound();
        }

        return ChefDto.FromChef(chef);
    }

    // PUT: api/chefs/{id}
    // Updates an existing chef from the dashboard. Accepts a
    // ChefUpdateDto rather than a Chef entity, which means the
    // request body cannot overwrite the password hash or the id -
    // both are fields that the frontend has no business sending.
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateChef(int id, [FromBody] ChefUpdateDto update)
    {
        var chef = await _context.Chefs.FindAsync(id);

        if (chef == null)
        {
            return NotFound();
        }

        // Copy whitelisted fields from the DTO onto the entity. Doing
        // this manually (instead of using AutoMapper) keeps the
        // mapping completely visible and easy to audit.
        chef.FullName = update.FullName;
        chef.Email = update.Email;
        chef.Bio = update.Bio;
        chef.ProfileImageUrl = update.ProfileImageUrl;
        chef.Specialty = update.Specialty;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Returns 404 if the record was deleted between the read
            // and the write, which is the standard REST behaviour.
            if (!await _context.Chefs.AnyAsync(c => c.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/chefs/{id}
    // Removes a chef record. Recipes belonging to this chef are also
    // removed because cascade delete is configured in AppDbContext.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChef(int id)
    {
        var chef = await _context.Chefs.FindAsync(id);

        if (chef == null)
        {
            return NotFound();
        }

        _context.Chefs.Remove(chef);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
