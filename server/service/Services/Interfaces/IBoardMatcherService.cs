using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.WinnerResultDTO;

namespace service.Services.Interfaces
{
    public interface IBoardMatcherService
    {
        Task<List<WinnerResultDto>> GetBoardsContainingNumbersAsync(string winningBoardId);
    }
}