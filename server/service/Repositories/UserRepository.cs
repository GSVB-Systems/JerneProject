using dataaccess;
using dataaccess.Entities;
namespace Service.Repositories;



public class UserRepository : Repository<User>, IUser
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
}