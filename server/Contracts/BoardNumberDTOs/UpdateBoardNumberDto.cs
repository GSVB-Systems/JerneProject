using System;

namespace Contracts.BoardNumberDTOs
{
    public class UpdateBoardNumberDto
    {
        public int? Number { get; set; }
        
        public string? BoardID { get; set; }
    }
}