using Contracts.WinningNumberDTOs;

using System.Collections.Generic;

namespace Contracts.WinningBoardDTOs
{
    public class CreateWinningBoardDto
    {
        public List<CreateWinningNumberDto> WinningNumbers { get; set; } = new List<CreateWinningNumberDto>();
    }
}