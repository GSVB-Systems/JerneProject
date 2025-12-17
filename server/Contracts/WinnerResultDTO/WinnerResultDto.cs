using Contracts.BoardDTOs;
using Contracts.UserDTOs;

namespace Contracts.WinnerResultDTO
{
    public class WinnerResultDto
    {
        public BoardDto Board { get; set; } = null!;
        public UserDto User { get; set; } = null!;
    }
}