using Contracts.BoardDTOs;
using Contracts.TransactionDTOs;

namespace service.Rules.RuleInterfaces;

public interface IPurchaseRules
{
    Task ValidateProcessPurchaseAsync(CreateBoardDto boardDto, CreateTransactionDto transactionDto);
}

