using dataaccess.Entities;
using Service.Repositories;

namespace service.Services;

public class WinningBoardService : Service<WinningBoard>
{
    public WinningBoardService(IWinningboardRepository winningBoardRepository) : base(winningBoardRepository)
    {
    }
    
}