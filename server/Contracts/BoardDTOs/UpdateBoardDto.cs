using System.Collections.Generic;
using Contracts.BoardNumberDTOs;

namespace Contracts.BoardDTOs
{
    public class UpdateBoardDto
    {
        public int? BoardSize { get; set; }
        public bool? IsActive { get; set; }
        public int? Week { get; set; }
        
        public List<CreateBoardNumberDto> Numbers { get; set; } = null;
    }
}