using Contracts;
using Contracts.WinningBoardDTOs;
using Contracts.WinningNumberDTOs;
using dataaccess.Entities;

namespace service.Mappers
{
    public static class WinningBoardMapper
    {
        public static WinningBoardDto ToDto(WinningBoard w) =>
            w == null ? null : new WinningBoardDto
            {
                WinningBoardID = w.WinningBoardID,
                CreatedAt = w.CreatedAt,
                WinningNumbers = w.WinningNumbers?.Select(WinningNumberMapper.ToDto).ToList() ?? new List<WinningNumberDto>()
            };

        public static WinningBoard ToEntity(CreateWinningBoardDto dto)
        {
            if (dto == null) return null;
            var wbId = Guid.NewGuid().ToString();
            var wb = new WinningBoard
            {
                WinningBoardID = wbId,
                CreatedAt = DateTime.UtcNow,
                WinningNumbers = dto.WinningNumbers?.Select(n =>
                {
                    var e = WinningNumberMapper.ToEntity(n);
                    e.WinningBoardID = wbId;
                    return e;
                }).ToList() ?? new List<WinningNumber>()
            };
            return wb;
        }

        public static void ApplyUpdate(WinningBoard target, UpdateWinningBoardDto dto)
        {
            if (dto == null || target == null) return;
            // Replace winning numbers if provided (service may choose a different strategy)
            if (dto.WinningNumbers != null)
            {
                var wbId = target.WinningBoardID ?? Guid.NewGuid().ToString();
                target.WinningNumbers = dto.WinningNumbers.Select(n =>
                {
                    var e = WinningNumberMapper.ToEntity(n);
                    e.WinningBoardID = wbId;
                    return e;
                }).ToList();
            }
        }
    }
}