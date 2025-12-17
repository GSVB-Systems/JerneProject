using Contracts.BoardDTOs;
using Contracts.TransactionDTOs;

namespace service.Services.Interfaces;

public interface IPurchaseService
{
    Task<bool> ProcessPurchaseAsync(CreateBoardDto board, CreateTransactionDto transaction);
}