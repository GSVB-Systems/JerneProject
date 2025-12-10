using dataaccess;
using dataaccess.Entities;
using service.Repositories.Interfaces;

namespace service.Repositories;



public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
}