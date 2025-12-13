using dataaccess;
using dataaccess.Entities;
using service.Repositories.Interfaces;

namespace service.Repositories;



public class BoardRepository : Repository<Board>, IBoardRepository
{

    public BoardRepository(AppDbContext context) : base(context)
    {
    }
}