
using dataaccess.Entities;
using dataaccess.Entities.Enums;
using Service.Repositories;
using service.Mappers;
using service.Services.Interfaces;
using Contracts.UserDTOs;

namespace service.Services;

public class UserService : Service<User>, IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordService _passwordService;

    public UserService(IUserRepository userRepository, PasswordService passwordService)
        : base(userRepository)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }
    
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await base.GetAllAsync();
        return users.Select(UserMapper.ToDto);
    }

    public async Task<UserDto?> GetByIdAsync(string id)
    {
        var user = await base.GetByIdAsync(id);
        return user == null ? null : UserMapper.ToDto(user);
    }

    public Task<UserDto> CreateAsync(UserDto entity)
    {
        throw new NotSupportedException("skal lige finde ud af hvad jeg gør her - lige nu bruger controlleren bare RegisterUser");
    }

    public async Task<UserDto?> UpdateAsync(string id, UserDto dto)
    {
        var existing = await base.GetByIdAsync(id);
        if (existing == null) return null;

        if (dto.Firstname != null) existing.Firstname = dto.Firstname;
        if (dto.Lastname != null) existing.Lastname = dto.Lastname;
        if (dto.Email != null) existing.Email = dto.Email;

        existing.Role = dto.Role;
        existing.Firstlogin = dto.Firstlogin;
        existing.IsActive = dto.IsActive;
        existing.Balance = dto.Balance;

        await _userRepository.UpdateAsync(existing);
        await _userRepository.SaveChangesAsync();

        return UserMapper.ToDto(existing);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await base.DeleteAsync(id);
    }
    
    public async Task<UserDto> RegisterUserAsync(RegisterUserDto dto)
    {
        var entity = UserMapper.ToEntity(dto);
        entity.Hash = _passwordService.HashPassword(dto.Password);
        entity.Firstlogin = true;
        entity.IsActive = true;
        entity.Balance = 0;

        await _userRepository.AddAsync(entity);
        await _userRepository.SaveChangesAsync();

        return UserMapper.ToDto(entity);
    }

    public async Task<bool> VerifyUserPasswordAsync(string userId, string plainPassword)
    {
        var user = await base.GetByIdAsync(userId); 
        if (user == null) return false;

        return _passwordService.VerifyPassword(plainPassword, user.Hash);
    }

  
    public override async Task<User> CreateAsync(User user)
    {
        user.Hash = _passwordService.HashPassword(user.Hash);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        return user;
    }
}