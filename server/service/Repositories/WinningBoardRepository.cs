using dataaccess;
using dataaccess.Entities;

namespace Service.Repositories;

public class WinningBoardRepository : Repository<WinningBoard>, IWinningboardRepository
{
    public WinningBoardRepository(AppDbContext context) : base(context)
    {
    }
    
}