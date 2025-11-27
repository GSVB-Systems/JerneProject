using dataaccess.Entities;
using Service.Repositories;
using service.Services.Interfaces;

namespace service.Services;

public class BoardService : Service<Board>, IBoardService
{
    public BoardService(IBoardRepository boardRepositoryRepository) : base(boardRepositoryRepository)
    {
    }
}