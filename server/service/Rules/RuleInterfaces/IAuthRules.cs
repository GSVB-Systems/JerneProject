using System.Threading.Tasks;
using Contracts.UserDTOs;
using dataaccess.Entities;
using Sieve.Models;

namespace service.Rules.RuleInterfaces;

public interface IAuthRules
{
    Task ValidateGetByIdAsync(string id);
    Task ValidateGetAllAsync(SieveModel? parameters);
    Task ValidateCreateAsync(User entity);
    Task ValidateUpdateAsync(string id, User entity);
    Task ValidateDeleteAsync(string id);

    Task ValidateVerifyPasswordByEmailAsync(string email, string plainPassword);
    Task ValidateGetUserByEmailAsync(string email);
    Task ValidateUpdateUserPasswordAsync(string userId, string oldPassword, string newPassword);
    Task ValidateAdminResetUserPasswordAsync(string userId, string newPassword);
}
