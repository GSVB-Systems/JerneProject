using System.Transactions;
using Contracts;
using Contracts.WinningBoardDTOs;
using dataaccess.Entities;

namespace service.Services.Interfaces;

public interface IWinningBoardService : IService<WinningBoardDto, CreateWinningBoardDto, UpdateWinningBoardDto>
{
    
}