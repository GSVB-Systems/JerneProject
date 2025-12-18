using System.ComponentModel.DataAnnotations;
using dataaccess;
using dataaccess.Entities;
using dataaccess.Entities.Enums;
using Contracts.BoardDTOs;
using Contracts.BoardNumberDTOs;
using Microsoft.EntityFrameworkCore;
using service.Exceptions;
using service.Services.Interfaces;
using Sieve.Models;


namespace test.BoardServiceTests
{
    public class BoardServiceTests(IBoardService service, AppDbContext ctx)
    {
        private async Task<string> CreateUserAsync(string? userId = null)
        {
            userId ??= Guid.NewGuid().ToString();

            if (!await ctx.Set<User>().AnyAsync(u => u.UserID == userId))
            {
                ctx.Set<User>().Add(new User
                {
                    UserID = userId,
                    Firstname = "Test",
                    Lastname = "User",
                    Email = $"{userId}@example.local",
                    Hash = "hash",
                    Role = (UserRole)0,
                    Firstlogin = false,
                    IsActive = true,
                    Balance = 0m,
                    Transactions = new List<Transaction>(),
                    Boards = new List<Board>()
                });
                await ctx.SaveChangesAsync();
            }

            return userId;
        }

        [Fact]
        public async Task GetById_ReturnsDto_WhenExists()
        {
            var boardId = await TestDataFactory.CreateBoardAsync(ctx, boardSize: 5);
            var result = await service.GetByIdAsync(boardId);

            Assert.NotNull(result);
            Assert.Equal(boardId, result.BoardID);
        }

        [Fact]
        public async Task GetById_Invalid_NullOrEmpty_ThrowsInvalidRequestException()
        {
            await Assert.ThrowsAsync<InvalidRequestException>(() => service.GetByIdAsync(null));
            await Assert.ThrowsAsync<InvalidRequestException>(() => service.GetByIdAsync(string.Empty));
        }

        [Fact]
        public async Task GetAll_NoParams_ReturnsAll()
        {
            ctx.Set<Board>().RemoveRange(ctx.Set<Board>());
            await ctx.SaveChangesAsync();

            for (var i = 0; i < 3; i++)
            {
                await TestDataFactory.CreateBoardAsync(ctx, boardSize: 5);
            }

            var result = await service.GetAllAsync(null);

            Assert.NotNull(result);
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(3, result.Items.Count);
        }

        [Fact]
        public async Task GetAll_InvalidPage_ThrowsRangeValidationException()
        {
            await Assert.ThrowsAsync<RangeValidationException>(() => service.GetAllAsync(new SieveModel { Page = 0 }));
        }

        [Fact]
        public async Task Create_ReturnsDto_WhenValid()
        {
            var userId = await CreateUserAsync();

            var dto = new CreateBoardDto
            {
                BoardSize = 5,
                Numbers = new List<int> { 1, 2, 3, 4, 5 },
                Week = 1,
                UserID = userId
            };

            var result = await service.CreateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(5, result.BoardSize);
            Assert.False(string.IsNullOrWhiteSpace(result.BoardID));
            Assert.Equal(userId, result.UserID);
            Assert.Equal(5, result.Numbers.Count);
        }

        [Fact]
        public async Task Create_NullDto_ThrowsInvalidRequestException()
        {
            await Assert.ThrowsAsync<InvalidRequestException>(() => service.CreateAsync(null));
        }

        [Fact]
        public async Task Create_SizeMismatch_ThrowsValidationException()
        {
            var userId = await CreateUserAsync();

            var dto = new CreateBoardDto
            {
                BoardSize = 5,
                Numbers = new List<int> { 1, 2 }, // mismatch
                Week = 1,
                UserID = userId
            };

            await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(dto));
        }

        [Fact]
        public async Task Update_ReturnsDto_WhenValid()
        {
            var boardId = await TestDataFactory.CreateBoardAsync(ctx, boardSize: 5);
            var update = new UpdateBoardDto
            {
                BoardSize = 5,
                Numbers = new List<CreateBoardNumberDto>
                {
                    new CreateBoardNumberDto { Number = 1 },
                    new CreateBoardNumberDto { Number = 2 },
                    new CreateBoardNumberDto { Number = 3 },
                    new CreateBoardNumberDto { Number = 4 },
                    new CreateBoardNumberDto { Number = 5 }
                }
            };

            var result = await service.UpdateAsync(boardId, update);

            Assert.NotNull(result);
            Assert.Equal(5, result.BoardSize);
            Assert.Equal(5, result.Numbers.Count);
        }

        [Fact]
        public async Task Update_InvalidId_ThrowsInvalidRequestException()
        {
            await Assert.ThrowsAsync<InvalidRequestException>(() => service.UpdateAsync(null, new UpdateBoardDto { BoardSize = 5 }));
        }

        [Fact]
        public async Task Update_NotFound_ReturnsNull()
        {
            var randomId = Guid.NewGuid().ToString();
            var result = await service.UpdateAsync(randomId, new UpdateBoardDto { BoardSize = 5 });
            Assert.Null(result);
        }

        [Fact]
        public async Task Delete_Deletes_WhenExists()
        {
            var boardId = await TestDataFactory.CreateBoardAsync(ctx, boardSize: 5);
            var deleted = await service.DeleteAsync(boardId);

            Assert.True(deleted);
            var still = await ctx.Set<Board>().AnyAsync(b => b.BoardID == boardId);
            Assert.False(still);
        }

        [Fact]
        public async Task Delete_InvalidId_ThrowsInvalidRequestException()
        {
            await Assert.ThrowsAsync<InvalidRequestException>(() => service.DeleteAsync(null));
        }

        [Fact]
        public async Task Delete_NotFound_ReturnsFalse()
        {
            var randomId = Guid.NewGuid().ToString();
            var result = await service.DeleteAsync(randomId);
            Assert.False(result);
        }

        [Fact]
        public async Task Delete_Twice_SecondCall_ReturnsFalse()
        {
            var boardId = await TestDataFactory.CreateBoardAsync(ctx, boardSize: 5);
            var first = await service.DeleteAsync(boardId);
            Assert.True(first);

            var second = await service.DeleteAsync(boardId);
            Assert.False(second);
        }

        [Fact]
        public async Task GetAllByUserId_ReturnsUserBoards_And_InvalidUser_Throws()
        {
            
            var userId = await CreateUserAsync(); 
            await TestDataFactory.CreateBoardAsync(ctx, boardSize: 5, userId: userId);
            await TestDataFactory.CreateBoardAsync(ctx, boardSize: 5, userId: userId);

            var result = await service.getAllByUserIdAsync(userId, null);

            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count);

         
            await Assert.ThrowsAsync<InvalidRequestException>(() => service.getAllByUserIdAsync(null, null));
            await Assert.ThrowsAsync<InvalidRequestException>(() => service.getAllByUserIdAsync(string.Empty, null));
        }
    }
}