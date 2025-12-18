using System.Threading.Tasks;
using Contracts.UserDTOs;

namespace service.Rules.RuleInterfaces
{
    public interface IUserRules
    {
        Task ValidateCreateAsync(RegisterUserDto createDto);
        Task ValidateUpdateAsync(string id, UpdateUserDto updateDto);
        Task ValidateDeleteAsync(string id);
    }
}