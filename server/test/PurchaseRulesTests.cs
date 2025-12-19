using Contracts.BoardDTOs;
using Contracts.TransactionDTOs;
using dataaccess;
using Microsoft.EntityFrameworkCore;
using service.Exceptions;
using service.Repositories;
using service.Repositories.Interfaces;
using service.Rules;

namespace xunittests;

public class PurchaseRulesTests
{
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static CreateBoardDto ValidBoard(string userId) => new()
    {
        BoardSize = 5,
        Week = 1,
        UserID = userId,
        Numbers = new() { 1, 2, 3, 4, 5 }
    };

    private static CreateTransactionDto ValidTransaction(string userId) => new()
    {
        UserID = userId,
        TransactionString = "GUID",
        Amount = 0m
    };


    [Fact]
    public async Task ValidateProcessPurchaseAsync_WhenBoardIsNull_ThrowsInvalidRequest()
    {
        await using var ctx = CreateInMemoryDbContext();

        var user = await test.TestDataFactory.CreateUserAsync(ctx, passwordHash: "hash");
        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new PurchaseRules(userRepo);

        await Assert.ThrowsAsync<InvalidRequestException>(() =>
            rules.ValidateProcessPurchaseAsync(null!, ValidTransaction(user.UserID)));
    }

    [Fact]
    public async Task ValidateProcessPurchaseAsync_WhenTransactionIsNull_ThrowsInvalidRequest()
    {
        await using var ctx = CreateInMemoryDbContext();

        var user = await test.TestDataFactory.CreateUserAsync(ctx, passwordHash: "hash");
        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new PurchaseRules(userRepo);

        await Assert.ThrowsAsync<InvalidRequestException>(() =>
            rules.ValidateProcessPurchaseAsync(ValidBoard(user.UserID), null!));
    }

    [Fact]
    public async Task ValidateProcessPurchaseAsync_WhenMissingTransactionUserId_ThrowsInvalidRequest()
    {
        await using var ctx = CreateInMemoryDbContext();

        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new PurchaseRules(userRepo);

        var boardDto = ValidBoard(userId: "u");
        var transactionDto = new CreateTransactionDto { UserID = " ", TransactionString = "GUID", Amount = 0m };

        await Assert.ThrowsAsync<InvalidRequestException>(() => rules.ValidateProcessPurchaseAsync(boardDto, transactionDto));
    }

    [Fact]
    public async Task ValidateProcessPurchaseAsync_WhenMissingBoardUserId_ThrowsInvalidRequest()
    {
        await using var ctx = CreateInMemoryDbContext();

        var user = await test.TestDataFactory.CreateUserAsync(ctx, passwordHash: "hash");
        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new PurchaseRules(userRepo);

        var boardDto = ValidBoard(user.UserID);
        boardDto.UserID = " ";

        await Assert.ThrowsAsync<InvalidRequestException>(() => rules.ValidateProcessPurchaseAsync(boardDto, ValidTransaction(user.UserID)));
    }

    [Fact]
    public async Task ValidateProcessPurchaseAsync_WhenBoardAndTransactionUserIdsMismatch_ThrowsRangeValidation()
    {
        await using var ctx = CreateInMemoryDbContext();

        // Create two users
        var userA = await test.TestDataFactory.CreateUserAsync(ctx, passwordHash: "hash");
        var userB = await test.TestDataFactory.CreateUserAsync(ctx, passwordHash: "hash");

        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new PurchaseRules(userRepo);

        await Assert.ThrowsAsync<RangeValidationException>(() =>
            rules.ValidateProcessPurchaseAsync(ValidBoard(userA.UserID), ValidTransaction(userB.UserID)));
    }

    [Fact]
    public async Task ValidateProcessPurchaseAsync_WhenUserDoesNotExist_ThrowsResourceNotFound()
    {
        await using var ctx = CreateInMemoryDbContext();

        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new PurchaseRules(userRepo);

        var userId = Guid.NewGuid().ToString();
        await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
            rules.ValidateProcessPurchaseAsync(ValidBoard(userId), ValidTransaction(userId)));
    }

    [Fact]
    public async Task ValidateProcessPurchaseAsync_WhenInvalidBoardSize_ThrowsRangeValidation()
    {
        await using var ctx = CreateInMemoryDbContext();

        var user = await test.TestDataFactory.CreateUserAsync(ctx, passwordHash: "hash");

        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new PurchaseRules(userRepo);

        var boardDto = ValidBoard(user.UserID);
        boardDto.BoardSize = 999;
        boardDto.Numbers = new() { 1 }; // keep it consistent with the invalid board size

        await Assert.ThrowsAsync<RangeValidationException>(() => rules.ValidateProcessPurchaseAsync(boardDto, ValidTransaction(user.UserID)));
    }

    [Fact]
    public async Task ValidateProcessPurchaseAsync_WhenWeekIsZero_ThrowsRangeValidation()
    {
        await using var ctx = CreateInMemoryDbContext();

        var user = await test.TestDataFactory.CreateUserAsync(ctx, passwordHash: "hash");

        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new PurchaseRules(userRepo);

        var boardDto = ValidBoard(user.UserID);
        boardDto.Week = 0;

        await Assert.ThrowsAsync<RangeValidationException>(() => rules.ValidateProcessPurchaseAsync(boardDto, ValidTransaction(user.UserID)));
    }

    [Fact]
    public async Task ValidateProcessPurchaseAsync_WhenNumbersMissing_ThrowsInvalidRequest()
    {
        await using var ctx = CreateInMemoryDbContext();

        var user = await test.TestDataFactory.CreateUserAsync(ctx, passwordHash: "hash");

        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new PurchaseRules(userRepo);

        var boardDto = ValidBoard(user.UserID);
        boardDto.Numbers = new();

        await Assert.ThrowsAsync<InvalidRequestException>(() => rules.ValidateProcessPurchaseAsync(boardDto, ValidTransaction(user.UserID)));
    }

    [Fact]
    public async Task ValidateProcessPurchaseAsync_WhenNumbersCountDoesNotMatchBoardSize_ThrowsRangeValidation()
    {
        await using var ctx = CreateInMemoryDbContext();

        var user = await test.TestDataFactory.CreateUserAsync(ctx, passwordHash: "hash");

        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new PurchaseRules(userRepo);

        var boardDto = ValidBoard(user.UserID);
        boardDto.Numbers = new() { 1, 2, 3, 4 }; // 4 but boardSize=5

        await Assert.ThrowsAsync<RangeValidationException>(() => rules.ValidateProcessPurchaseAsync(boardDto, ValidTransaction(user.UserID)));
    }

    [Fact]
    public async Task ValidateProcessPurchaseAsync_WhenNumberOutOfRange_ThrowsRangeValidation()
    {
        await using var ctx = CreateInMemoryDbContext();

        var user = await test.TestDataFactory.CreateUserAsync(ctx, passwordHash: "hash");

        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new PurchaseRules(userRepo);

        var boardDto = ValidBoard(user.UserID);
        boardDto.Numbers = new() { 0, 2, 3, 4, 5 };

        await Assert.ThrowsAsync<RangeValidationException>(() => rules.ValidateProcessPurchaseAsync(boardDto, ValidTransaction(user.UserID)));
    }

    [Fact]
    public async Task ValidateProcessPurchaseAsync_WhenComputedAmountTooNegative_ThrowsRangeValidation()
    {
        await using var ctx = CreateInMemoryDbContext();

        // Use BoardSize=8 => price 160, Week=7 => computedAmount = -1120 < -1000
        var user = await test.TestDataFactory.CreateUserAsync(ctx, passwordHash: "hash");

        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new PurchaseRules(userRepo);

        var boardDto = new CreateBoardDto
        {
            BoardSize = 8,
            Week = 7,
            UserID = user.UserID,
            Numbers = new() { 1, 2, 3, 4, 5, 6, 7, 8 }
        };

        await Assert.ThrowsAsync<RangeValidationException>(() => rules.ValidateProcessPurchaseAsync(boardDto, ValidTransaction(user.UserID)));
    }

   

    [Fact]
    public async Task ValidateProcessPurchaseAsync_WhenValid_Passes()
    {
        await using var ctx = CreateInMemoryDbContext();

        var user = await test.TestDataFactory.CreateUserAsync(ctx, passwordHash: "hash");

        IUserRepository userRepo = new UserRepository(ctx);
        var rules = new PurchaseRules(userRepo);

        await rules.ValidateProcessPurchaseAsync(ValidBoard(user.UserID), ValidTransaction(user.UserID));
    }
}
