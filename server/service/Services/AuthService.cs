using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Contracts.UserDTOs;
using dataaccess.Entities;
using service.Mappers;
using service.Repositories.Interfaces;
using service.Services.Interfaces;
using Sieve.Models;
using System.ComponentModel.DataAnnotations;
using Contracts.Validation;
using service.Rules.RuleInterfaces;

namespace service.Services;

public class AuthService : Service<User, User, User>, IAuthService
{
    protected readonly IAuthRepository _authRepository;
    protected readonly PasswordService _passwordService;
    protected readonly IAuthRules _authRules;

    public AuthService(IAuthRepository authRepository, PasswordService passwordService, IAuthRules authRules)
        : base(authRepository)
    {
        _authRules = authRules;
        _authRepository = authRepository;
        _passwordService = passwordService;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await base.GetByIdAsync(id);
    }

    public async Task<PagedResult<UserDto>> GetAllAsync(SieveModel? parameters = null)
    {
        _authRules.ValidateGetAllAsync(parameters);
        var result = await base.GetAllAsync(parameters);
        return new PagedResult<UserDto>
        {
            Items = result.Items.Select(UserMapper.ToDto).ToList(),
            TotalCount = result.TotalCount
        };
    }

    public Task<User> CreateAsync(User entity)
    {
        _authRules.ValidateCreateAsync(entity);
        return base.CreateAsync(entity);
    }

    public Task<User?> UpdateAsync(string id, User entity)
    {
        _authRules.ValidateUpdateAsync(id, entity);
        return base.UpdateAsync(id, entity);
    }

    public Task<bool> DeleteAsync(string id)
    {
        _authRules.ValidateDeleteAsync(id);
        return base.DeleteAsync(id);
    }

    public async Task<bool> verifyPasswordByEmailAsync(string email, string plainPassword)
    {
        _authRules.ValidateVerifyPasswordByEmailAsync(email, plainPassword);
        var user = await GetUserByEmailAsync(email);
        if (user == null) return false;

        return _passwordService.VerifyPassword(plainPassword, user.Hash);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        _authRules.ValidateGetUserByEmailAsync(email);
        return await _authRepository.getUserByEmailAsync(email);
    }

    private static void EnsurePasswordComplexity(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Ny adgangskode nødvendig.");

        var attribute = new PasswordComplexityAttribute();
        var result = attribute.GetValidationResult(password, new ValidationContext(new object()));
        if (result != ValidationResult.Success)
            throw new InvalidOperationException(result?.ErrorMessage ?? "Adgangskoden opfylder ikke kravene.");
    }

    public async Task<User> UpdateUserPasswordAsync(string userId, string oldPassword, string newPassword)
    {
        await _authRules.ValidateUpdateUserPasswordAsync(userId, oldPassword, newPassword);
        var user = await base.GetByIdAsync(userId);
        if (user is null)
            throw new InvalidOperationException("Brugeren ikke fundet.");

        if (!_passwordService.VerifyPassword(oldPassword, user.Hash))
            throw new InvalidOperationException("Eksisterende adgangskode er forkert.");


        EnsurePasswordComplexity(newPassword);


        if (_passwordService.VerifyPassword(newPassword, user.Hash))
            throw new InvalidOperationException("Den nye adgangskode må ikke være den samme som den eksisterende.");

        var newHashedPassword = _passwordService.HashPassword(newPassword);
        return await _authRepository.updateUserPasswordAsync(userId, newHashedPassword);
   }
	public async Task<User> AdminResetUserPasswordAsync(string userId, string newPassword)
{
    await _authRules.ValidateAdminResetUserPasswordAsync(userId, newPassword);
    var user = await base.GetByIdAsync(userId);
    if (user is null)
        throw new InvalidOperationException("Brugeren ikke fundet.");

        EnsurePasswordComplexity(newPassword);

        var newHashedPassword = _passwordService.HashPassword(newPassword);
        return await _authRepository.updateUserPasswordAsync(userId, newHashedPassword);
    }
}