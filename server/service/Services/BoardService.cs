using System.Linq;
using System.Threading.Tasks;
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
            var board = await _boardRepository.GetByIdAsync(id);
            return board == null ? null : BoardMapper.ToDto(board);
        }

        public async Task<PagedResult<BoardDto>> GetAllAsync(SieveModel? parameters)
        {
            var query = _boardRepository.AsQueryable();
            var sieveModel = parameters ?? new SieveModel();

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

        public async Task<BoardDto> CreateAsync(CreateBoardDto dto)
        {
            var entity = BoardMapper.ToEntity(dto);
            await _boardRepository.AddAsync(entity);
            await _boardRepository.SaveChangesAsync();
            return BoardMapper.ToDto(entity);
        }

        public async Task<BoardDto?> UpdateAsync(string id, UpdateBoardDto dto)
        {
            var existing = await _boardRepository.GetByIdAsync(id);
            if (existing == null) return null;

            BoardMapper.ApplyUpdate(existing, dto);
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
    }
}