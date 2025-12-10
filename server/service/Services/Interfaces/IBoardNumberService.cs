using Contracts;
using dataaccess.Entities;
using Contracts.BoardNumberDTOs;

namespace service.Services.Interfaces;

public interface IBoardNumberService : IService<BoardNumberDto, CreateBoardNumberDto, UpdateBoardNumberDto>
{
    
}