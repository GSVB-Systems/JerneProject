using Contracts.WinningNumberDTOs;
using Contracts;
using dataaccess.Entities;

namespace service.Mappers
{
    public static class WinningNumberMapper
    {
        public static WinningNumberDto ToDto(WinningNumber n) =>
            n == null ? null : new WinningNumberDto
            {
                WinningNumberID = n.WinningNumberID,
                WinningBoardID = n.WinningBoardID,
                Number = n.Number
            };

        public static WinningNumber ToEntity(CreateWinningNumberDto dto)
        {
            if (dto == null) return null;
            return new WinningNumber
            {
                WinningNumberID = Guid.NewGuid().ToString(),
                Number = dto.Number
            };
        }

        public static void ApplyUpdate(WinningNumber target, UpdateWinningNumberDto dto)
        {
            if (dto == null || target == null) return;
            if (dto.Number.HasValue) target.Number = dto.Number.Value;
        }
    }
}