using System.ComponentModel.DataAnnotations;

namespace RecipeNestAPI.Models.Dtos;

// ChefUpdateDto is the request body accepted by PUT /api/chefs/{id}.
// It deliberately excludes the password and id fields, which means
// a logged-in chef can update their bio or photo from the dashboard
// without being able to overwrite their own (hashed) password by
// mistake or change someone else's id. Password changes go through
// a separate, dedicated endpoint if/when that feature is added.
public class ChefUpdateDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Bio { get; set; } = string.Empty;

    [Url]
    [StringLength(500)]
    public string ProfileImageUrl { get; set; } = string.Empty;

    [StringLength(100)]
    public string Specialty { get; set; } = string.Empty;
}
