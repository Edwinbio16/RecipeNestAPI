using System.ComponentModel.DataAnnotations;

// Chef represents a registered chef on the platform. The validation
// attributes are enforced automatically by the [ApiController]
// attribute on the controllers, so any invalid request body returns
// a 400 with a structured error list before the controller method
// even runs.
public class Chef
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Full name must be between 2 and 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not in a valid format.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    // The Password column stores a BCrypt hash, never the plaintext.
    // The controller is responsible for hashing the value supplied by
    // the client before saving. The hash itself is around 60 chars.
    [Required]
    [StringLength(200)]
    public string Password { get; set; } = string.Empty;

    [StringLength(1000,
        ErrorMessage = "Bio must be 1000 characters or fewer.")]
    public string Bio { get; set; } = string.Empty;

    [Url(ErrorMessage = "Profile image URL must be a valid URL.")]
    [StringLength(500)]
    public string ProfileImageUrl { get; set; } = string.Empty;

    [StringLength(100)]
    public string Specialty { get; set; } = string.Empty;
}
