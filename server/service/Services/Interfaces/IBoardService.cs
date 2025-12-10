using Contracts;
using dataaccess.Entities;
using Contracts.BoardDTOs;

namespace service.Services.Interfaces;

public interface IBoardService : IService<BoardDto, CreateBoardDto, UpdateBoardDto>
{
}