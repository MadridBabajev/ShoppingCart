using System.ComponentModel.DataAnnotations;

namespace Public.DTO.v1.Identity;

/// <summary>
/// Register DTO that contains all the necessary data in order to become a user
/// </summary>
public class Register
{
    /// <summary>
    /// An email address the user is trying to register with.
    /// </summary>
    /// <remarks>
    /// The email address should be between 5 and 128 characters long.
    /// </remarks>
    [StringLength(128, MinimumLength = 5, ErrorMessage = "Incorrect length")]
    public string Email { get; set; } = default!;
    
    /// <summary>
    /// A password the user is trying to register with.
    /// </summary>
    /// <remarks>
    /// The password should be between 8 and 128 characters long.
    /// </remarks>
    [StringLength(128, MinimumLength = 8, ErrorMessage = "Incorrect length")]
    public string Password { get; set; } = default!;
    
    /// <summary>
    /// The password confirmation field.
    /// </summary>
    /// <remarks>
    /// The confirmation password should be between 8 and 128 characters long.
    /// </remarks>
    [StringLength(128, MinimumLength = 8, ErrorMessage = "Incorrect length")]
    public string ConfirmPassword { get; set; } = default!;
    
    /// <summary>
    /// The first name the user is trying to register with.
    /// </summary>
    /// <remarks>
    /// The first name should be between 1 and 128 characters long.
    /// </remarks>
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Incorrect length")]
    public string Firstname { get; set; } = default!;

    /// <summary>
    /// The last name the user is trying to register with.
    /// </summary>
    /// <remarks>
    /// The last name should be between 1 and 128 characters long.
    /// </remarks>
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Incorrect length")]
    public string Lastname { get; set; } = default!;
}