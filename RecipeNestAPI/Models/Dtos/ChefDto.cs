namespace RecipeNestAPI.Models.Dtos;

// ChefDto is the public, password-free shape of a Chef sent back to
// the frontend. Returning this from the controllers (instead of the
// raw Chef entity) means a hashed password can never accidentally
// be serialised into a JSON response and exposed to a client.
public class ChefDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;

    // Convenience factory used by the controllers to project a Chef
    // entity into its DTO form without repeating the mapping in every
    // endpoint.
    public static ChefDto FromChef(Chef chef) => new()
    {
        Id = chef.Id,
        FullName = chef.FullName,
        Email = chef.Email,
        Bio = chef.Bio,
        ProfileImageUrl = chef.ProfileImageUrl,
        Specialty = chef.Specialty
    };
}
