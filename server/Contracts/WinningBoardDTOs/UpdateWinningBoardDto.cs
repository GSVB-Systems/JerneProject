using System.Collections.Generic;
using Contracts.WinningNumberDTOs;

namespace Contracts.WinningBoardDTOs
{
    public class UpdateWinningBoardDto
    {
        public List<CreateWinningNumberDto> WinningNumbers { get; set; }
    }
}