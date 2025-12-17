using Contracts.BoardDTOs;

namespace Contracts.TransactionDTOs;

public class PurchaseDTO
{
    public CreateBoardDto Board { get; set; }
    public CreateTransactionDto Transaction { get; set; }
    
}