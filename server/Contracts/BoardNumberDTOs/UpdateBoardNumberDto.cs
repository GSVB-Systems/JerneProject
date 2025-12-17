using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.BoardNumberDTOs
{
    public class UpdateBoardNumberDto
    {
        [Range(1,16)]
        public int? Number { get; set; }
        
        [MinLength(1)]
        public string? BoardID { get; set; }
    }
}