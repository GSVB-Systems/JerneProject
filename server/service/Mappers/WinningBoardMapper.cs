using System;
using System.Linq;
using System.Collections.Generic;
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