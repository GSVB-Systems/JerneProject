using dataaccess.Entities;
using Service.Repositories;
using System.Threading.Tasks;
using service.Services;
using service.Services.Interfaces;

namespace service.Services;

public class UserService : Service<User>, IUserService
{
    private readonly IUser _userRepository;
    private readonly PasswordService _passwordService;

    public UserService(IUser userRepository, PasswordService passwordService)
        : base(userRepository)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    /// <summary>
    /// Registers a new user with password hashing.
    /// </summary>
    public async Task<User> RegisterUserAsync(User user, string plainPassword)
    {
        user.Hash = _passwordService.HashPassword(plainPassword);

        // Optional defaults
        user.Firstlogin = true;
        user.IsActive = true;
        user.Balance = 0;

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Verifies a user's password by UserID.
    /// </summary>
    public async Task<bool> VerifyUserPasswordAsync(string userId, string plainPassword)
    {
        var user = await GetByIdAsync(userId); // Use generic GetByIdAsync
        if (user == null) return false;

        return _passwordService.VerifyPassword(plainPassword, user.Hash);
    }

    /// <summary>
    /// Override CreateAsync to hash password automatically if needed
    /// </summary>
    public override async Task<User> CreateAsync(User user)
    {
        user.Hash = _passwordService.HashPassword(user.Hash);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        return user;
    }
}