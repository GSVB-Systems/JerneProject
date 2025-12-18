using System.Threading.Tasks;
using Contracts.UserDTOs;
using Sieve.Models;

namespace service.Rules.RuleInterfaces
{
    public interface IUserRules
    {
        Task ValidateCreateAsync(RegisterUserDto createDto);
        Task ValidateUpdateAsync(string id, UpdateUserDto updateDto);
        Task ValidateDeleteAsync(string id);
        Task ValidateUpdateBalanceAsync(string id);
        Task ValidateExtendSubscriptionAsync(string id, int months);
        Task ValidateIsSubscriptionActiveAsync(string id);
        Task ValidateGetByIdAsync(string id);
        Task ValidateGetAllAsync(SieveModel? parameters);
    }
}