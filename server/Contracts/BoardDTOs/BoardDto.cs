using System;
using Contracts.BoardNumberDTOs;

namespace Contracts.BoardDTOs
{
    public class BoardDto
    {
        public string BoardID { get; set; }
        public int BoardSize { get; set; }
        public bool IsActive { get; set; }
        public int Week { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserID { get; set; }
        public List<BoardNumberDto> Numbers { get; set; } = new List<BoardNumberDto>();
        public bool Win { get; set; }
    }
}