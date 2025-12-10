using Contracts;
using Contracts.BoardNumberDTOs;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using service.Mappers;
using service.Repositories.Interfaces;
using service.Services.Interfaces;
using Sieve.Models;
using Sieve.Services;

namespace service.Services;

public class BoardNumberService : IBoardNumberService
{
    private readonly IBoardNumberRepository _boardNumberRepository;
    private readonly ISieveProcessor _sieveProcessor;

    public BoardNumberService(IBoardNumberRepository repository, ISieveProcessor sieveProcessor)
    {
        _boardNumberRepository = repository;
        _sieveProcessor = sieveProcessor;
    }

    public async Task<BoardNumberDto?> GetByIdAsync(string id)
    {
        var entity = await _boardNumberRepository.GetByIdAsync(id);
        return entity == null ? null : BoardNumberMapper.ToDto(entity);
    }

    public async Task<PagedResult<BoardNumberDto>> GetAllAsync(SieveModel? parameters)
    {
        var query = _boardNumberRepository.AsQueryable();
        var sieveModel = parameters ?? new SieveModel();

        var totalCount = await query.CountAsync();
        var processedQuery = _sieveProcessor.Apply(sieveModel, query);
        var entities = await processedQuery.ToListAsync();

        return new PagedResult<BoardNumberDto>
        {
            Items = entities.Select(BoardNumberMapper.ToDto).ToList(),
            TotalCount = totalCount,
            Page = sieveModel.Page ?? 1,
            PageSize = sieveModel.PageSize ?? entities.Count
        };
    }

    public async Task<BoardNumberDto> CreateAsync(CreateBoardNumberDto createDto)
    {
        var entity = BoardNumberMapper.ToEntity(createDto);
        await _boardNumberRepository.AddAsync(entity);
        await _boardNumberRepository.SaveChangesAsync();
        return BoardNumberMapper.ToDto(entity);
    }

    public async Task<BoardNumberDto?> UpdateAsync(string id, UpdateBoardNumberDto updateDto)
    {
        var existing = await _boardNumberRepository.GetByIdAsync(id);
        if (existing == null) return null;

        BoardNumberMapper.ApplyUpdate(existing, updateDto);
        await _boardNumberRepository.UpdateAsync(existing);
        await _boardNumberRepository.SaveChangesAsync();

        return BoardNumberMapper.ToDto(existing);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var existing = await _boardNumberRepository.GetByIdAsync(id);
        if (existing == null) return false;

        await _boardNumberRepository.DeleteAsync(existing);
        await _boardNumberRepository.SaveChangesAsync();
        return true;
    }
}