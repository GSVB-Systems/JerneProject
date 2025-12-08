using dataaccess.Entities;
using service.Repositories.Interfaces;
using service.Services.Interfaces;

namespace service.Services;

public class WinningNumberService : Service<WinningNumber>, IWinningNumberService
{
    public WinningNumberService(IRepository<WinningNumber> repository) : base(repository)
    {
    }
}