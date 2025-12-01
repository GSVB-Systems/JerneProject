using dataaccess;
using dataaccess.Entities;
namespace Service.Repositories;



public class BoardRepository : Repository<Board>, IBoardRepository
{
    public BoardRepository(AppDbContext context) : base(context)
    {
    }
}