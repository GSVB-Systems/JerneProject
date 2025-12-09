using System;
using System.Collections.Generic;
using System.Linq;
using Contracts.UserDTOs;
using Contracts;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using Service.Repositories;
using service.Mappers;
using service.Services.Interfaces;
using Sieve.Services;

namespace service.Services;

public class UserService : Service<User>, IUserService
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
    
    public async Task<PagedResult<UserDto>> GetAllAsync()
    {
        return await GetAllAsync(null);
    }
    
    public async Task<PagedResult<UserDto>> GetAllAsync(UserQueryParameters? parameters)
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
    
    //har addet 1 linje her som sætter subscription expiry til et år frem ved registrering
    public async Task<UserDto> RegisterUserAsync(RegisterUserDto dto)
    {
        var entity = UserMapper.ToEntity(dto);
        entity.Hash = _passwordService.HashPassword(dto.Password);
        entity.Firstlogin = true;
        entity.IsActive = true;
        entity.Balance = 0;
        entity.SubscriptionExpiresAt = DateTime.UtcNow.AddYears(1);

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

    // Explicit interface implementations to satisfy IService<UserDto>
    async Task<IEnumerable<UserDto>> IService<UserDto>.GetAllAsync() => (await GetAllAsync()).Items;
    async Task<UserDto?> IService<UserDto>.GetByIdAsync(string id) => await GetByIdAsync(id);
    async Task<UserDto> IService<UserDto>.CreateAsync(UserDto entity) => await CreateAsync(entity);
    async Task<UserDto?> IService<UserDto>.UpdateAsync(string id, UserDto entity) => await UpdateAsync(id, entity);
    async Task<bool> IService<UserDto>.DeleteAsync(string id) => await DeleteAsync(id);
  
    public override async Task<User> CreateAsync(User user)
    {
        user.Hash = _passwordService.HashPassword(user.Hash);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        return user;
    }
}