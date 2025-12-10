using Contracts;
using Contracts.WinningNumberDTOs;
using dataaccess.Entities;

namespace service.Services.Interfaces;

public interface IWinningNumberService : IService<WinningNumberDto, CreateWinningNumberDto, UpdateWinningNumberDto>
{
    
}