namespace Contracts.BoardDTOs
{
    public class UpdateBoardDto
    {
        public int? BoardSize { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsRepeating { get; set; }
    }
}