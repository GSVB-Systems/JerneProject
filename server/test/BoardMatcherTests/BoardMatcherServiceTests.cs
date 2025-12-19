using dataaccess;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using service.Exceptions;
using service.Services.Interfaces;
using test;

namespace xunittests.BoardMatcherTests;

public class BoardMatcherServiceTests(IBoardMatcherService service, AppDbContext ctx)
{
    [Fact]
    public async Task GetBoardsContainingNumbersAsync_NullOrEmptyId_ThrowsInvalidRequestException()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() => service.GetBoardsContainingNumbersAsync(null!));
        await Assert.ThrowsAsync<InvalidRequestException>(() => service.GetBoardsContainingNumbersAsync(string.Empty));
        await Assert.ThrowsAsync<InvalidRequestException>(() => service.GetBoardsContainingNumbersAsync("   "));
    }

    [Fact]
    public async Task GetBoardsContainingNumbersAsync_NotFound_ThrowsResourceNotFoundException()
    {
        var randomId = Guid.NewGuid().ToString();
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.GetBoardsContainingNumbersAsync(randomId));
    }

    [Fact]
    public async Task GetBoardsContainingNumbersAsync_ReturnsMatchingBoards_ForSameWeekAndYear()
    {
        var ct = TestContext.Current.CancellationToken;

        
        var week = 12;
        var year = 2025;
        var winningBoardId = await TestDataFactory.CreateWinningBoardAsync(ctx, week: week, weekYear: year);
        await TestDataFactory.CreateWinningNumberAsync(ctx, winningBoardId, 1);
        await TestDataFactory.CreateWinningNumberAsync(ctx, winningBoardId, 2);
        await TestDataFactory.CreateWinningNumberAsync(ctx, winningBoardId, 3);

        
        var matchingBoardId = await TestDataFactory.CreateBoardAsync(ctx, boardSize: 5);
        var matchingBoard = await ctx.Set<Board>().FirstAsync(b => b.BoardID == matchingBoardId, ct);
        matchingBoard.Week = week;
        matchingBoard.Year = year;
        matchingBoard.IsActive = true;
        matchingBoard.WeeksPurchased = 1;
        await ctx.SaveChangesAsync(ct);

        await TestDataFactory.CreateBoardNumberAsync(ctx, matchingBoardId, 1);
        await TestDataFactory.CreateBoardNumberAsync(ctx, matchingBoardId, 2);
        await TestDataFactory.CreateBoardNumberAsync(ctx, matchingBoardId, 3);
        await TestDataFactory.CreateBoardNumberAsync(ctx, matchingBoardId, 8);
        await TestDataFactory.CreateBoardNumberAsync(ctx, matchingBoardId, 9);

      
        var nonMatchingBoardId = await TestDataFactory.CreateBoardAsync(ctx, boardSize: 5);
        var nonMatchingBoard = await ctx.Set<Board>().FirstAsync(b => b.BoardID == nonMatchingBoardId, ct);
        nonMatchingBoard.Week = week;
        nonMatchingBoard.Year = year;
        nonMatchingBoard.IsActive = true;
        nonMatchingBoard.WeeksPurchased = 1;
        await ctx.SaveChangesAsync(ct);

        await TestDataFactory.CreateBoardNumberAsync(ctx, nonMatchingBoardId, 1);
        await TestDataFactory.CreateBoardNumberAsync(ctx, nonMatchingBoardId, 2);
        await TestDataFactory.CreateBoardNumberAsync(ctx, nonMatchingBoardId, 8);
        await TestDataFactory.CreateBoardNumberAsync(ctx, nonMatchingBoardId, 9);
        await TestDataFactory.CreateBoardNumberAsync(ctx, nonMatchingBoardId, 10);

        // Control board: different week/year but has the numbers
        var otherWeekBoardId = await TestDataFactory.CreateBoardAsync(ctx, boardSize: 5);
        var otherWeekBoard = await ctx.Set<Board>().FirstAsync(b => b.BoardID == otherWeekBoardId, ct);
        otherWeekBoard.Week = week + 1;
        otherWeekBoard.Year = year;
        otherWeekBoard.IsActive = true;
        otherWeekBoard.WeeksPurchased = 1;
        await ctx.SaveChangesAsync(ct);

        await TestDataFactory.CreateBoardNumberAsync(ctx, otherWeekBoardId, 1);
        await TestDataFactory.CreateBoardNumberAsync(ctx, otherWeekBoardId, 2);
        await TestDataFactory.CreateBoardNumberAsync(ctx, otherWeekBoardId, 3);
        await TestDataFactory.CreateBoardNumberAsync(ctx, otherWeekBoardId, 8);
        await TestDataFactory.CreateBoardNumberAsync(ctx, otherWeekBoardId, 9);

        var results = await service.GetBoardsContainingNumbersAsync(winningBoardId);

        Assert.NotNull(results);
        Assert.Single(results);
        Assert.Equal(matchingBoardId, results[0].Board.BoardID);
        Assert.Equal(matchingBoard.UserID, results[0].User.UserID);
    }

    [Fact]
    public async Task GetBoardsContainingNumbersAsync_ReturnsEmpty_WhenWinningBoardHasNoNumbers()
    {
       
        var winningBoardId = await TestDataFactory.CreateWinningBoardAsync(ctx, week: 12, weekYear: 2025);

       
        var results = await service.GetBoardsContainingNumbersAsync(winningBoardId);

        
        Assert.NotNull(results);
        Assert.Empty(results);
    }

}



