using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dataaccess;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using service.Repositories;
using service.Repositories.Interfaces;
using service.Rules;
using service.Services;
using Sieve.Models;
using Xunit;
using test;

namespace service.Tests;

public class AuthServiceTests : IDisposable
{
    private readonly AppDbContext _ctx;
    private readonly IAuthRepository _authRepository;
    private readonly PasswordService _passwordService;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"AuthServiceTests_{Guid.NewGuid()}")
            .Options;
        _ctx = new AppDbContext(options);
        _authRepository = new AuthRepository(_ctx);
        _passwordService = new PasswordService();
        var authRules = new AuthRules(_authRepository);
        _service = new AuthService(_authRepository, _passwordService, authRules);
    }

    private Task<User> SeedUserAsync(string password = "Password1!")
    {
        var hash = _passwordService.HashPassword(password);
        return TestDataFactory.CreateUserAsync(_ctx, hash);
    }

    public void Dispose() => _ctx?.Dispose();

    [Fact]
    public async Task GetByIdAsync_ReturnsUser()
    {
        var user = await SeedUserAsync();
        var result = await _service.GetByIdAsync(user.UserID);

        Assert.NotNull(result);
        Assert.Equal(user.UserID, result?.UserID);
    }

    [Fact]
    public async Task GetAllAsync_IncludesSeededUsers()
    {
        await SeedUserAsync();
        await SeedUserAsync();

        var result = await _service.GetAllAsync(new SieveModel { Page = 1, PageSize = 10 });

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }



   

    [Fact]
    public async Task DeleteAsync_RemovesUser()
    {
        var user = await SeedUserAsync();

        var deleted = await _service.DeleteAsync(user.UserID);

        Assert.True(deleted);
        Assert.False(await _ctx.Set<User>().AnyAsync(u => u.UserID == user.UserID));
    }

    [Fact]
    public async Task VerifyPasswordByEmailAsync_ReturnsExpected()
    {
        var user = await SeedUserAsync("Password1!");

        Assert.True(await _service.verifyPasswordByEmailAsync(user.Email, "Password1!"));
        Assert.False(await _service.verifyPasswordByEmailAsync(user.Email, "WrongPass1!"));
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsUser()
    {
        var user = await SeedUserAsync();
        var result = await _service.GetUserByEmailAsync(user.Email);

        Assert.Equal(user.UserID, result.UserID);
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_ChangesHash()
    {
        var user = await SeedUserAsync("OldPass1!");

        await _service.UpdateUserPasswordAsync(user.UserID, "OldPass1!", "NewPass1!");
        var updated = await _service.GetUserByEmailAsync(user.Email);

        Assert.True(_passwordService.VerifyPassword("NewPass1!", updated.Hash));
    }

    [Fact]
    public async Task AdminResetUserPasswordAsync_ChangesHash()
    {
        var user = await SeedUserAsync("AdminPass1!");

        await _service.AdminResetUserPasswordAsync(user.UserID, "ResetPass1!");
        var updated = await _service.GetUserByEmailAsync(user.Email);

        Assert.True(_passwordService.VerifyPassword("ResetPass1!", updated.Hash));
    }
}
