using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Contracts.UserDTOs;
using dataaccess.Entities;
using service.Mappers;
using service.Repositories.Interfaces;
using service.Services.Interfaces;
using Sieve.Models;

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

    public async Task<User> UpdateUserPasswordAsync(string userId, string oldPassword, string newPassword)
    {
        var user = await base.GetByIdAsync(userId);
        if (user is null)
            throw new InvalidOperationException("User not found.");

        if (!_passwordService.VerifyPassword(oldPassword, user.Hash))
            throw new InvalidOperationException("Old password is incorrect.");

        var newHashedPassword = _passwordService.HashPassword(newPassword);
        return await _authRepository.updateUserPasswordAsync(userId, newHashedPassword);
    }
}