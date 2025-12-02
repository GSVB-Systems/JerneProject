// csharp

using Contracts.UserDTOs;
using dataaccess.Entities;

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
            Balance = u.Balance
        };

    public static User ToEntity(RegisterUserDto dto) =>
        new User
        {
            UserID = Guid.NewGuid().ToString(),
            Firstname = dto.Firstname,
            Lastname = dto.Lastname,
            Email = dto.Email,
        };

    public static void ApplyUpdate(User target, UpdateUserDto dto)
    {
        if (dto.Firstname != null) target.Firstname = dto.Firstname;
        if (dto.Lastname != null) target.Lastname = dto.Lastname;
        if (dto.Email != null) target.Email = dto.Email;
        if (dto.Role.HasValue) target.Role = dto.Role.Value;
        if (dto.IsActive.HasValue) target.IsActive = dto.IsActive.Value;
        if (dto.Balance.HasValue) target.Balance = dto.Balance.Value;
    }
}