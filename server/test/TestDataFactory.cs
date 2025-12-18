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
        public static async Task<string> CreateWinningBoardAsync(AppDbContext ctx, string? winningBoardId = null, int week = 0, int? weekYear = null, DateTime? createdAt = null)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            winningBoardId ??= Guid.NewGuid().ToString();
            createdAt ??= DateTime.UtcNow;
            weekYear ??= createdAt.Value.Year;

            var wb = new WinningBoard
            {
                WinningBoardID = winningBoardId,
                CreatedAt = createdAt.Value,
                Week = week,
                WeekYear = weekYear.Value,
                WinningNumbers = new List<WinningNumber>()
            };

            ctx.Set<WinningBoard>().Add(wb);
            await ctx.SaveChangesAsync();
            return winningBoardId;
        }
        public static async Task<string> CreateWinningNumberAsync(AppDbContext ctx, string winningBoardId, int number, string? winningNumberId = null)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (string.IsNullOrWhiteSpace(winningBoardId)) throw new ArgumentException("winningBoardId must be provided", nameof(winningBoardId));

            
            var existingBoard = await ctx.Set<WinningBoard>().FindAsync(winningBoardId);
            if (existingBoard == null)
            {
                await CreateWinningBoardAsync(ctx, winningBoardId);
            }

            var id = winningNumberId ?? Guid.NewGuid().ToString();
            var wn = new WinningNumber
            {
                WinningNumberID = id,
                WinningBoardID = winningBoardId,
                Number = number
            };

            ctx.Set<WinningNumber>().Add(wn);
            await ctx.SaveChangesAsync();
            return id;
        }

        public static async Task<User> CreateUserAsync(AppDbContext ctx, string passwordHash, string? userId = null, string? email = null)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("passwordHash must be provided", nameof(passwordHash));

            userId ??= Guid.NewGuid().ToString();
            email ??= $"{userId}@example.local";

            var user = new User
            {
                UserID = userId,
                Firstname = "Test",
                Lastname = "User",
                Email = email,
                Hash = passwordHash,
                Role = UserRole.Bruger,
                Firstlogin = false,
                IsActive = true,
                Balance = 0m,
                Transactions = new List<Transaction>(),
                Boards = new List<Board>()
            };

            ctx.Set<User>().Add(user);
            await ctx.SaveChangesAsync();
            return user;
        }
    }
}