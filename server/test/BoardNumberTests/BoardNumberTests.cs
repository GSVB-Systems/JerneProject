using dataaccess;
using dataaccess.Entities;
using Contracts.BoardNumberDTOs;
using Microsoft.EntityFrameworkCore;
using service.Exceptions;
using service.Services.Interfaces;
using Sieve.Models;

namespace test.BoardNumberTests;

public class BoardNumberTests(IBoardNumberService service, AppDbContext ctx)
{

    [Fact]
    public async Task Create_ReturnsDto_WhenValid()
    {
        await TestDataFactory.CreateBoardAsync(ctx, boardId: string.Empty);

        var dto = new CreateBoardNumberDto { Number = 5 };

        var result = await service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(5, result.Number);
        Assert.False(string.IsNullOrWhiteSpace(result.BoardNumberID));
    }

    [Fact]
    public async Task Create_InvalidNumber_ThrowsRangeValidationException()
    {
        await Assert.ThrowsAsync<RangeValidationException>(() => service.CreateAsync(new CreateBoardNumberDto { Number = 0 }));
    }

    [Fact]
    public async Task Create_DuplicateNumber_ThrowsDuplicateResourceException()
    {
        var boardId = await TestDataFactory.CreateBoardAsync(ctx);
        await TestDataFactory.CreateBoardNumberAsync(ctx, boardId, 7);

        await TestDataFactory.CreateBoardAsync(ctx, boardId: string.Empty);

        await Assert.ThrowsAsync<DuplicateResourceException>(() => service.CreateAsync(new CreateBoardNumberDto { Number = 7 }));
    }

    [Fact]
    public async Task Create_NullDto_ThrowsInvalidRequestException()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() => service.CreateAsync(null));
    }


    [Fact]
    public async Task GetById_ReturnsDto_WhenExists()
    {
        var boardId = await TestDataFactory.CreateBoardAsync(ctx);
        var id = await TestDataFactory.CreateBoardNumberAsync(ctx, boardId, 3);

        var result = await service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(3, result.Number);
        Assert.Equal(id, result.BoardNumberID);
    }

    [Fact]
    public async Task GetById_Invalid_NullOrEmptyId_ThrowsInvalidRequestException()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() => service.GetByIdAsync(null));
        await Assert.ThrowsAsync<InvalidRequestException>(() => service.GetByIdAsync(string.Empty));
    }

    [Fact]
    public async Task GetById_NotFound_ThrowsResourceNotFoundException()
    {
        var randomId = Guid.NewGuid().ToString();
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.GetByIdAsync(randomId));
    }

    [Fact]
    public async Task GetById_WhitespaceId_ThrowsInvalidRequestException()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() => service.GetByIdAsync("   "));
    }


    [Fact]
    public async Task GetAll_NoParams_ReturnsAll()
    {
        ctx.Set<BoardNumber>().RemoveRange(ctx.Set<BoardNumber>());
        await ctx.SaveChangesAsync();

        var boardId = await TestDataFactory.CreateBoardAsync(ctx);
        for (var i = 0; i < 3; i++)
        {
            await TestDataFactory.CreateBoardNumberAsync(ctx, boardId, i + 1);
        }

        var result = await service.GetAllAsync(null);

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(1, result.Page);
    }
    

    [Fact]
    public async Task GetAll_InvalidPage_ThrowsRangeValidationException()
    {
        await Assert.ThrowsAsync<RangeValidationException>(() => service.GetAllAsync(new SieveModel { Page = 0 }));
    }


    [Fact]
    public async Task Update_ReturnsDto_WhenValid()
    {
        var boardId = await TestDataFactory.CreateBoardAsync(ctx);
        var id = await TestDataFactory.CreateBoardNumberAsync(ctx, boardId, 2);

        var result = await service.UpdateAsync(id, new UpdateBoardNumberDto { Number = 4 });

        Assert.NotNull(result);
        Assert.Equal(4, result.Number);
    }

    [Fact]
    public async Task Update_InvalidId_ThrowsInvalidRequestException()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() => service.UpdateAsync(null, new UpdateBoardNumberDto { Number = 2 }));
    }

    [Fact]
    public async Task Update_NotFound_ThrowsResourceNotFoundException()
    {
        var randomId = Guid.NewGuid().ToString();
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.UpdateAsync(randomId, new UpdateBoardNumberDto { Number = 2 }));
    }

    [Fact]
    public async Task Update_NumberConflict_ThrowsDuplicateResourceException()
    {
        var boardId = await TestDataFactory.CreateBoardAsync(ctx);
        var id1 = await TestDataFactory.CreateBoardNumberAsync(ctx, boardId, 8);
        var id2 = await TestDataFactory.CreateBoardNumberAsync(ctx, boardId, 9);

        await Assert.ThrowsAsync<DuplicateResourceException>(() => service.UpdateAsync(id1, new UpdateBoardNumberDto { Number = 9 }));
    }


    [Fact]
    public async Task Delete_Deletes_WhenExists()
    {
        var boardId = await TestDataFactory.CreateBoardAsync(ctx);
        var id = await TestDataFactory.CreateBoardNumberAsync(ctx, boardId, 11);

        var deleted = await service.DeleteAsync(id);

        Assert.True(deleted);
        var still = await ctx.Set<BoardNumber>().AnyAsync(b => b.BoardNumberID == id);
        Assert.False(still);
    }

    [Fact]
    public async Task Delete_InvalidId_ThrowsInvalidRequestException()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() => service.DeleteAsync(null));
    }

    [Fact]
    public async Task Delete_NotFound_ThrowsResourceNotFoundException()
    {
        var randomId = Guid.NewGuid().ToString();
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.DeleteAsync(randomId));
    }

    [Fact]
    public async Task Delete_Twice_SecondCall_ThrowsResourceNotFoundException()
    {
        var boardId = await TestDataFactory.CreateBoardAsync(ctx);
        var id = await TestDataFactory.CreateBoardNumberAsync(ctx, boardId, 12);

        var first = await service.DeleteAsync(id);
        Assert.True(first);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.DeleteAsync(id));
    }
}