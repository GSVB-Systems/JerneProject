using System.Threading.Tasks;
using Contracts.BoardNumberDTOs;

namespace service.Rules.RuleInterfaces
{
    public interface IBoardNumberRules
    {
        Task ValidateCreateAsync(CreateBoardNumberDto createDto);
        Task ValidateUpdateAsync(string id, UpdateBoardNumberDto updateDto);
        Task ValidateDeleteAsync(string id);
    }
}