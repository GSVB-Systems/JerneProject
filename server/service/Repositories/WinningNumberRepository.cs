using dataaccess;
using dataaccess.Entities;
using service.Repositories.Interfaces;

namespace service.Repositories;

public class WinningNumberRepository : Repository<WinningNumber>, IWinningNumberRepository
{
    public WinningNumberRepository(AppDbContext context) : base(context)
    {
    }
}