using dataaccess;
using dataaccess.Entities;
using dataaccess.Entities.Enums;

namespace test
{
    public static class TestDataFactory
    {
        public static async Task<string> CreateBoardAsync(AppDbContext ctx, string? boardId = null, int boardSize = 1, string? userId = null)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            boardId ??= Guid.NewGuid().ToString();

            
            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = Guid.NewGuid().ToString();

                
                ctx.Set<User>().Add(new User
                {
                    UserID = userId,
                    Firstname = "Test",
                    Lastname = "User",
                    Email = $"{userId}@example.local",
                    Hash = "test-hash",
                    Role = (UserRole)0,
                    Firstlogin = false,
                    IsActive = true,
                    Balance = 0m,
                    Transactions = new List<Transaction>(),
                    Boards = new List<Board>()
                });
            }

            var board = new Board
            {
                BoardID = boardId,
                BoardSize = boardSize,
                IsActive = false,
                Week = 0,
                CreatedAt = DateTime.UtcNow,
                Year = DateTime.UtcNow.Year,
                WeeksPurchased = 0,
                UserID = userId,
                Win = false,
                Numbers = new List<BoardNumber>()
            };

            ctx.Set<Board>().Add(board);
            await ctx.SaveChangesAsync();
            return boardId;
        }

        public static async Task<string> CreateBoardNumberAsync(AppDbContext ctx, string boardId, int number)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (string.IsNullOrWhiteSpace(boardId)) throw new ArgumentException("boardId must be provided", nameof(boardId));

            var id = Guid.NewGuid().ToString();
            var bn = new BoardNumber
            {
                BoardNumberID = id,
                Number = number,
                BoardID = boardId
            };

            ctx.Set<BoardNumber>().Add(bn);
            await ctx.SaveChangesAsync();
            return id;
        }
    }
}