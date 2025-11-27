using dataaccess.Entities;

namespace Service.Repositories;

public class WinningBoardRepository : Repository<WinningBoard>, IWinningboardRepository
{
    public WinningBoardRepository(AppContext context) : base(context)
    {
    }
    
}