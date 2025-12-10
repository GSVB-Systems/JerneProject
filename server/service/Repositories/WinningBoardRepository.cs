using dataaccess;
using dataaccess.Entities;
using service.Repositories.Interfaces;

namespace service.Repositories;

public class WinningBoardRepository : Repository<WinningBoard>, IWinningboardRepository
{
    public WinningBoardRepository(AppDbContext context) : base(context)
    {
    }
    
}