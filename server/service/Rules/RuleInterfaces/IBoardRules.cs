using Contracts.BoardDTOs;
using Sieve.Models;

namespace service.Rules
{
    public interface IBoardRules
    {
        Task ValidateGetByIdAsync(string? id);
        Task ValidateGetAllAsync(SieveModel? parameters);
        Task ValidateCreateAsync(CreateBoardDto createDto);
        Task ValidateUpdateAsync(string? id, UpdateBoardDto? updateDto);
        Task ValidateDeleteAsync(string? id);
        Task ValidateGetAllByUserIdAsync(string? userId, BoardQueryParameters? parameters);

    }
}