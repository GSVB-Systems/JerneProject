using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.TransactionDTOs;
using dataaccess.Entities;
using Service.Repositories;
using service.Mappers;
using service.Services.Interfaces;

namespace service.Services;

public class TransactionService : Service<Transaction>, ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(ITransactionRepository transactionRepository) : base(transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<TransactionDto>> GetAllAsync()
    {
        var transactions = await base.GetAllAsync();
        return transactions.Select(TransactionMapper.ToDto);
    }

    public async Task<TransactionDto?> GetByIdAsync(string id)
    {
        var transaction = await base.GetByIdAsync(id);
        return transaction == null ? null : TransactionMapper.ToDto(transaction);
    }

   
    public Task<TransactionDto> CreateAsync(TransactionDto entity) =>
        throw new NotSupportedException(@"restmetoder fra service interface, skal lige finde ud af hvad jeg gør");

    public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto)
    {
        var entity = TransactionMapper.ToEntity(dto);
        await _transactionRepository.AddAsync(entity);
        await _transactionRepository.SaveChangesAsync();
        return TransactionMapper.ToDto(entity);
    }

    
    public Task<TransactionDto?> UpdateAsync(string id, TransactionDto entity) =>
        throw new NotSupportedException(@"levn fra service interface, skal lige finde ud af hvad jeg gør");

    public async Task<TransactionDto?> UpdateAsync(string id, UpdateTransactionDto dto)
    {
        var existing = await base.GetByIdAsync(id);
        if (existing == null) return null;

        TransactionMapper.ApplyUpdate(existing, dto);
        await _transactionRepository.UpdateAsync(existing);
        await _transactionRepository.SaveChangesAsync();

        return TransactionMapper.ToDto(existing);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await base.DeleteAsync(id);
    }
}