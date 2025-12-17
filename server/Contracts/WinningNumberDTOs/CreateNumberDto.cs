using System.ComponentModel.DataAnnotations;

namespace Contracts.WinningNumberDTOs
{
    public class CreateWinningNumberDto
    {
        [Range(1,16)]
        public int Number { get; set; }
    }
}