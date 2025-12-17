using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Contracts.UserDTOs;
using dataaccess.Entities;
using service.Mappers;
using service.Repositories.Interfaces;
using service.Services.Interfaces;
using Sieve.Models;
using System.ComponentModel.DataAnnotations; // for ValidationContext
using Contracts.Validation; // for PasswordComplexityAttribute

namespace service.Services;

public class AuthService : Service<User, User, User>, IAuthService
{
    protected readonly IAuthRepository _authRepository;
    protected readonly PasswordService _passwordService;

    public AuthService(IAuthRepository authRepository, PasswordService passwordService)
        : base(authRepository)
    {
        _authRepository = authRepository;
        _passwordService = passwordService;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await base.GetByIdAsync(id);
    }

    public async Task<PagedResult<UserDto>> GetAllAsync(SieveModel? parameters = null)
    {
        var result = await base.GetAllAsync(parameters);
        return new PagedResult<UserDto>
        {
            Items = result.Items.Select(UserMapper.ToDto).ToList(),
            TotalCount = result.TotalCount
        };
    }

    public Task<User> CreateAsync(User entity)
    {
        return base.CreateAsync(entity);
    }

    public Task<User?> UpdateAsync(string id, User entity)
    {
        return base.UpdateAsync(id, entity);
    }

    public Task<bool> DeleteAsync(string id)
    {
        return base.DeleteAsync(id);
    }

    public async Task<bool> verifyPasswordByEmailAsync(string email, string plainPassword)
    {
        var user = await GetUserByEmailAsync(email);
        if (user == null) return false;

        return _passwordService.VerifyPassword(plainPassword, user.Hash);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _authRepository.getUserByEmailAsync(email);
    }

    private static void EnsurePasswordComplexity(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("New password is required.");

        var attribute = new PasswordComplexityAttribute();
        var result = attribute.GetValidationResult(password, new ValidationContext(new object()));
        if (result != ValidationResult.Success)
            throw new InvalidOperationException(result?.ErrorMessage ?? "Password does not meet complexity requirements.");
    }

    public async Task<User> UpdateUserPasswordAsync(string userId, string oldPassword, string newPassword)
    {
        var user = await base.GetByIdAsync(userId);
        if (user is null)
            throw new InvalidOperationException("User not found.");

        if (!_passwordService.VerifyPassword(oldPassword, user.Hash))
            throw new InvalidOperationException("Old password is incorrect.");

        // Enforce password policy
        EnsurePasswordComplexity(newPassword);

        // Optional: prevent re-use of the same password
        if (_passwordService.VerifyPassword(newPassword, user.Hash))
            throw new InvalidOperationException("New password must be different from the old password.");

        var newHashedPassword = _passwordService.HashPassword(newPassword);
        return await _authRepository.updateUserPasswordAsync(userId, newHashedPassword);
   }
	public async Task<User> AdminResetUserPasswordAsync(string userId, string newPassword)
{
    var user = await base.GetByIdAsync(userId);
    if (user is null)
        throw new InvalidOperationException("User not found.");

    // Enforce the same password policy for admin resets
    EnsurePasswordComplexity(newPassword);

    var newHashedPassword = _passwordService.HashPassword(newPassword);
    
    var updatedUser = await _authRepository.updateUserPasswordAsync(userId, newHashedPassword);
    return updatedUser;

	}
}