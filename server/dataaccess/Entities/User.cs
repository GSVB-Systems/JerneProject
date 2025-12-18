using dataaccess.Entities.Enums;
using Sieve.Attributes;

namespace dataaccess.Entities;

public class User
{
    public String UserID { get; set; }
    
    [Sieve(CanFilter = true, CanSort = true)]
    public String Firstname { get; set; }
    
    [Sieve(CanFilter = true, CanSort = true)]
    public String Lastname { get; set; }
    
    [Sieve(CanFilter = true, CanSort = true)]
    public String Email { get; set; }
    public String Hash { get; set; }
    
    [Sieve(CanFilter = true, CanSort = true)]
    public UserRole Role { get; set; }
    public bool Firstlogin { get; set; }
    
    [Sieve(CanFilter = true, CanSort = true)]
    public bool IsActive { get; set; }
    
    public DateTime? SubscriptionExpiresAt { get; set; }
    
    [Sieve(CanFilter = true, CanSort = true)]
    public decimal Balance { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
    public ICollection<Board> Boards { get; set; }
}