using Contracts;
using Contracts.TransactionDTOs;
using dataaccess.Entities;

namespace service.Services.Interfaces;

public interface ITransactionService : IService<TransactionDto>
{
    Task<TransactionDto> CreateAsync(CreateTransactionDto dto);
    Task<TransactionDto?> UpdateAsync(string id, UpdateTransactionDto dto);
    Task<PagedResult<TransactionDto>> getAllByUserIdAsync(string userId, TransactionQueryParameters? parameters);
}