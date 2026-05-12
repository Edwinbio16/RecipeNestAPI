using System.ComponentModel.DataAnnotations;

namespace RecipeNestAPI.Models.Dtos;

// LoginDto is the request body for POST /api/auth/login. Keeping
// authentication input in its own DTO means we never accidentally
// bind extra Chef fields (like an attacker-supplied Bio) when all
// we want is the email and password.
public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Password { get; set; } = string.Empty;
}
