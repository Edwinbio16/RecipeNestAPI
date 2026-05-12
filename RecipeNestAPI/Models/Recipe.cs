using System.ComponentModel.DataAnnotations;

// Recipe represents a single recipe authored by a chef. The validation
// rules below are evaluated automatically because the controllers are
// decorated with [ApiController].
public class Recipe
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(150, MinimumLength = 2,
        ErrorMessage = "Title must be between 2 and 150 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500,
        ErrorMessage = "Description must be 500 characters or fewer.")]
    public string Description { get; set; } = string.Empty;

    [StringLength(2000,
        ErrorMessage = "Ingredients must be 2000 characters or fewer.")]
    public string Ingredients { get; set; } = string.Empty;

    [StringLength(5000,
        ErrorMessage = "Instructions must be 5000 characters or fewer.")]
    public string Instructions { get; set; } = string.Empty;

    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [Range(1, int.MaxValue,
        ErrorMessage = "ChefId must reference a valid chef.")]
    public int ChefId { get; set; }
}
