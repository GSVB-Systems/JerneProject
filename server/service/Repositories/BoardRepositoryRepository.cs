using dataaccess;
using dataaccess.Entities;
namespace Service.Repositories;



public class BoardRepositoryRepository : Repository<Board>, IBoardRepository
{
    public BoardRepositoryRepository(AppDbContext context) : base(context)
    {
    }
}