using dataaccess.Entities;
using service.Repositories.Interfaces;
using service.Services.Interfaces;


namespace service.Services;

public class BoardNumberService : Service<BoardNumber>, IBoardNumberService
{
    public BoardNumberService(IRepository<BoardNumber> repository) : base(repository)
    {
    }
}