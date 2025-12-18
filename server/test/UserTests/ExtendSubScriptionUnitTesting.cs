using dataaccess;
using dataaccess.Entities;
using dataaccess.Entities.Enums;
using service.Exceptions;
using service.Services.Interfaces;

namespace xunittests.UserTests;

public class ExtendSubscriptionUnitTesting(IUserService userService, AppDbContext ctx)
{
    [Fact]
    public async Task ExtendSubscription_HappyPath_ExtendsExpiry()
    {
        var id = Guid.NewGuid().ToString();
        var baseTime = DateTime.UtcNow.AddDays(1);
        ctx.Users.Add(new User
        {
            UserID = id,
            Email = "sub@test.dk",
            Firstname = "Sub",
            Lastname = "User",
            Role = UserRole.Bruger,
            Hash = "h",
            IsActive = true,
            SubscriptionExpiresAt = baseTime
        });
        await ctx.SaveChangesAsync();

        var months = 3;
        var result = await userService.ExtendSubscriptionAsync(id, months);

        Assert.NotNull(result);
        Assert.True(result.SubscriptionExpiresAtUtc.HasValue);
        Assert.Equal(baseTime.AddMonths(months), result.SubscriptionExpiresAtUtc.Value);
    }

    [Fact]
    public async Task ExtendSubscription_InvalidId_ThrowsInvalidRequestException()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() => userService.ExtendSubscriptionAsync(null, 1));
        await Assert.ThrowsAsync<InvalidRequestException>(() => userService.ExtendSubscriptionAsync(string.Empty, 1));
    }

    [Fact]
    public async Task ExtendSubscription_InvalidMonths_ThrowsRangeValidationException()
    {
        var id = Guid.NewGuid().ToString();
        ctx.Users.Add(new User
        {
            UserID = id,
            Email = "sub2@test.dk",
            Firstname = "Sub2",
            Lastname = "User",
            Role = UserRole.Bruger,
            Hash = "h",
            IsActive = true
        });
        await ctx.SaveChangesAsync();

        await Assert.ThrowsAsync<RangeValidationException>(() => userService.ExtendSubscriptionAsync(id, 0));
        await Assert.ThrowsAsync<RangeValidationException>(() => userService.ExtendSubscriptionAsync(id, -5));
    }

    [Fact]
    public async Task ExtendSubscription_UserNotFound_ThrowsResourceNotFoundException()
    {
        var randomId = Guid.NewGuid().ToString();
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => userService.ExtendSubscriptionAsync(randomId, 1));
    }
}