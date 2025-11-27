using dataaccess.Entities;
using Service.Repositories;

namespace service.Services;


public interface IBoardService : IService<Board>
{
    
}
public class BoardService : Service<Board>, IBoardService
{
    public BoardService(IBoard boardRepository) : base(boardRepository)
    {
    }
}