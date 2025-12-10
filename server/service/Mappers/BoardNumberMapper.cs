using System;
using Contracts.BoardNumberDTOs;
using dataaccess.Entities;

namespace service.Mappers
{
    public static class BoardNumberMapper
    {
        public static BoardNumberDto ToDto(BoardNumber n) =>
            n == null ? null : new BoardNumberDto
            {
                BoardNumberID = n.BoardNumberID,
                BoardID = n.BoardID,
                WinningBoardID = n.WinningBoardID,
                Number = n.Number
            };

        public static BoardNumber ToEntity(CreateBoardNumberDto dto)
        {
            if (dto == null) return null;
            return new BoardNumber
            {
                BoardNumberID = Guid.NewGuid().ToString(),
                Number = dto.Number,
                WinningBoardID = dto.WinningBoardID
            };
        }

        public static void ApplyUpdate(BoardNumber target, UpdateBoardNumberDto dto)
        {
            if (dto == null || target == null) return;
            if (dto.Number.HasValue) target.Number = dto.Number.Value;
            if (dto.WinningBoardID != null) target.WinningBoardID = dto.WinningBoardID;
        }
    }
}