using Contracts.UserDTOs;

namespace service.Services.Interfaces;

public interface IUserService : IService<UserDto>
{
    Task<IEnumerable<UserDto>> GetAllAsync(UserQueryParameters? parameters);
    Task<UserDto> RegisterUserAsync(RegisterUserDto dto);
    Task<bool> VerifyUserPasswordAsync(string userId, string plainPassword);
}