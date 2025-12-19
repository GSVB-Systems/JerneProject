using service.Rules.RuleInterfaces;
using System.ComponentModel.DataAnnotations;
using Contracts.BoardNumberDTOs;
using Contracts;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using service.Mappers;
using service.Repositories.Interfaces;
using service.Services.Interfaces;
using Sieve.Models;


namespace service.Services
{
    public class BoardNumberService : Service<BoardNumber, CreateBoardNumberDto, UpdateBoardNumberDto>, IBoardNumberService
    {
        private readonly IRepository<BoardNumber> _repo;
		private readonly IBoardNumberRules _boardNumberRules;

        public BoardNumberService(IRepository<BoardNumber> repo, IBoardNumberRules boardNumberRules)
            : base(repo)
        {
            _repo = repo;
			_boardNumberRules = boardNumberRules ?? throw new ArgumentNullException(nameof(boardNumberRules));
        }

        public async Task<BoardNumberDto?> GetByIdAsync(string id)
        {
            await _boardNumberRules.ValidateGetByIdAsync(id);
            
            var entity = await _repo.AsQueryable()
                .FirstOrDefaultAsync(n => n.BoardNumberID == id);

            return entity == null ? null : BoardNumberMapper.ToDto(entity);
        }

        public async Task<PagedResult<BoardNumberDto>> GetAllAsync(SieveModel? parameters)
        {
            await _boardNumberRules.ValidateGetAllAsync(parameters);
            var query = _repo.AsQueryable();

            var items = await query.ToListAsync();
            var dtoItems = items.Select(BoardNumberMapper.ToDto).ToList();
            var count = dtoItems.Count;
            var page = parameters?.Page ?? 1;
            var pageSize = parameters?.PageSize ?? count;

            return new PagedResult<BoardNumberDto>
            {
                Items = dtoItems,
                TotalCount = count,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<BoardNumberDto> CreateAsync(CreateBoardNumberDto createDto)
        {
			
			await _boardNumberRules.ValidateCreateAsync(createDto);
			
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));
            if (createDto.Number < 1 || createDto.Number > 16)
                throw new ValidationException("Number must be between 1 and 16.");

            var entity = BoardNumberMapper.ToEntity(createDto) ?? new BoardNumber();
            if (string.IsNullOrWhiteSpace(entity.BoardNumberID))
                entity.BoardNumberID = Guid.NewGuid().ToString();

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return BoardNumberMapper.ToDto(entity);
        }

        public async Task<BoardNumberDto?> UpdateAsync(string id, UpdateBoardNumberDto updateDto)
        {

			await _boardNumberRules.ValidateUpdateAsync(id, updateDto);

            var existing = await base.GetByIdAsync(id);
            if (existing == null) return null;

            
            if (updateDto?.Number.HasValue == true)
            {
                var val = updateDto.Number.Value;
                if (val < 1 || val > 16)
                    throw new ValidationException("De valgte tal skal være mellem 1 og 16.");
            }

            BoardNumberMapper.ApplyUpdate(existing, updateDto);

            await _repo.UpdateAsync(existing);
            await _repo.SaveChangesAsync();

            return BoardNumberMapper.ToDto(existing);
        }

        public async Task<bool> DeleteAsync(string id)
        {
			await _boardNumberRules.ValidateDeleteAsync(id);
            return await base.DeleteAsync(id);
        }
    }
}