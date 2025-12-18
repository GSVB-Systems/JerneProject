using System.ComponentModel.DataAnnotations;
using Contracts.Validation;

namespace Contracts.UserDTOs;

public class RegisterUserDto
{
    [Required]
    [StringLength(100)]
    public string Firstname { get; set; }

    [Required]
    [StringLength(100)]
    public string Lastname { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [PasswordComplexity(8)]
    public string Password { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Role { get; set; }
}