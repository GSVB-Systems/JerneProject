using Contracts.BoardNumberDTOs;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using service.Exceptions;
using service.Repositories.Interfaces;
using service.Rules.RuleInterfaces;
using Sieve.Models;

namespace service.Rules
{
    public class BoardNumberRules : IBoardNumberRules
    {
        private readonly IRepository<BoardNumber> _repo;

        public BoardNumberRules(IRepository<BoardNumber> repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task ValidateCreateAsync(CreateBoardNumberDto createDto)
        {
            try
            {
                if (createDto == null) throw new InvalidRequestException("Oprettelses-DTO skal angives.");

                if (createDto.Number < 1 || createDto.Number > 16)
                    throw new RangeValidationException("Nummer skal være mellem 1 og 16.");

                var numberInUse = await _repo.AsQueryable()
                    .AnyAsync(b => b.Number == createDto.Number);
                if (numberInUse)
                    throw new DuplicateResourceException($"Nummer '{createDto.Number}' er allerede i brug.");

            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Kunne ikke validere regler for oprettelse af board-nummer.", ex);
            }
        }

        public async Task ValidateUpdateAsync(string id, UpdateBoardNumberDto updateDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("Id skal angives for opdatering.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(b => b.BoardNumberID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"Board-nummer med id '{id}' findes ikke.");

                if (updateDto == null) return;

                if (updateDto.Number.HasValue)
                {
                    var val = updateDto.Number.Value;
                    if (val < 1 || val > 16)
                        throw new RangeValidationException("Nummer skal være mellem 1 og 16.");

                    var conflict = await _repo.AsQueryable()
                        .AnyAsync(b => b.BoardNumberID != id && b.Number == val);
                    if (conflict)
                        throw new DuplicateResourceException($"Nummer '{val}' er allerede i brug af et andet board-nummer.");
                }
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Kunne ikke validere opdateringsregler for board-nummer med id '{id}'.", ex);
            }
        }

        public async Task ValidateDeleteAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("Id skal angives for sletning.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(b => b.BoardNumberID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"Board-nummer med id '{id}' findes ikke.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Kunne ikke validere sletningsregler for board-nummer med id '{id}'.", ex);
            }
        }

        public async Task ValidateGetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("Id skal angives for hentning.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(b => b.BoardNumberID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"Board-nummer med id '{id}' findes ikke.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Kunne ikke validere regler for hentning af board-nummer med id '{id}'.", ex);
            }
        }

        public async Task ValidateGetAllAsync(SieveModel? parameters)
        {
            try
            {
                if (parameters == null) return;

                if (parameters.Page.HasValue && parameters.Page.Value <= 0)
                    throw new RangeValidationException("Side skal være større end nul.");

                if (parameters.PageSize.HasValue && (parameters.PageSize.Value <= 0 || parameters.PageSize.Value > 1000))
                    throw new RangeValidationException("PageSize skal være mellem 1 og 1000.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Kunne ikke validere regler for hentning af board-numre.", ex);
            }
        }
    }
}