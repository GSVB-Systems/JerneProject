using dataaccess.Entities;

namespace service.Services.Interfaces;

public interface IUserService : IService<User>
{    
    Task<User> RegisterUserAsync(User user, string plainPassword);
    Task<bool> VerifyUserPasswordAsync(string userId, string plainPassword);
}