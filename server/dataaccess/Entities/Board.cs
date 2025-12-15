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
    public bool IsRepeating { get; set; }
    
    [Sieve(CanSort = true, CanFilter = true)]
    public DateTime CreatedAt { get; set; }
    
    public String UserID { get; set; }
    
    public ICollection<BoardNumber> Numbers { get; set; }
    
    [ForeignKey(nameof(UserID))]
    
    [Sieve(CanSort = true, CanFilter = true)]
    public User? User { get; set; }
}