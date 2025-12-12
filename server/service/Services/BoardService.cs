using System.ComponentModel.DataAnnotations;
using Contracts.BoardDTOs;
using Contracts;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using service.Mappers;
using service.Repositories.Interfaces;
using service.Services.Interfaces;
using Sieve.Models;

namespace service.Services
{
    public class BoardService : Service<Board, CreateBoardDto, UpdateBoardDto>, IBoardService
    {
        private readonly IBoardRepository _boardRepository;

        public BoardService(IBoardRepository boardRepository)
            : base(boardRepository)
        {
            _boardRepository = boardRepository;
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

            var items = await query.ToListAsync();

            var dtoItems = items.Select(BoardMapper.ToDto).ToList();
            var count = dtoItems.Count;

            var page = parameters?.Page ?? 1;
            var pageSize = parameters?.PageSize ?? count;

            return new PagedResult<BoardDto>
            {
                Items = dtoItems,
                TotalCount = count,
                Page = page,
                PageSize = pageSize
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
            
            
           
            var values = createDto.Numbers.ToList();

            if (!createDto.IsRepeating)
            {
                var duplicateValues = values.GroupBy(v => v).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                if (duplicateValues.Any())
                    throw new ValidationException($"Numbers must be unique. Duplicates: {string.Join(',', duplicateValues)}");
            }

            var entity = BoardMapper.ToEntity(createDto);
            
            if (string.IsNullOrWhiteSpace(entity.BoardID))
                entity.BoardID = Guid.NewGuid().ToString();

            if (entity.Numbers == null)
                entity.Numbers = new System.Collections.Generic.List<BoardNumber>();

            foreach (var n in entity.Numbers)
            {
                if (string.IsNullOrWhiteSpace(n.BoardNumberID))
                    n.BoardNumberID = Guid.NewGuid().ToString();
                n.BoardID = entity.BoardID;
            }

            await _boardRepository.AddAsync(entity);
            await _boardRepository.SaveChangesAsync();

            return BoardMapper.ToDto(entity);
        }

        public async Task<BoardDto?> UpdateAsync(string id, UpdateBoardDto updateDto)
        {
            var existing = await base.GetByIdAsync(id);
            if (existing == null) return null;

            // If numbers are being updated validate them
            if (updateDto?.Numbers != null)
            {
                var numbers = updateDto.Numbers.ToList(); // ints directly
                var targetBoardSize = updateDto.BoardSize ?? existing.BoardSize;
                if (numbers.Count != targetBoardSize)
                    throw new ValidationException($"Numbers count must equal BoardSize ({targetBoardSize}).");

                var isRepeating = updateDto.IsRepeating ?? existing.IsRepeating;
                if (!isRepeating && numbers.Count != numbers.Distinct().Count())
                    throw new ValidationException("Numbers must be unique.");
            }

            BoardMapper.ApplyUpdate(existing, updateDto);

            await _boardRepository.UpdateAsync(existing);
            await _boardRepository.SaveChangesAsync();

            return BoardMapper.ToDto(existing);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await base.DeleteAsync(id);
        }
    }
}