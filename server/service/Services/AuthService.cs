using dataaccess.Entities;
using service.Repositories.Interfaces;
using service.Services.Interfaces;

namespace service.Services;

public class AuthService : IAuthService
{
    
    protected readonly IAuthRepository _authRepository;
    protected readonly PasswordService _passwordService;

    public AuthService(IAuthRepository authRepository, PasswordService passwordService){

        _authRepository = authRepository;
        _passwordService = passwordService;
    }

    public Task<User?> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User> CreateAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task<User?> UpdateAsync(string id, User entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> verifyPasswordByEmailAsync(string email, string plainPassword)
    {
        var user = await GetUserByEmailAsync(email); // Use generic GetByIdAsync
        if (user == null) return false;

        return _passwordService.VerifyPassword(plainPassword, user.Hash);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _authRepository.getUserByEmailAsync(email);
    }
}