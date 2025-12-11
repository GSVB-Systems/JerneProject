using Sieve.Attributes;

namespace dataaccess.Entities;

public class Transaction
{
    public String TransactionID { get; set; }
    
    [Sieve(CanSort = true, CanFilter = true)]
    public String TransactionString { get; set; }
    
    [Sieve(CanSort = true, CanFilter = true)]
    public DateTime TransactionDate { get; set; }
    
    [Sieve(CanSort = true, CanFilter = true)]
    public Decimal Amount { get; set; }
    public String UserID { get; set; }
}