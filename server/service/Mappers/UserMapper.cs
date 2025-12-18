// csharp

using Contracts.UserDTOs;
using dataaccess.Entities;
using dataaccess.Entities.Enums;

namespace service.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(User u) =>
        u == null ? null : new UserDto
        {
            UserID = u.UserID,
            Firstname = u.Firstname,
            Lastname = u.Lastname,
            Email = u.Email,
            Role = u.Role,
            Firstlogin = u.Firstlogin,
            IsActive = u.IsActive,
            Balance = u.Balance,
            SubscriptionExpiresAtUtc = u.SubscriptionExpiresAt?.ToUniversalTime(),
            DaysUntilExpiry = u.SubscriptionExpiresAt.HasValue
                ? (int)Math.Max(0, Math.Ceiling((u.SubscriptionExpiresAt.Value - DateTime.UtcNow).TotalDays))
                : (int?)null
        };

    public static User ToEntity(RegisterUserDto dto) =>
        new User
        {
            UserID = Guid.NewGuid().ToString(),
            Firstname = dto.Firstname,
            Lastname = dto.Lastname,
            Email = dto.Email,
            Role = dto.Role.Equals("Administrator")? UserRole.Administrator: UserRole.Bruger
        };
    
    public static void ApplyUpdate(User target, UpdateUserDto dto)
    {
        if (dto == null) return;
        if (dto.Firstname != null) target.Firstname = dto.Firstname;
        if (dto.Lastname != null) target.Lastname = dto.Lastname;
        if (dto.Email != null) target.Email = dto.Email;
        if (dto.Role.HasValue) target.Role = dto.Role.Value;
        if (dto.IsActive.HasValue) target.IsActive = dto.IsActive.Value;
        if (dto.Balance.HasValue) target.Balance = dto.Balance.Value;
    }
}