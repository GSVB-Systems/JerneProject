
using Contracts.WinningBoardDTOs;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using service.Exceptions;
using service.Repositories.Interfaces;
using service.Rules.RuleInterfaces;

namespace service.Rules
{
    public class WinningBoardRules : IWinningBoardRules
    {
        private readonly IWinningboardRepository _repo;

        public WinningBoardRules(IWinningboardRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task ValidateGetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("Angiv et id for at hente winning board.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(w => w.WinningBoardID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"WinningBoard med id '{id}' findes ikke.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Kunne ikke validere WinningBoard med id: {id}", ex);
            }
        }

        public Task ValidateGetAllAsync(SieveModel? parameters)
        {
            try
            {
                if (parameters == null) return Task.CompletedTask;

                if (parameters.Page.HasValue && parameters.Page.Value <= 0)
                    throw new RangeValidationException("Side nummer skal være større end nul.");

                if (parameters.PageSize.HasValue && (parameters.PageSize.Value <= 0 || parameters.PageSize.Value > 1000))
                    throw new RangeValidationException("PageSize skal være mellem 1 og 1000.");

                return Task.CompletedTask;
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Kunne ikke validere forespørgslen for WinningBoards.", ex);
            }
        }

        public Task ValidateCreateAsync(CreateWinningBoardDto createDto)
        {
            try
            {
                if (createDto == null)
                    throw new InvalidRequestException("Create DTO skal medtages.");

                if (createDto.WinningNumbers == null)
                    throw new InvalidRequestException("Udfyld felterne med de trukkede numre.");

                var count = createDto.WinningNumbers.Count;
                if (count != 3 && count != 5)
                    throw new RangeValidationException("Der må kun trækkes 3 eller 5 numre.");

                foreach (var n in createDto.WinningNumbers)
                {
                    if (n < 1 || n > 16)
                        throw new RangeValidationException("De trukkede numre må kun være mellem 1 og 16.");
                }

                if (createDto.WinningNumbers.Count != new HashSet<int>(createDto.WinningNumbers).Count)
                    throw new RangeValidationException("De trukkede numre skal være unikke.");

                return Task.CompletedTask;
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Kunne ikke oprette WinningBoard, da den ikke opfylder alle krav.", ex);
            }
        }

        public async Task ValidateUpdateAsync(string id, UpdateWinningBoardDto updateDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("Angiv et id for opdatering.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(w => w.WinningBoardID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"WinningBoard med ID: '{id}' findes ikke.");

                if (updateDto == null) return;

                if (updateDto.WinningNumbers == null)
                    throw new InvalidRequestException("Udfyld felterne med de trukkede numre.");

                var count = updateDto.WinningNumbers.Count;
                if (count != 3 && count != 5)
                    throw new RangeValidationException("Der må kun trækkes 3 eller 5 numre.");

                foreach (var n in updateDto.WinningNumbers)
                {
                    if (n < 1 || n > 16)
                        throw new RangeValidationException("De trukkede numre må kun være mellem 1 og 16.");
                }

                if (updateDto.WinningNumbers.Count != new HashSet<int>(updateDto.WinningNumbers).Count)
                    throw new RangeValidationException("De trukkede numre skal være unikke.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Kunne ikke validere opdatering for WinningBoard : {id}", ex);
            }
        }

        public async Task ValidateDeleteAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("Angiv et id for sletning.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(w => w.WinningBoardID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"WinningBoard med ID: '{id}' findes ikke.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Kunne ikke validere WinningBoard til sletning.", ex);
            }
        }
    }
}