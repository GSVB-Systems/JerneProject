using System.ComponentModel.DataAnnotations;
using Contracts.BoardDTOs;
using Contracts;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using service.Mappers;
using service.Repositories.Interfaces;
using service.Services.Interfaces;
using Sieve.Models;
using Sieve.Services;

namespace service.Services
{
    public class BoardService : IBoardService
    {
        private readonly IBoardRepository _boardRepository;
        private readonly ISieveProcessor _sieveProcessor;

        public BoardService(IBoardRepository boardRepository, ISieveProcessor sieveProcessor)
        {
            _boardRepository = boardRepository;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<BoardDto?> GetByIdAsync(string id)
        {
            var entity = await _boardRepository.AsQueryable()
                .Include(b => b.Numbers)
                .FirstOrDefaultAsync(b => b.BoardID == id);

            return entity == null ? null : BoardMapper.ToDto(entity);
        }

        public async Task<PagedResult<BoardDto>> GetAllAsync(SieveModel? parameters)
        {
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
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            if (createDto.Numbers == null)
                throw new ValidationException("Numbers are required.");

            if (createDto.BoardSize <= 0)
                throw new ValidationException("Must choose Boardsize between 5-8");

            if (createDto.Numbers.Count != createDto.BoardSize)
                throw new ValidationException($"Numbers count must equal BoardSize ({createDto.BoardSize}).");
            if (createDto.Week <= 0)
                throw new ValidationException("Week count must be at least 1.");

            var entities = BoardMapper.ToWeeklyEntities(createDto);
            if (!entities.Any())
                throw new ValidationException("Unable to create boards for the requested week range.");

            foreach (var entity in entities)
            {
                await _boardRepository.AddAsync(entity);
            }

            await _boardRepository.SaveChangesAsync();

            return BoardMapper.ToDto(entities.First());
        }

        public async Task<BoardDto?> UpdateAsync(string id, UpdateBoardDto updateDto)
        {
            var existing = await _boardRepository.GetByIdAsync(id);
            if (existing == null) return null;

            // If numbers are being updated validate them
            if (updateDto?.Numbers != null)
            {
                var numbers = updateDto.Numbers.ToList(); // ints directly
                var targetBoardSize = updateDto.BoardSize ?? existing.BoardSize;
                if (numbers.Count != targetBoardSize)
                    throw new ValidationException($"Numbers count must equal BoardSize ({targetBoardSize}).");
                
            }

            BoardMapper.ApplyUpdate(existing, updateDto);

            await _boardRepository.UpdateAsync(existing);
            await _boardRepository.SaveChangesAsync();

            return BoardMapper.ToDto(existing);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var existing = await _boardRepository.GetByIdAsync(id);
            if (existing == null) return false;
            
            await _boardRepository.DeleteAsync(existing);
            await _boardRepository.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<BoardDto>> getAllByUserIdAsync(string userId, BoardQueryParameters? parameters)
        {
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