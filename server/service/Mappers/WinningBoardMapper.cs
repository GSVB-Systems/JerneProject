using System.Globalization;
using Contracts.WinningBoardDTOs;
using Contracts.WinningNumberDTOs;
using dataaccess.Entities;

namespace service.Mappers
{
    public static class WinningBoardMapper
    {
        public static WinningBoardDto ToDto(WinningBoard entity)
        {
            return new WinningBoardDto
            {
                WinningBoardID = entity.WinningBoardID,
                CreatedAt = entity.CreatedAt,
                Week = entity.Week,
                WeekYear = entity.WeekYear,
                WinningNumbers = entity.WinningNumbers?.Select(WinningNumberMapper.ToDto).ToList() ?? new List<WinningNumberDto>()
            };
        }

        public static WinningBoard ToEntity(CreateWinningBoardDto dto)
        {
            var board = new WinningBoard
            {
                WinningBoardID = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                WinningNumbers = dto.WinningNumbers.Select(n => new WinningNumber
                {
                    WinningNumberID = Guid.NewGuid().ToString(),
                    Number = n,
                    WinningBoardID = string.Empty
                }).ToList()
            };

            foreach (var wn in board.WinningNumbers)
                wn.WinningBoardID = board.WinningBoardID;

            var localNow = DateTime.Now;
            var cal = CultureInfo.CurrentCulture.Calendar;
            board.Week = cal.GetWeekOfYear(localNow, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            board.WeekYear = localNow.Year;

            return board;
        }

        public static void ApplyUpdate(WinningBoard existing, UpdateWinningBoardDto dto)
        {
            existing.WinningNumbers = dto.WinningNumbers.Select(n => new WinningNumber
            {
                WinningNumberID = Guid.NewGuid().ToString(),
                Number = n,
                WinningBoardID = existing.WinningBoardID
            }).ToList();
        }
    }
}