using dataaccess.Entities;

namespace Service.Repositories;

public class WinningBoardRepository : Repository<WinningBoard>, IWinningboard
{
    public WinningBoardRepository(AppContext context) : base(context)
    {
    }
    
}