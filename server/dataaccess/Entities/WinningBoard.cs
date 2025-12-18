using Sieve.Attributes;

namespace dataaccess.Entities;


public class WinningBoard
{
    public String WinningBoardID { get; set; }
    
    [Sieve(CanSort = true, CanFilter = true)]
    public DateTime CreatedAt { get; set; }
    
    [Sieve(CanSort = true, CanFilter = true)]
    public int Week { get; set; }
    
    [Sieve(CanSort = true, CanFilter = true)]
    public int WeekYear { get; set; }
    
    [Sieve(CanSort = true, CanFilter = true)]
    public ICollection<WinningNumber> WinningNumbers { get; set; }
}