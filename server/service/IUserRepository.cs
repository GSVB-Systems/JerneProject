using dataaccess;
using dataaccess.Entities;
namespace Service;

public interface IUserRepository : IRepository<User>
{
    
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
}