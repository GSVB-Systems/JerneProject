using dataaccess;
using dataaccess.Entities;
namespace Service.Repositories;



public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
}