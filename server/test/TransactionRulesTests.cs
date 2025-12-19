using Contracts.TransactionDTOs;
using dataaccess;
using Microsoft.EntityFrameworkCore;
using service.Exceptions;
using service.Repositories;
using service.Repositories.Interfaces;
using service.Rules;

namespace xunittests;

public class TransactionRulesTests
{
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task ValidateCreateAsync_WhenMissingUserId_ThrowsInvalidRequest()
    {
        await using var ctx = CreateInMemoryDbContext();

        ITransactionRepository transactionRepo = new TransactionRepository(ctx);
        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new TransactionRules(transactionRepo, userRepo);

        var dto = new CreateTransactionDto
        {
            UserID = " ",
            TransactionString = "GUID",
            Amount = 0
        };

        await Assert.ThrowsAsync<InvalidRequestException>(() => rules.ValidateCreateAsync(dto));
    }

    [Fact]
    public async Task ValidateCreateAsync_WhenUserDoesNotExist_ThrowsResourceNotFound()
    {
        await using var ctx = CreateInMemoryDbContext();

        ITransactionRepository transactionRepo = new TransactionRepository(ctx);
        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new TransactionRules(transactionRepo, userRepo);

        var dto = new CreateTransactionDto
        {
            UserID = Guid.NewGuid().ToString(),
            TransactionString = "GUID",
            Amount = 0
        };

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => rules.ValidateCreateAsync(dto));
    }

    [Fact]
    public async Task ValidateCreateAsync_WhenValid_Passes()
    {
        await using var ctx = CreateInMemoryDbContext();

        // Create user
        var user = await test.TestDataFactory.CreateUserAsync(ctx, passwordHash: "hash");

        ITransactionRepository transactionRepo = new TransactionRepository(ctx);
        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new TransactionRules(transactionRepo, userRepo);

        var dto = new CreateTransactionDto
        {
            UserID = user.UserID,
            TransactionString = "GUID",
            Amount = 10m
        };

        await rules.ValidateCreateAsync(dto);
    }

    [Fact]
    public async Task ValidateGetByIdAsync_WhenMissingId_ThrowsInvalidRequest()
    {
        await using var ctx = CreateInMemoryDbContext();

        ITransactionRepository transactionRepo = new TransactionRepository(ctx);
        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new TransactionRules(transactionRepo, userRepo);

        await Assert.ThrowsAsync<InvalidRequestException>(() => rules.ValidateGetByIdAsync(""));
    }
}
