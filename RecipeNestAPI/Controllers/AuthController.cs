using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeNestAPI.Models.Dtos;

// AuthController owns the login flow. There is intentionally no
// /register endpoint: chefs are created via the seed data so the
// dashboard demo always has known credentials. A password-change
// endpoint could live here in a future iteration.
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/auth/login
    // Validates the supplied email and password against the database.
    // The plaintext password is verified with BCrypt.Verify against
    // the stored hash; we never compare passwords directly. On
    // success the chef record is returned in DTO form (without the
    // hash) so the frontend can populate its auth context.
    [HttpPost("login")]
    public async Task<ActionResult<ChefDto>> Login([FromBody] LoginDto login)
    {
        // Looks up the chef by email. We use a case-insensitive match
        // because the user might capitalise their email differently
        // each time they log in.
        var chef = await _context.Chefs
            .FirstOrDefaultAsync(c => c.Email.ToLower() == login.Email.ToLower());

        // Constant-time comparison via BCrypt.Verify is important here:
        // a naive == on the password would leak information about which
        // accounts exist. We also return the same generic 401 whether
        // the email is unknown or the password is wrong.
        if (chef == null ||
            !BCrypt.Net.BCrypt.Verify(login.Password, chef.Password))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(ChefDto.FromChef(chef));
    }
}
