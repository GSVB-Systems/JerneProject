using dataaccess.Entities;
using Service.Repositories;

namespace service.Services;

public interface IUserService : IService<User>
{
    // no extra members - CRUD from base IService
}

public class UserService : Service<User>, IUserService
{
    public UserService(IUserRepository userRepository) : base(userRepository)
    {
    }
}