using dataaccess;
using dataaccess.Entities;
namespace Service.Repositories;



public class UserRepositoryRepository : Repository<User>, IUserRepository
{
    public UserRepositoryRepository(AppDbContext context) : base(context)
    {
    }
}