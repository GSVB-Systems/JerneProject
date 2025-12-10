using Contracts;
using Contracts.TransactionDTOs;
using dataaccess.Entities;

namespace service.Services.Interfaces;

public interface ITransactionService : IService<TransactionDto, CreateTransactionDto, UpdateTransactionDto>
{
    
    Task<PagedResult<TransactionDto>> getAllByUserIdAsync(string userId, TransactionQueryParameters? parameters);
}