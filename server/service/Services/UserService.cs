using service.Rules.RuleInterfaces;
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
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserRules _userRules;
    
    public UserService(IUserRepository userRepository, PasswordService passwordService, ISieveProcessor sieveProcessor, ITransactionRepository transactionRepository, IUserRules userRules)
        : base(userRepository)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _sieveProcessor = sieveProcessor;
        _transactionRepository = transactionRepository;
        _userRules = userRules ?? throw new ArgumentNullException(nameof(userRules));
    }
    
    public async Task<UserDto?> GetByIdAsync(string id)
    {
        await _userRules.ValidateGetByIdAsync(id);
        
        var user = await base.GetByIdAsync(id);
        return user == null ? null : UserMapper.ToDto(user);
    }

    public async Task<PagedResult<UserDto>> GetAllAsync(SieveModel? parameters)
    {
        var sieveModel = parameters ?? new UserQueryParameters();
        
        await _userRules.ValidateGetAllAsync(sieveModel);
        
        var query = _userRepository.AsQueryable();
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
        await _userRules.ValidateCreateAsync(createDto);
        
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
        await _userRules.ValidateUpdateAsync(id, updateDto);
        
        var existing = await base.GetByIdAsync(id);
        if (existing == null) return null;

        
        UserMapper.ApplyUpdate(existing, updateDto);

        await _userRepository.UpdateAsync(existing);
        await _userRepository.SaveChangesAsync();

        return UserMapper.ToDto(existing);
    }
    

    public async Task<bool> DeleteAsync(string id)
    {
        await _userRules.ValidateDeleteAsync(id);
        return await base.DeleteAsync(id);
    }
    
    public async Task<bool> IsSubscriptionActiveAsync(string userId)
    {
        await _userRules.ValidateIsSubscriptionActiveAsync(userId);
        
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
        await _userRules.ValidateExtendSubscriptionAsync(userId, months);
        
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

    public async Task<UserDto?> UpdateBalanceAsync(string userId)
    {
        await _userRules.ValidateUpdateBalanceAsync(userId);
        
        var user = await base.GetByIdAsync(userId);
        if (user == null) return null;

        var transactions = await _transactionRepository
            .AsQueryable()
            .Where(t => t.UserID == userId)
            .ToListAsync();

        var summed = transactions
            .Where(t => !t.Pending)
            .Sum(t => t.Amount);

        user.Balance = summed;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return UserMapper.ToDto(user);
    }

}