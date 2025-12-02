using dataaccess.Entities.Enums;

namespace Contracts.UserDTOs;

public class UpdateUserDto
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public UserRole? Role { get; set; }
    public bool? IsActive { get; set; }
    public decimal? Balance { get; set; }
}