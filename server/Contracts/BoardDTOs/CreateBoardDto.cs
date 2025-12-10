using System.Collections.Generic;
using Contracts.BoardNumberDTOs;

namespace Contracts.BoardDTOs
{
    public class CreateBoardDto
    {
        public int BoardSize { get; set; }
        public bool IsRepeating { get; set; }
        public string UserID { get; set; }
        public List<CreateBoardNumberDto> Numbers { get; set; } = new List<CreateBoardNumberDto>();
    }
}