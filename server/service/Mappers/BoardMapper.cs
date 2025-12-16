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
                CreatedAt = b.CreatedAt,
                UserID = b.UserID,
                Numbers = b.Numbers?.Select(BoardNumberMapper.ToDto).ToList() ?? new System.Collections.Generic.List<BoardNumberDto>()
            };

        public static Board ToEntity(CreateBoardDto dto)
        {
            if (dto == null) return null;
            var boardId = Guid.NewGuid().ToString();
            var board = new Board
            {
                BoardID = boardId,
                BoardSize = dto.BoardSize,
                Week = dto.Week,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
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