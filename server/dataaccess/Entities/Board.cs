using System.ComponentModel.DataAnnotations.Schema;
using Sieve.Attributes;

namespace dataaccess.Entities;

public class Board
{
    public String BoardID { get; set; }
    
    [Sieve(CanSort = true, CanFilter = true)]
    public int BoardSize  { get; set; }
    
    [Sieve(CanSort = true, CanFilter = true)]
    public bool IsActive { get; set; }
    
    [Sieve(CanSort = true, CanFilter = true)]
    public int Week { get; set; }
    
    [Sieve(CanSort = true, CanFilter = true)]
    public DateTime CreatedAt { get; set; }
    
    [Sieve(CanSort = true, CanFilter = true)]
    public int Year { get; set; }

    [Sieve(CanSort = true, CanFilter = true)]
    public int WeeksPurchased { get; set; }
    
    public String UserID { get; set; }
    
    public ICollection<BoardNumber> Numbers { get; set; }
    
    [ForeignKey(nameof(UserID))]
    
    [Sieve(CanSort = true, CanFilter = true)]
    public User? User { get; set; }
    
    [Sieve(CanSort = true, CanFilter = true)]
    public bool Win { get; set; }
}