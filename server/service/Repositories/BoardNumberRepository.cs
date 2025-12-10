using dataaccess;
using dataaccess.Entities;
using service.Repositories.Interfaces;

namespace service.Repositories;

public class BoardNumberRepository : Repository<BoardNumber>, IBoardNumberRepository
{
    public BoardNumberRepository(AppDbContext context) : base(context)
    {
    }
}