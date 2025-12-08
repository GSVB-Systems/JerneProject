namespace dataaccess.Entities;

public class WinningNumber
{
    public String WinningNumberID { get; set; }
    public String WinningBoardID { get; set; }
    public WinningBoard WinningBoard { get; set; }
    public int Number { get; set; }
}