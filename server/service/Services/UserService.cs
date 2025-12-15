using Contracts.UserDTOs;
using Contracts;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using service.Repositories.Interfaces;
using service.Mappers;
using service.Services.Interfaces;
using Sieve.Models;
using Sieve.Services;

namespace service.Services;

public class UserService : Service<User, RegisterUserDto, UpdateUserDto>, IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordService _passwordService;
    private readonly ISieveProcessor _sieveProcessor;

    public UserService(IUserRepository userRepository, PasswordService passwordService, ISieveProcessor sieveProcessor)
        : base(userRepository)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _sieveProcessor = sieveProcessor;
    }
    
    public async Task<UserDto?> GetByIdAsync(string id)
    {
        var user = await base.GetByIdAsync(id);
        return user == null ? null : UserMapper.ToDto(user);
    }

    public async Task<PagedResult<UserDto>> GetAllAsync(SieveModel? parameters)
    {
        var query = _userRepository.AsQueryable();
        var sieveModel = parameters ?? new UserQueryParameters();

        var totalCount = await query.CountAsync();
        var processedQuery = _sieveProcessor.Apply(sieveModel, query);
        var users = await processedQuery.ToListAsync();

        return new PagedResult<UserDto>
        {
            Items = users.Select(UserMapper.ToDto).ToList(),
            TotalCount = totalCount,
            Page = sieveModel.Page ?? 1,
            PageSize = sieveModel.PageSize ?? users.Count
        };
    }

    public async Task<UserDto> CreateAsync(RegisterUserDto createDto)
    {
        var entity = UserMapper.ToEntity(createDto);
        entity.Hash = _passwordService.HashPassword(createDto.Password);
        entity.Firstlogin = true;
        entity.IsActive = true;
        entity.Balance = 0;
        entity.SubscriptionExpiresAt = DateTime.UtcNow.AddYears(1);

        await _userRepository.AddAsync(entity);
        await _userRepository.SaveChangesAsync();

        return UserMapper.ToDto(entity);
    }

    public async Task<UserDto?> UpdateAsync(string id, UpdateUserDto updateDto)
    {
        var existing = await base.GetByIdAsync(id);
        if (existing == null) return null;

        
        UserMapper.ApplyUpdate(existing, updateDto);

        await _userRepository.UpdateAsync(existing);
        await _userRepository.SaveChangesAsync();

        return UserMapper.ToDto(existing);
    }
    

    public async Task<bool> DeleteAsync(string id)
    {
        return await base.DeleteAsync(id);
    }
    
    

    public async Task<bool> VerifyUserPasswordAsync(string userId, string plainPassword)
    {
        var user = await base.GetByIdAsync(userId); 
        if (user == null) return false;

        return _passwordService.VerifyPassword(plainPassword, user.Hash);
    }

    public async Task<bool> IsSubscriptionActiveAsync(string userId)
    {
        var user = await base.GetByIdAsync(userId);
        if (user == null) return false;
        return IsSubscriptionActiveHelper(user);
    }
    
    private static bool IsSubscriptionActiveHelper(User user)
    {
        if (user == null) return false;
        return user.SubscriptionExpiresAt.HasValue && user.SubscriptionExpiresAt.Value > DateTime.UtcNow;
    }

    public async Task<UserDto?> ExtendSubscriptionAsync(string userId, int months)
    {
        var user = await base.GetByIdAsync(userId);
        if (user == null) return null;

        var baseTime = user.SubscriptionExpiresAt.HasValue && user.SubscriptionExpiresAt.Value > DateTime.UtcNow
            ? user.SubscriptionExpiresAt.Value
            : DateTime.UtcNow;

        user.SubscriptionExpiresAt = baseTime.AddMonths(months);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return UserMapper.ToDto(user);
    }

    public async Task<UserDto?> UpdateBalanceAsync(string userId, decimal price)
    {
        var user = await base.GetByIdAsync(userId);
        if (user == null) return null;

        user.Balance += price;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return UserMapper.ToDto(user);
    }
}