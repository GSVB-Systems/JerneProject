using dataaccess;
using dataaccess.Entities;
using dataaccess.Entities.Enums;
using service.Services.Interfaces;
using service.Exceptions;

namespace xunittests.UserTests;

public class UpdateBalanceUnitTesting(IUserService userService, AppDbContext ctx)
{
    [Fact]
    public async Task UpdateBalance_SumsNonPendingTransactions()
    {
        var id = Guid.NewGuid().ToString();
        ctx.Users.Add(new User
        {
            UserID = id,
            Email = "bal@test.dk",
            Firstname = "Bal",
            Lastname = "User",
            Role = UserRole.Bruger,
            Hash = "h",
            IsActive = true,
            Balance = 0m
        });

        ctx.Transactions.AddRange(
            new Transaction { TransactionID = Guid.NewGuid().ToString(), UserID = id, Amount = 100m, Pending = true, TransactionString = "tx-1" },
            new Transaction { TransactionID = Guid.NewGuid().ToString(), UserID = id, Amount = 50m, Pending = false, TransactionString = "tx-2" },
            new Transaction { TransactionID = Guid.NewGuid().ToString(), UserID = id, Amount = -20m, Pending = false, TransactionString = "tx-3" }
        );

        await ctx.SaveChangesAsync();

        var result = await userService.UpdateBalanceAsync(id);

        Assert.NotNull(result);
        Assert.Equal(30m, result.Balance);
        var u = ctx.Users.Single(u => u.UserID == id);
        Assert.Equal(30m, u.Balance);
    }

    [Fact]
    public async Task UpdateBalance_NoTransactions_SetsZero()
    {
        var id = Guid.NewGuid().ToString();
        ctx.Users.Add(new User
        {
            UserID = id,
            Email = "notx@example.dk",
            Firstname = "No",
            Lastname = "Tx",
            Role = UserRole.Bruger,
            Hash = "h",
            IsActive = true,
            Balance = 123m // should be overwritten to 0
        });
        await ctx.SaveChangesAsync();

        var result = await userService.UpdateBalanceAsync(id);

        Assert.NotNull(result);
        Assert.Equal(0m, result.Balance);
        var u = ctx.Users.Single(u => u.UserID == id);
        Assert.Equal(0m, u.Balance);
    }

    [Fact]
    public async Task UpdateBalance_Invalid_NullOrEmptyId_ThrowsInvalidRequestException()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() => userService.UpdateBalanceAsync(null));
        await Assert.ThrowsAsync<InvalidRequestException>(() => userService.UpdateBalanceAsync(string.Empty));
    }

    [Fact]
    public async Task UpdateBalance_UserNotFound_ThrowsResourceNotFoundException()
    {
        var randomId = Guid.NewGuid().ToString();

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => userService.UpdateBalanceAsync(randomId));
    }
}