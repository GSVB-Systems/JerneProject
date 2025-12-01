using dataaccess.Entities;

namespace service.Repositories.Interfaces;

public interface IAuthRepository : IRepository<User>
{
    Task<User> getUserByEmailAsync(string email);
    
}