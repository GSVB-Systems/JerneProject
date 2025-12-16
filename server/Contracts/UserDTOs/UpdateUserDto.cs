using System.ComponentModel.DataAnnotations;
using dataaccess.Entities.Enums;

namespace Contracts.UserDTOs;

public class UpdateUserDto
{
    [StringLength(100)]
    public string Firstname { get; set; }

    [StringLength(100)]
    public string Lastname { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public UserRole? Role { get; set; }
    public bool? IsActive { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? Balance { get; set; }
}