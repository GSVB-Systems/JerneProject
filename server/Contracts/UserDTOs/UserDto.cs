using dataaccess.Entities.Enums;

namespace Contracts.UserDTOs;

public class UserDto
{
    public string UserID { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
    public bool Firstlogin { get; set; }
    public bool IsActive { get; set; }
    public decimal Balance { get; set; }
    public DateTime? SubscriptionExpiresAtUtc { get; set; }
    public int? DaysUntilExpiry { get; set; }
}