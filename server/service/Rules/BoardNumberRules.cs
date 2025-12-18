using System;
using System.Threading.Tasks;
using Contracts.BoardNumberDTOs;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using service.Exceptions;
using service.Repositories.Interfaces;
using service.Rules.RuleInterfaces;

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
                if (createDto == null) throw new InvalidRequestException("Create DTO must be provided.");

                if (createDto.Number < 1 || createDto.Number > 16)
                    throw new RangeValidationException("Number must be between 1 and 16.");

                var numberInUse = await _repo.AsQueryable()
                    .AnyAsync(b => b.Number == createDto.Number);
                if (numberInUse)
                    throw new DuplicateResourceException($"Number '{createDto.Number}' is already in use.");

                // Intentional: do not reference BoardNumberID on the DTO — DTO does not contain that property.
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to validate board number creation rules.", ex);
            }
        }

        public async Task ValidateUpdateAsync(string id, UpdateBoardNumberDto updateDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("Id must be provided for update.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(b => b.BoardNumberID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"Board number with id '{id}' does not exist.");

                if (updateDto == null) return;

                if (updateDto.Number.HasValue)
                {
                    var val = updateDto.Number.Value;
                    if (val < 1 || val > 16)
                        throw new RangeValidationException("Number must be between 1 and 16.");

                    var conflict = await _repo.AsQueryable()
                        .AnyAsync(b => b.BoardNumberID != id && b.Number == val);
                    if (conflict)
                        throw new DuplicateResourceException($"Number '{val}' is already in use by another board number.");
                }
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to validate board number update rules for id '{id}'.", ex);
            }
        }

        public async Task ValidateDeleteAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("Id must be provided for delete.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(b => b.BoardNumberID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"Board number with id '{id}' does not exist.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to validate board number delete rules for id '{id}'.", ex);
            }
        }
    }
}