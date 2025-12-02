using System.Security.Claims;
using Contracts.UserDTOs;
using dataaccess.Entities;

namespace service.Services.Interfaces;

public interface IAuthService : IService<User>
{
    Task<bool> verifyPasswordByEmailAsync(string email, string plainPassword);
    Task<User> GetUserByEmailAsync(string email);
    
}