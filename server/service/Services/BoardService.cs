using System.ComponentModel.DataAnnotations;
using Contracts.BoardDTOs;
using Contracts;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using service.Mappers;
using service.Repositories.Interfaces;
using service.Rules;
using service.Services.Interfaces;
using Sieve.Models;
using Sieve.Services;

namespace service.Services
{
    public class BoardService : IBoardService
    {
        private readonly IBoardRepository _boardRepository;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly IBoardRules _boardRules;

        public BoardService(IBoardRepository boardRepository, ISieveProcessor sieveProcessor, IBoardRules boardRules)
        {
            _boardRepository = boardRepository;
            _sieveProcessor = sieveProcessor;
            _boardRules = boardRules ?? throw new ArgumentNullException(nameof(boardRules));
        }

        public async Task<BoardDto?> GetByIdAsync(string id)
        {
            await _boardRules.ValidateGetByIdAsync(id);
            
            var entity = await _boardRepository.AsQueryable()
                .Include(b => b.Numbers)
                .FirstOrDefaultAsync(b => b.BoardID == id);

            return entity == null ? null : BoardMapper.ToDto(entity);
        }

        public async Task<PagedResult<BoardDto>> GetAllAsync(SieveModel? parameters)
        {
            await _boardRules.ValidateGetAllAsync(parameters);
            
            var query = _boardRepository.AsQueryable().Include(b => b.Numbers);
            var sieveModel = parameters ?? new SieveModel();
            var totalCount = await query.CountAsync();
            var processedQuery = _sieveProcessor.Apply(sieveModel, query);
            var items = await processedQuery.ToListAsync();

            var dtoItems = items.Select(BoardMapper.ToDto).ToList();

            return new PagedResult<BoardDto>
            {
                Items = dtoItems,
                TotalCount = totalCount,
                Page = sieveModel.Page ?? 1,
                PageSize = sieveModel.PageSize ?? dtoItems.Count
            };
        }

        public async Task<BoardDto> CreateAsync(CreateBoardDto createDto)
        {
            await _boardRules.ValidateCreateAsync(createDto);

            var entities = BoardMapper.ToWeeklyEntities(createDto);
            if (!entities.Any())
                throw new ValidationException("Det var ikke muligt at oprette plade for de valgte uger.");

            foreach (var entity in entities)
            {
                await _boardRepository.AddAsync(entity);
            }

            await _boardRepository.SaveChangesAsync();

            return BoardMapper.ToDto(entities.First());
        }

        public async Task<BoardDto?> UpdateAsync(string id, UpdateBoardDto updateDto)
        {
            await _boardRules.ValidateUpdateAsync(id, updateDto);
            
            var existing = await _boardRepository.GetByIdAsync(id);
            if (existing == null) return null;

            
            if (updateDto?.Numbers != null)
            {
                var numbers = updateDto.Numbers.ToList(); // ints directly
                var targetBoardSize = updateDto.BoardSize ?? existing.BoardSize;
                if (numbers.Count != targetBoardSize)
                    throw new ValidationException($"De valgte tal skal stemme over ens med pladens størrelse på ({targetBoardSize}).");
                
            }

            BoardMapper.ApplyUpdate(existing, updateDto);

            await _boardRepository.UpdateAsync(existing);
            await _boardRepository.SaveChangesAsync();

            return BoardMapper.ToDto(existing);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            
            await _boardRules.ValidateDeleteAsync(id);
            var existing = await _boardRepository.GetByIdAsync(id);
            if (existing == null) return false;
            
            await _boardRepository.DeleteAsync(existing);
            await _boardRepository.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<BoardDto>> getAllByUserIdAsync(string userId, BoardQueryParameters? parameters)
        {
            await _boardRules.ValidateGetAllByUserIdAsync(userId, parameters);
            
            var query = _boardRepository.AsQueryable().Where(b => b.UserID == userId).Include(b => b.Numbers);
            var sieveModel = parameters ?? new BoardQueryParameters();
            var totalCount = await query.CountAsync();
            var processedQuery = _sieveProcessor.Apply(sieveModel, query);
            var boards = await processedQuery.ToListAsync();

            return new PagedResult<BoardDto>
            {
                Items = boards.Select(BoardMapper.ToDto).ToList(),
                TotalCount = totalCount,
                Page = sieveModel.Page ?? 1,
                PageSize = sieveModel.PageSize ?? boards.Count
            };
        }
    }
}