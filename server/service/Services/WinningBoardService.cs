using System.ComponentModel.DataAnnotations;
using Contracts;
using Contracts.WinningBoardDTOs;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using service.Repositories.Interfaces;
using service.Services.Interfaces;
using service.Mappers;
using service.Rules.RuleInterfaces;
using Sieve.Models;
using Sieve.Services;

namespace service.Services
{
    public class WinningBoardService : IWinningBoardService
    {
        private readonly IWinningboardRepository _winningBoardRepository;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly IBoardMatcherService _boardMatcherService;
        private readonly IWinningBoardRules _winningBoardRules;

        public WinningBoardService(IWinningboardRepository winningBoardRepository, ISieveProcessor sieveProcessor, IBoardMatcherService boardMatcherService, IWinningBoardRules winningBoardRules)
        {
            _winningBoardRepository = winningBoardRepository;
            _sieveProcessor = sieveProcessor;
            _boardMatcherService = boardMatcherService;
            _winningBoardRules = winningBoardRules;
        }

        public async Task<WinningBoardDto?> GetByIdAsync(string id)
        {
            _winningBoardRules.ValidateGetByIdAsync(id);
            
            var entity = await _winningBoardRepository.AsQueryable()
                .Include(w => w.WinningNumbers)
                .FirstOrDefaultAsync(w => w.WinningBoardID == id);

            return entity == null ? null : WinningBoardMapper.ToDto(entity);
        }

        public async Task<PagedResult<WinningBoardDto>> GetAllAsync(SieveModel? parameters)
        {

            await _winningBoardRules.ValidateGetAllAsync(parameters);
            var query = _winningBoardRepository.AsQueryable().Include(w => w.WinningNumbers);
            var sieveModel = parameters ?? new SieveModel();
            var totalCount = await query.CountAsync();
            var processedQuery = _sieveProcessor.Apply(sieveModel, query);
            var items = await processedQuery.ToListAsync();


            var dtoItems = items.Select(WinningBoardMapper.ToDto).ToList();

            return new PagedResult<WinningBoardDto>
            {
                Items = dtoItems,
                TotalCount = totalCount,
                Page = sieveModel.Page ?? 1,
                PageSize = sieveModel.PageSize ?? dtoItems.Count
            };
        }

        public async Task<WinningBoardDto> CreateAsync(CreateWinningBoardDto createDto)
        {
            await _winningBoardRules.ValidateCreateAsync(createDto);
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            if (createDto.WinningNumbers == null)
                throw new ValidationException("Udfyld felterne med de trukkede numre.");

           //checker om der er duplikater af tal på samme board
            var duplicateValues = createDto.WinningNumbers
                .GroupBy(n => n)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateValues.Any())
                throw new ValidationException($"De trukkede numre skal være unikke. Dublikater: {string.Join(',', duplicateValues)}");

            //sikrer at vi kun har 3 eller 5 tal på et WinningBoard
            var count = createDto.WinningNumbers.Count;
            if (count != 3 && count != 5)
                throw new ValidationException("Der må kune trækkes endten 3 eller 5 numre.");

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
                throw new ValidationException($"De trukkede numre skal være unikke. Dublikater: {string.Join(',', dupAfterMap)}");

            var alreadyExists = await _winningBoardRepository.AsQueryable()
                .AnyAsync(w => w.Week == entity.Week && w.WeekYear == entity.WeekYear);
            if (alreadyExists)
                throw new ValidationException($"Der findes allerede et vindende board for uge {entity.Week}, {entity.WeekYear}.");
            
            await _winningBoardRepository.AddAsync(entity);
            await _winningBoardRepository.SaveChangesAsync();

            await _boardMatcherService.GetBoardsContainingNumbersWithDecrementerAsync(entity.WinningBoardID);

            return WinningBoardMapper.ToDto(entity);
        }

        public async Task<WinningBoardDto?> UpdateAsync(string id, UpdateWinningBoardDto updateDto)
        {
            _winningBoardRules.ValidateUpdateAsync(id, updateDto);
            var existing = await _winningBoardRepository.GetByIdAsync(id);
            if (existing == null) return null;

            
            if (updateDto?.WinningNumbers == null)
                throw new ValidationException("Udfyld felterne med de trukkede numre.");

            var count = updateDto.WinningNumbers.Count;
            if (count != 3 && count != 5)
                throw new ValidationException("Der må kune trækkes endten 3 eller 5 numre.");

            if (updateDto.WinningNumbers.Count != updateDto.WinningNumbers.Distinct().Count())
                throw new ValidationException("De trukkede numre skal være unikke.");

            WinningBoardMapper.ApplyUpdate(existing, updateDto);

            await _winningBoardRepository.UpdateAsync(existing);
            await _winningBoardRepository.SaveChangesAsync();

            return WinningBoardMapper.ToDto(existing);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            _winningBoardRules.ValidateDeleteAsync(id);
            var existing = await _winningBoardRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _winningBoardRepository.DeleteAsync(existing);
            await _winningBoardRepository.SaveChangesAsync();
            return true;
        }
    }
}