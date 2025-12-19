using System.Threading.Tasks;
using Contracts.BoardNumberDTOs;
using Sieve.Models;

namespace service.Rules.RuleInterfaces
{
    public interface IBoardNumberRules
    {
        Task ValidateCreateAsync(CreateBoardNumberDto createDto);
        Task ValidateUpdateAsync(string id, UpdateBoardNumberDto updateDto);
        Task ValidateDeleteAsync(string id);
        Task ValidateGetByIdAsync(string id);
        Task ValidateGetAllAsync(SieveModel? parameters);
        
    }
}