using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using service.Repositories.Interfaces;
using service.Services.Interfaces;
using Contracts.WinnerResultDTO;
using service.Mappers;

namespace service.Services
{
    public class BoardMatchService : IBoardMatcherService
    {
        private readonly IRepository<WinningBoard> _winningBoardRepo;
        private readonly IRepository<Board> _boardRepo;

        public BoardMatchService(IRepository<WinningBoard> winningBoardRepo, IRepository<Board> boardRepo)
        {
            _winningBoardRepo = winningBoardRepo;
            _boardRepo = boardRepo;
        }
        
        public async Task<List<WinnerResultDto>> GetBoardsContainingNumbersAsync(string winningBoardId)
        {
            var winning = await _winningBoardRepo.AsQueryable()
                .Include(w => w.WinningNumbers)
                .FirstOrDefaultAsync(w => w.WinningBoardID == winningBoardId);

            if (winning == null || winning.WinningNumbers == null || !winning.WinningNumbers.Any())
                return new List<WinnerResultDto>();

            var numbers = winning.WinningNumbers.Select(w => w.Number).Distinct().ToList();

            IQueryable<Board> query = _boardRepo.AsQueryable()
                .Include(b => b.Numbers)
                .Include(b => b.User);

            foreach (var n in numbers)
            {
                var local = n;
                query = query.Where(b => b.Numbers.Any(bn => bn.Number == local));
            }

            var boards = await query.ToListAsync();

            var results = boards.Select(b => new WinnerResultDto
            {
                Board = BoardMapper.ToDto(b),
                User = UserMapper.ToDto(b.User!)
            }).ToList();
            

            return results;
        }

        public  async Task<List<WinnerResultDto>> GetBoardsContainingNumbersWithDecrementerAsync(string winningBoardId)
        {
            var winning = await _winningBoardRepo.AsQueryable()
                .Include(w => w.WinningNumbers)
                .FirstOrDefaultAsync(w => w.WinningBoardID == winningBoardId);

            if (winning == null || winning.WinningNumbers == null || !winning.WinningNumbers.Any())
                return new List<WinnerResultDto>();

            var numbers = winning.WinningNumbers.Select(w => w.Number).Distinct().ToList();

            IQueryable<Board> query = _boardRepo.AsQueryable()
                .Include(b => b.Numbers)
                .Include(b => b.User);

            foreach (var n in numbers)
            {
                var local = n;
                query = query.Where(b => b.Numbers.Any(bn => bn.Number == local));
            }

            var boards = await query.ToListAsync();

            var results = boards.Select(b => new WinnerResultDto
            {
                Board = BoardMapper.ToDto(b),
                User = UserMapper.ToDto(b.User!)
            }).ToList();

            
            await _boardRepo
                .AsQueryable()
                .Where(b => b.Week > 0)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(b => b.Week, b => b.Week - 1));

            
            await _boardRepo
                .AsQueryable()
                .Where(b => b.Week == 0 && b.IsActive)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(b => b.IsActive, b => false));

            return results;
        }
    }
}