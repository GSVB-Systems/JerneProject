
using System.ComponentModel.DataAnnotations;
using Contracts;
using Contracts.WinningBoardDTOs;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using service.Repositories.Interfaces;
using service.Services.Interfaces;
using service.Mappers;
using Sieve.Models;

namespace service.Services
{
    public class WinningBoardService : Service<WinningBoard, CreateWinningBoardDto, UpdateWinningBoardDto>, IWinningBoardService
    {
        private readonly IRepository<WinningBoard> _winningBoardRepo;

        public WinningBoardService(IRepository<WinningBoard> winningBoardRepo)
            : base(winningBoardRepo)
        {
            _winningBoardRepo = winningBoardRepo;
        }

        public async Task<WinningBoardDto?> GetByIdAsync(string id)
        {
            var entity = await _winningBoardRepo.AsQueryable()
                .Include(w => w.WinningNumbers)
                .FirstOrDefaultAsync(w => w.WinningBoardID == id);

            return entity == null ? null : WinningBoardMapper.ToDto(entity);
        }

        public async Task<PagedResult<WinningBoardDto>> GetAllAsync(SieveModel? parameters)
        {
          
            var query = _winningBoardRepo.AsQueryable().Include(w => w.WinningNumbers);

            var items = await query.ToListAsync();

            var dtoItems = items.Select(WinningBoardMapper.ToDto).ToList();
            var count = dtoItems.Count;

            var page = parameters?.Page ?? 1;
            var pageSize = parameters?.PageSize ?? count;

            return new PagedResult<WinningBoardDto>
            {
                Items = dtoItems,
                TotalCount = count,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<WinningBoardDto> CreateAsync(CreateWinningBoardDto createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            if (createDto.WinningNumbers == null)
                throw new ValidationException("WinningNumbers are required.");

           //checker om der er duplikater af tal på samme board
            var duplicateValues = createDto.WinningNumbers
                .GroupBy(n => n)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateValues.Any())
                throw new ValidationException($"WinningNumbers must be unique within a winning board. Duplicates: {string.Join(',', duplicateValues)}");

            //sikrer at vi kun har 3 eller 5 tal på et WinningBoard
            var count = createDto.WinningNumbers.Count;
            if (count != 3 && count != 5)
                throw new ValidationException("WinningNumbers must contain exactly 3 or 5 items.");

            var entity = WinningBoardMapper.ToEntity(createDto);

            
            if (string.IsNullOrWhiteSpace(entity.WinningBoardID))
                entity.WinningBoardID = Guid.NewGuid().ToString();

            if (entity.WinningNumbers == null)
                entity.WinningNumbers = new List<WinningNumber>();

            foreach (var wn in entity.WinningNumbers)
            {
                if (string.IsNullOrWhiteSpace(wn.WinningNumberID))
                    wn.WinningNumberID = Guid.NewGuid().ToString();
                wn.WinningBoardID = entity.WinningBoardID;
            }

            //checker efter mapping om der er duplikater af tal på samme board
            var dupAfterMap = entity.WinningNumbers.GroupBy(w => w.Number).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (dupAfterMap.Any())
                throw new ValidationException($"WinningNumbers must be unique within a winning board. Duplicates: {string.Join(',', dupAfterMap)}");

            await _winningBoardRepo.AddAsync(entity);
            await _winningBoardRepo.SaveChangesAsync();

            return WinningBoardMapper.ToDto(entity);
        }

        public async Task<WinningBoardDto?> UpdateAsync(string id, UpdateWinningBoardDto updateDto)
        {
            var existing = await base.GetByIdAsync(id);
            if (existing == null) return null;

            
            if (updateDto?.WinningNumbers == null)
                throw new ValidationException("WinningNumbers are required.");

            var count = updateDto.WinningNumbers.Count;
            if (count != 3 && count != 5)
                throw new ValidationException("WinningNumbers must contain exactly 3 or 5 items.");

            if (updateDto.WinningNumbers.Count != updateDto.WinningNumbers.Distinct().Count())
                throw new ValidationException("WinningNumbers must be unique.");

            WinningBoardMapper.ApplyUpdate(existing, updateDto);

            await _winningBoardRepo.UpdateAsync(existing);
            await _winningBoardRepo.SaveChangesAsync();

            return WinningBoardMapper.ToDto(existing);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await base.DeleteAsync(id);
        }
    }
}