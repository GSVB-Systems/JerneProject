using Contracts;
using Contracts.TransactionDTOs;
using service.Mappers;
using service.Repositories.Interfaces;
using service.Services.Interfaces;
using Sieve.Models;
using Sieve.Services;
using Microsoft.EntityFrameworkCore;
using service.Rules.RuleInterfaces;

namespace service.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly IUserService _userService;
        private readonly ITransactionRules _transactionRules;

        public TransactionService(
            ITransactionRepository transactionRepository,
            ISieveProcessor sieveProcessor,
            IUserService userService,
            ITransactionRules transactionRules)
        {
            _transactionRepository = transactionRepository;
            _sieveProcessor = sieveProcessor;
            _userService = userService;
            _transactionRules = transactionRules ?? throw new ArgumentNullException(nameof(transactionRules));
        }

        public async Task<TransactionDto?> GetByIdAsync(string id)
        {
            await _transactionRules.ValidateGetByIdAsync(id);

            var entity = await _transactionRepository.GetByIdAsync(id);
            return entity == null ? null : TransactionMapper.ToDto(entity);
        }

        public async Task<PagedResult<TransactionDto>> GetAllAsync(SieveModel? parameters)
        {
            var sieveModel = parameters ?? new SieveModel();
            await _transactionRules.ValidateGetAllAsync(sieveModel);

            var query = _transactionRepository.AsQueryable();

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
            await _transactionRules.ValidateCreateAsync(dto);

            var entity = TransactionMapper.ToEntity(dto);
            
            if(entity.TransactionString == "GUID")
            {
                entity.TransactionString = Guid.NewGuid().ToString();
            }

            await _transactionRepository.AddAsync(entity);
            await _transactionRepository.SaveChangesAsync();

           
            await _userService.UpdateBalanceAsync(entity.UserID);

            return TransactionMapper.ToDto(entity);
        }

        public async Task<TransactionDto?> UpdateAsync(string id, UpdateTransactionDto dto)
        {
            await _transactionRules.ValidateUpdateAsync(id, dto);

            var existing = await _transactionRepository.GetByIdAsync(id);
            if (existing == null) return null;

            TransactionMapper.ApplyUpdate(existing, dto);
            await _transactionRepository.UpdateAsync(existing);
            await _transactionRepository.SaveChangesAsync();

            await _userService.UpdateBalanceAsync(existing.UserID);
            
            return TransactionMapper.ToDto(existing);
        }

        public async Task<PagedResult<TransactionDto>> getAllByUserIdAsync(string userId,
            TransactionQueryParameters? parameters)
        {
            await _transactionRules.ValidateGetAllByUserIdAsync(userId, parameters);

            var query = _transactionRepository.AsQueryable().Where(t => t.UserID == userId);
            var sieveModel = parameters ?? new TransactionQueryParameters();
            var totalCount = await query.CountAsync();
            var processedQuery = _sieveProcessor.Apply(sieveModel, query);
            var transactions = await processedQuery.ToListAsync();

            return new PagedResult<TransactionDto>
            {
                Items = transactions.Select(TransactionMapper.ToDto)
                    .ToList(), 
                TotalCount = totalCount,
                Page = sieveModel.Page ?? 1,
                PageSize = sieveModel.PageSize ?? transactions.Count
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            await _transactionRules.ValidateDeleteAsync(id);

            var existing = await _transactionRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _transactionRepository.DeleteAsync(existing);
            await _transactionRepository.SaveChangesAsync();
            return true;
        }
    }
}