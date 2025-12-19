using Contracts.TransactionDTOs;
using Sieve.Models;

namespace service.Rules.RuleInterfaces;

public interface ITransactionRules
{
    Task ValidateGetByIdAsync(string id);
    Task ValidateGetAllAsync(SieveModel? parameters);
    Task ValidateGetAllByUserIdAsync(string userId, TransactionQueryParameters? parameters);
    Task ValidateCreateAsync(CreateTransactionDto dto);
    Task ValidateUpdateAsync(string id, UpdateTransactionDto dto);
    Task ValidateDeleteAsync(string id);
}

