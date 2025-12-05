using Contracts.UserDTOs;

namespace service.Services.Interfaces;

public interface IUserService : IService<UserDto>
{
    Task<UserDto> RegisterUserAsync(RegisterUserDto dto);
    Task<bool> VerifyUserPasswordAsync(string userId, string plainPassword);
}