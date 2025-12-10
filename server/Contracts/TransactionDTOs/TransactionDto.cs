using System;

namespace Contracts.TransactionDTOs;

public class TransactionDto
{
    public string TransactionID { get; set; }
    public string TransactionString { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public string UserID { get; set; }
    
}