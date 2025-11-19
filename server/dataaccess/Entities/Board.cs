namespace dataaccess;

public class Board
{
    public String BoardID { get; set; }
    public int SelectedNumbers  { get; set; }
    public int BoardSize  { get; set; }
    public bool IsActive { get; set; }
    public bool IsRepeating { get; set; }
    public DateTime CreatedAt { get; set; }
    public String UserID { get; set; }
}