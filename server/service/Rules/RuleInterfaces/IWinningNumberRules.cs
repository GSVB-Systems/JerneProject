using Contracts.WinningNumberDTOs;
using Sieve.Models;

namespace service.Rules.RuleInterfaces
{
    public interface IWinningNumberRules
    {
        void ValidateGetById(string id);
        void ValidateGetAll(SieveModel? parameters);
        void ValidateCreate(CreateWinningNumberDto dto);
        void ValidateUpdate(string id, UpdateWinningNumberDto dto);
        void ValidateDelete(string id);
    }
}