using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Contracts.WinningNumberDTOs;
using service.Mappers;
using service.Repositories.Interfaces;
using service.Services.Interfaces;
using Sieve.Models;
using Sieve.Services;
using Microsoft.EntityFrameworkCore;

namespace service.Services
{
    public class WinningNumberService : IWinningNumberService
    {
        private readonly IWinningNumberRepository _repository;
        private readonly ISieveProcessor _sieveProcessor;

        public WinningNumberService(IWinningNumberRepository repository, ISieveProcessor sieveProcessor)
        {
            _repository = repository;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<WinningNumberDto?> GetByIdAsync(string id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : WinningNumberMapper.ToDto(entity);
        }

        public async Task<PagedResult<WinningNumberDto>> GetAllAsync(SieveModel? parameters)
        {
            var query = _repository.AsQueryable();
            var sieveModel = parameters ?? new SieveModel();

            var totalCount = await query.CountAsync();
            var processedQuery = _sieveProcessor.Apply(sieveModel, query);
            var entities = await processedQuery.ToListAsync();

            return new PagedResult<WinningNumberDto>
            {
                Items = entities.Select(WinningNumberMapper.ToDto).ToList(),
                TotalCount = totalCount,
                Page = sieveModel.Page ?? 1,
                PageSize = sieveModel.PageSize ?? entities.Count
            };
        }

        public async Task<WinningNumberDto> CreateAsync(CreateWinningNumberDto dto)
        {
            var entity = WinningNumberMapper.ToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return WinningNumberMapper.ToDto(entity)!;
        }

        public async Task<WinningNumberDto?> UpdateAsync(string id, UpdateWinningNumberDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            WinningNumberMapper.ApplyUpdate(existing, dto);
            await _repository.UpdateAsync(existing);
            await _repository.SaveChangesAsync();

            return WinningNumberMapper.ToDto(existing);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            await _repository.DeleteAsync(existing);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}