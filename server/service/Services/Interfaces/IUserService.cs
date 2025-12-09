using Contracts.UserDTOs;
using Contracts;

namespace service.Services.Interfaces;

public interface IUserService : IService<UserDto>
{
    Task<PagedResult<UserDto>> GetAllAsync(UserQueryParameters? parameters);
    Task<UserDto> RegisterUserAsync(RegisterUserDto dto);
    Task<bool> VerifyUserPasswordAsync(string userId, string plainPassword);
    Task<bool> IsSubscriptionActiveAsync(string userId);
    Task<UserDto?> ExtendSubscriptionAsync(string userId, int months);

}