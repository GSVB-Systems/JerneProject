using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Contracts;
using dataaccess.Entities;
using Contracts.WinningBoardDTOs;
using service.Mappers;
using service.Repositories.Interfaces;
using service.Services.Interfaces;
using Sieve.Models;
using Sieve.Services;
using Microsoft.EntityFrameworkCore;

namespace service.Services
{
    public class WinningBoardService : IWinningBoardService
    {
        private readonly IWinningboardRepository _repository;
        private readonly ISieveProcessor _sieveProcessor;

        public WinningBoardService(IWinningboardRepository repository, ISieveProcessor sieveProcessor)
        {
            _repository = repository;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<WinningBoardDto?> GetByIdAsync(string id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : WinningBoardMapper.ToDto(entity);
        }

        public async Task<PagedResult<WinningBoardDto>> GetAllAsync(SieveModel? parameters)
        {
            var query = _repository.AsQueryable();
            var sieveModel = parameters ?? new SieveModel();

            var totalCount = await query.CountAsync();
            var processedQuery = _sieveProcessor.Apply(sieveModel, query);
            var entities = await processedQuery.ToListAsync();

            return new PagedResult<WinningBoardDto>
            {
                Items = entities.Select(WinningBoardMapper.ToDto).ToList(),
                TotalCount = totalCount,
                Page = sieveModel.Page ?? 1,
                PageSize = sieveModel.PageSize ?? entities.Count
            };
        }

        public async Task<WinningBoardDto?> CreateAsync(CreateWinningBoardDto dto)
        {
            var entity = WinningBoardMapper.ToEntity(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return WinningBoardMapper.ToDto(entity);
        }

        public async Task<WinningBoardDto?> UpdateAsync(string id, UpdateWinningBoardDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            WinningBoardMapper.ApplyUpdate(existing, dto);
            await _repository.UpdateAsync(existing);
            await _repository.SaveChangesAsync();

            return WinningBoardMapper.ToDto(existing);
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