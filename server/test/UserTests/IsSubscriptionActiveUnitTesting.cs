using dataaccess;
using dataaccess.Entities;
using dataaccess.Entities.Enums;
using service.Exceptions;
using service.Services.Interfaces;

namespace xunittests.UserTests;

public class IsSubscriptionActiveUnitTesting(IUserService userService, AppDbContext ctx)
{
    [Fact]
    public async Task IsSubscriptionActive_ReturnsTrue_WhenExpiryInFuture()
    {
        var id = Guid.NewGuid().ToString();
        ctx.Users.Add(new User
        {
            UserID = id,
            Email = "active@test.dk",
            Firstname = "Active",
            Lastname = "User",
            Role = UserRole.Bruger,
            Hash = "h",
            IsActive = true,
            SubscriptionExpiresAt = DateTime.UtcNow.AddDays(1)
        });
        await ctx.SaveChangesAsync();

        var result = await userService.IsSubscriptionActiveAsync(id);

        Assert.True(result);
    }

    [Fact]
    public async Task IsSubscriptionActive_ReturnsFalse_WhenExpiredOrNoExpiry()
    {
        var id = Guid.NewGuid().ToString();
        ctx.Users.Add(new User
        {
            UserID = id,
            Email = "expired@test.dk",
            Firstname = "Expired",
            Lastname = "User",
            Role = UserRole.Bruger,
            Hash = "h",
            IsActive = true,
            SubscriptionExpiresAt = DateTime.UtcNow.AddDays(-1)
        });
        await ctx.SaveChangesAsync();

        var result = await userService.IsSubscriptionActiveAsync(id);

        Assert.False(result);
    }

    [Fact]
    public async Task IsSubscriptionActive_Invalid_NullOrEmptyId_ThrowsInvalidRequestException()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() => userService.IsSubscriptionActiveAsync(null));
        await Assert.ThrowsAsync<InvalidRequestException>(() => userService.IsSubscriptionActiveAsync(string.Empty));
    }

    [Fact]
    public async Task IsSubscriptionActive_UserNotFound_ThrowsResourceNotFoundException()
    {
        var randomId = Guid.NewGuid().ToString();
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => userService.IsSubscriptionActiveAsync(randomId));
    }
}