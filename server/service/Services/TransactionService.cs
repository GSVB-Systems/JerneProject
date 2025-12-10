using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Contracts;
using Contracts.TransactionDTOs;
using dataaccess.Entities;
using service.Mappers;
using service.Repositories.Interfaces;
using service.Services.Interfaces;
using Sieve.Models;
using Sieve.Services;
using Microsoft.EntityFrameworkCore;

namespace service.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ISieveProcessor _sieveProcessor;

        public TransactionService(ITransactionRepository transactionRepository, ISieveProcessor sieveProcessor)
        {
            _transactionRepository = transactionRepository;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<TransactionDto?> GetByIdAsync(string id)
        {
            var entity = await _transactionRepository.GetByIdAsync(id);
            return entity == null ? null : TransactionMapper.ToDto(entity);
        }

        public async Task<PagedResult<TransactionDto>> GetAllAsync(SieveModel? parameters)
        {
            var query = _transactionRepository.AsQueryable();
            var sieveModel = parameters ?? new SieveModel();

            var totalCount = await query.CountAsync();
            var processedQuery = _sieveProcessor.Apply(sieveModel, query);
            var entities = await processedQuery.ToListAsync();

            return new PagedResult<TransactionDto>
            {
                Items = entities.Select(TransactionMapper.ToDto).ToList(),
                TotalCount = totalCount,
                Page = sieveModel.Page ?? 1,
                PageSize = sieveModel.PageSize ?? entities.Count
            };
        }

        public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto)
        {
            var entity = TransactionMapper.ToEntity(dto);
            await _transactionRepository.AddAsync(entity);
            await _transactionRepository.SaveChangesAsync();
            return TransactionMapper.ToDto(entity);
        }

        public async Task<TransactionDto?> UpdateAsync(string id, UpdateTransactionDto dto)
        {
            var existing = await _transactionRepository.GetByIdAsync(id);
            if (existing == null) return null;

            TransactionMapper.ApplyUpdate(existing, dto);
            await _transactionRepository.UpdateAsync(existing);
            await _transactionRepository.SaveChangesAsync();

            return TransactionMapper.ToDto(existing);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var existing = await _transactionRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _transactionRepository.DeleteAsync(existing);
            await _transactionRepository.SaveChangesAsync();
            return true;
        }
    }
}