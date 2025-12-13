using Contracts;
using dataaccess.Entities;
using Contracts.BoardDTOs;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;

namespace service.Services.Interfaces;

public interface IBoardService : IService<BoardDto, CreateBoardDto, UpdateBoardDto>
{
    Task<PagedResult<BoardDto>> getAllByUserIdAsync(string userId, BoardQueryParameters? parameters);
}