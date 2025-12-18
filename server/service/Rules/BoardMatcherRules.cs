using Microsoft.EntityFrameworkCore;
using service.Exceptions;
using service.Repositories.Interfaces;
using service.Rules.RuleInterfaces;
using dataaccess.Entities;

namespace service.Rules
{
    public class BoardMatcherRules : IBoardMatcherRules
    {
        private readonly IRepository<WinningBoard> _winningBoardRepo;

        public BoardMatcherRules(IRepository<WinningBoard> winningBoardRepo)
        {
            _winningBoardRepo = winningBoardRepo ?? throw new ArgumentNullException(nameof(winningBoardRepo));
        }

        public async Task ValidateGetBoardsContainingNumbersAsync(string winningBoardId)
        {
            await ValidateWinningBoardIdExistsAsync(winningBoardId);
        }

        public async Task ValidateGetBoardsContainingNumbersWithDecrementerAsync(string winningBoardId)
        {
            await ValidateWinningBoardIdExistsAsync(winningBoardId);
        }

        private async Task ValidateWinningBoardIdExistsAsync(string winningBoardId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(winningBoardId))
                    throw new InvalidRequestException("Angiv et winningBoardId.");

                var exists = await _winningBoardRepo.AsQueryable()
                    .AnyAsync(w => w.WinningBoardID == winningBoardId);

                if (!exists)
                    throw new ResourceNotFoundException($"WinningBoard med id '{winningBoardId}' findes ikke.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Kunne ikke validere WinningBoard med id: {winningBoardId}", ex);
            }
        }
    }
}

