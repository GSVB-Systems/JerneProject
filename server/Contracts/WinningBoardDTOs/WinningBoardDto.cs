using System;
using System.Collections.Generic;
using Contracts.WinningNumberDTOs;

namespace Contracts.WinningBoardDTOs
{
    public class WinningBoardDto
    {
        public string WinningBoardID { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<WinningNumberDto> WinningNumbers { get; set; } = new List<WinningNumberDto>();
    }
}