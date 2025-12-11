using System;

namespace Contracts.TransactionDTOs;

public class CreateTransactionDto
{
    public string TransactionString { get; set; }
    public DateTime? TransactionDate { get; set; } 
    public decimal Amount { get; set; }
    public string UserID { get; set; }
    public bool? Pending { get; set; }
}