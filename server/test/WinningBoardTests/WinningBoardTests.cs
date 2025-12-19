using Microsoft.EntityFrameworkCore;
using dataaccess;
using dataaccess.Entities;
using service.Repositories;
using test;

namespace WinningBoardTests
{
    public class WinningBoardRepositoryTests
    {
        private AppDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Add_WinningBoard_And_SaveChanges()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var ctx = CreateContext(dbName);
            var repo = new WinningBoardRepository(ctx);

            var board = new WinningBoard
            {
                WinningBoardID = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                Week = 1,
                WeekYear = 2025,
                WinningNumbers = new System.Collections.Generic.List<WinningNumber>()
            };

            await repo.AddAsync(board);
            await repo.SaveChangesAsync(); // SaveChangesAsync returns Task, not Task<int>

            var fetched = await ctx.Set<WinningBoard>().FindAsync(board.WinningBoardID);
            Assert.NotNull(fetched);
            Assert.Equal(board.WinningBoardID, fetched.WinningBoardID);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Inserted_WinningBoard()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var ctx = CreateContext(dbName);

            var id = await TestDataFactory.CreateWinningBoardAsync(ctx);
            var repo = new WinningBoardRepository(ctx);

            var fetched = await repo.GetByIdAsync(id);

            Assert.NotNull(fetched);
            Assert.Equal(id, fetched.WinningBoardID);
        }

        [Fact]
        public async Task AsQueryable_Should_Return_All_WinningBoards()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var ctx = CreateContext(dbName);

            var id1 = await TestDataFactory.CreateWinningBoardAsync(ctx);
            var id2 = await TestDataFactory.CreateWinningBoardAsync(ctx);
            var repo = new WinningBoardRepository(ctx);

            var query = repo.AsQueryable();
            var count = query.Count();

            Assert.Equal(2, count);
            var ids = query.Select(w => w.WinningBoardID).ToList();
            Assert.Contains(id1, ids);
            Assert.Contains(id2, ids);
        }

        [Fact]
        public async Task UpdateAsync_Should_Persist_Changes()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var ctx = CreateContext(dbName);

            var id = await TestDataFactory.CreateWinningBoardAsync(ctx, week: 5);
            var repo = new WinningBoardRepository(ctx);

            var existing = await repo.GetByIdAsync(id);
            Assert.NotNull(existing);

            existing.Week = 99;
            await repo.UpdateAsync(existing);
            await repo.SaveChangesAsync();

            var fetched = await ctx.Set<WinningBoard>().FindAsync(id);
            Assert.NotNull(fetched);
            Assert.Equal(99, fetched.Week);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_WinningBoard()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var ctx = CreateContext(dbName);

            var id = await TestDataFactory.CreateWinningBoardAsync(ctx);
            var repo = new WinningBoardRepository(ctx);

            var existing = await repo.GetByIdAsync(id);
            Assert.NotNull(existing);

            await repo.DeleteAsync(existing);
            await repo.SaveChangesAsync();

            var fetched = await ctx.Set<WinningBoard>().FindAsync(id);
            Assert.Null(fetched);
        }
    }
}