namespace dataaccess.Entities;

public class Transaction
{
    public String TransactionID { get; set; }
    public String TransactionString { get; set; }
    public DateTime TransactionDate { get; set; }
    public Decimal Amount { get; set; }
    public String UserID { get; set; }
}