using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.TransactionDTOs;

public class CreateTransactionDto
{
    [StringLength(200)]
    public string? TransactionString { get; set; }

    public DateTime? TransactionDate { get; set; } 

    [Range(typeof(decimal), "-1000", "79228162514264337593543950335")]
    public decimal Amount { get; set; }
    
    public string? UserID { get; set; }

    public bool? Pending { get; set; }
}