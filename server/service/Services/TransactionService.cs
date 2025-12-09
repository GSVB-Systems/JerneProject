using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Contracts.TransactionDTOs;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using Service.Repositories;
using service.Mappers;
using service.Services.Interfaces;
using Sieve.Services;

namespace service.Services;

public class TransactionService : Service<Transaction>, ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ISieveProcessor _sieveProcessor;

    public TransactionService(ITransactionRepository transactionRepository, ISieveProcessor sieveProcessor) : base(transactionRepository)
    {
        _transactionRepository = transactionRepository;
        _sieveProcessor = sieveProcessor;
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

    public async Task<PagedResult<TransactionDto>> getAllByUserIdAsync(string userId, TransactionQueryParameters? parameters)
    {
        var query = _transactionRepository.AsQueryable().Where(t => t.UserID == userId);
        var sieveModel = parameters ?? new TransactionQueryParameters();
        var totalCount = await query.CountAsync();
        var processedQuery = _sieveProcessor.Apply(sieveModel, query);
        var transactions = await processedQuery.ToListAsync();

        return new PagedResult<TransactionDto>
        {
            Items = transactions.Select(TransactionMapper.ToDto).ToList(), // toList quickfix until getAllAsync is fixed
            TotalCount = totalCount,
            Page = sieveModel.Page ?? 1,
            PageSize = sieveModel.PageSize ?? transactions.Count
        };
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await base.DeleteAsync(id);
    }
}