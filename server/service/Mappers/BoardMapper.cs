using System.Globalization;
using Contracts;
using Contracts.BoardNumberDTOs;
using Contracts.BoardDTOs;
using dataaccess.Entities;

namespace service.Mappers
{
    public static class BoardMapper
    {
        public static BoardDto ToDto(Board b) =>
            b == null ? null : new BoardDto
            {
                BoardID = b.BoardID,
                BoardSize = b.BoardSize,
                IsActive = b.IsActive,
                Week = b.Week,
                Year = b.Year,
                CreatedAt = b.CreatedAt,
                UserID = b.UserID,
                Win = b.Win,
                WeeksPurchased = b.WeeksPurchased,
                Numbers = b.Numbers?.Select(BoardNumberMapper.ToDto).ToList() ?? new System.Collections.Generic.List<BoardNumberDto>()
            };

        public static Board ToEntity(CreateBoardDto dto)
        {
            if (dto == null) return null;
            var boardId = Guid.NewGuid().ToString();
            var nowUtc = DateTime.UtcNow;
            var nowLocal = DateTime.Now;
            var cal = CultureInfo.CurrentCulture.Calendar;
            var weekOfYear = cal.GetWeekOfYear(nowLocal, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            var year = nowLocal.Year;
            var board = new Board
            {
                BoardID = boardId,
                BoardSize = dto.BoardSize,
                Week = weekOfYear,
                IsActive = true,
                CreatedAt = nowUtc,
                Year = year,
                WeeksPurchased = 1,
                UserID = dto.UserID,
                Numbers = dto.Numbers?.Select(n => new BoardNumber
                {
                    BoardNumberID = Guid.NewGuid().ToString(),
                    Number = n,
                    BoardID = boardId
                }).ToList() ?? new System.Collections.Generic.List<BoardNumber>()
            };
            return board;
        }

        public static IReadOnlyCollection<Board> ToWeeklyEntities(CreateBoardDto dto)
        {
            if (dto?.Week <= 0)
                return Array.Empty<Board>();

            var cal = CultureInfo.CurrentCulture.Calendar;
            var baseUtc = DateTime.UtcNow;
            var baseLocal = DateTime.Now;
            var currentWeek = cal.GetWeekOfYear(baseLocal, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            var currentYear = baseLocal.Year;

            var boards = new List<Board>(dto.Week);
            for (var offset = 0; offset < dto.Week; offset++)
            {
                if (offset > 0)
                {
                    currentWeek++;
                    if (currentWeek > 52)
                    {
                        currentWeek = 1;
                        currentYear++;
                    }
                }
                var boardId = Guid.NewGuid().ToString();

                var board = new Board
                {
                    BoardID = boardId,
                    BoardSize = dto.BoardSize,
                    IsActive = true,
                    Week = currentWeek,
                    Year = currentYear,
                    CreatedAt = baseUtc,
                    WeeksPurchased = offset + 1,
                    UserID = dto.UserID,
                    Numbers = dto.Numbers?.Select(n => new BoardNumber
                    {
                        BoardNumberID = Guid.NewGuid().ToString(),
                        Number = n,
                        BoardID = boardId
                    }).ToList() ?? new System.Collections.Generic.List<BoardNumber>()
                };

                boards.Add(board);
            }

            return boards;
        }

        public static void ApplyUpdate(Board target, UpdateBoardDto dto)
        {
            if (dto == null || target == null) return;

            if (dto.BoardSize.HasValue) target.BoardSize = dto.BoardSize.Value;
            if (dto.IsActive.HasValue) target.IsActive = dto.IsActive.Value;
            if (dto.Week.HasValue) target.Week = dto.Week.Value;
            
            if (dto.Numbers != null)
            {
                var boardId = target.BoardID;
                target.Numbers = dto.Numbers.Select(n => new BoardNumber
                {
                    BoardNumberID = Guid.NewGuid().ToString(),
                    Number = n.Number,   
                    BoardID = boardId
                }).ToList();
            }
        }
    }
}