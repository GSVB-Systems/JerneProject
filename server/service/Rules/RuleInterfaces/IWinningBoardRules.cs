using Contracts.WinningBoardDTOs;
using Sieve.Models;

namespace service.Rules.RuleInterfaces
{
    public interface IWinningBoardRules
    {
        Task ValidateGetByIdAsync(string id);
        Task ValidateGetAllAsync(SieveModel? parameters);
        Task ValidateCreateAsync(CreateWinningBoardDto createDto);
        Task ValidateUpdateAsync(string id, UpdateWinningBoardDto updateDto);
        Task ValidateDeleteAsync(string id);
    }
}