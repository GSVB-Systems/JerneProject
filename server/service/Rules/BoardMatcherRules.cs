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

            
            await ValidateWinningBoardIsUniqueForWeekAsync(winningBoardId);
        }

        public async Task ValidateGetBoardsContainingNumbersWithDecrementerAsync(string winningBoardId)
        {
            await ValidateWinningBoardIdExistsAsync(winningBoardId);

            await ValidateWinningBoardIsUniqueForWeekAsync(winningBoardId);
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

        private async Task ValidateWinningBoardIsUniqueForWeekAsync(string winningBoardId)
        {
            try
            {
                var winning = await _winningBoardRepo.AsQueryable()
                    .FirstOrDefaultAsync(w => w.WinningBoardID == winningBoardId);

                if (winning == null)
                    return;

                var duplicates = await _winningBoardRepo.AsQueryable()
                    .AnyAsync(w => w.Week == winning.Week && w.WeekYear == winning.WeekYear && w.WinningBoardID != winningBoardId);

                if (duplicates)
                    throw new DuplicateResourceException($"Der findes allerede en winning board for uge {winning.Week} år {winning.WeekYear}.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Kunne ikke validere unique winning board for id: {winningBoardId}", ex);
            }
        }
    }
}
