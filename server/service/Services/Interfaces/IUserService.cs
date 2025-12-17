using Contracts.UserDTOs;
using Contracts;
using dataaccess.Entities;

namespace service.Services.Interfaces;

public interface IUserService : IService<UserDto, RegisterUserDto, UpdateUserDto>
{
    
    Task<bool> VerifyUserPasswordAsync(string userId, string plainPassword);
    Task<bool> IsSubscriptionActiveAsync(string userId);
    Task<UserDto?> ExtendSubscriptionAsync(string userId, int months);
    Task<UserDto?> UpdateBalanceAsync(string userId);
}