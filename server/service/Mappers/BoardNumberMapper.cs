using System;
using Contracts.BoardNumberDTOs;
using dataaccess.Entities;

namespace service.Mappers
{
    public static class BoardNumberMapper
    {
        public static BoardNumberDto ToDto(BoardNumber e) =>
            e == null ? null : new BoardNumberDto
            {
                BoardNumberID = e.BoardNumberID,
                Number = e.Number,
                BoardID = e.BoardID
            };

        public static BoardNumber? ToEntity(CreateBoardNumberDto dto)
        {
            if (dto == null) return null;

            return new BoardNumber
            {
                BoardNumberID = string.Empty,
                Number = dto.Number,
                BoardID = string.Empty
            };
        }

        public static void ApplyUpdate(BoardNumber target, UpdateBoardNumberDto dto)
        {
            if (target == null || dto == null) return;

            if (dto.Number.HasValue)
                target.Number = dto.Number.Value;

            if (!string.IsNullOrWhiteSpace(dto.BoardID))
                target.BoardID = dto.BoardID;
        }
    }
}