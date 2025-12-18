using dataaccess;
using dataaccess.Entities;
using dataaccess.Entities.Enums;
using service.Exceptions;
using service.Services.Interfaces;

namespace xunittests.UserTests;

public class GetByIdUnitTesting(IUserService userService, AppDbContext ctx)
{
    [Fact]
    public async Task GetById_ReturnsDto_WhenUserExists()
    {
        var id = Guid.NewGuid().ToString();
        var expiry = DateTime.UtcNow.AddDays(7);
        ctx.Users.Add(new User
        {
            UserID = id,
            Email = "get@test.dk",
            Firstname = "Get",
            Lastname = "User",
            Role = UserRole.Bruger,
            Hash = "h",
            IsActive = true,
            Balance = 42m,
            SubscriptionExpiresAt = expiry
        });
        await ctx.SaveChangesAsync();

        var result = await userService.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.UserID);
        Assert.Equal("get@test.dk", result.Email);
        Assert.Equal(42m, result.Balance);
        Assert.True(result.SubscriptionExpiresAtUtc.HasValue);
        Assert.Equal(expiry, result.SubscriptionExpiresAtUtc.Value);
    }

    [Fact]
    public async Task GetById_Invalid_NullOrEmptyId_ThrowsInvalidRequestException()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() => userService.GetByIdAsync(null));
        await Assert.ThrowsAsync<InvalidRequestException>(() => userService.GetByIdAsync(string.Empty));
    }

    [Fact]
    public async Task GetById_UserNotFound_ThrowsResourceNotFoundException()
    {
        var randomId = Guid.NewGuid().ToString();
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => userService.GetByIdAsync(randomId));
    }
}