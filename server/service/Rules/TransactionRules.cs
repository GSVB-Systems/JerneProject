using Contracts.TransactionDTOs;
using Microsoft.EntityFrameworkCore;
using service.Exceptions;
using service.Repositories.Interfaces;
using service.Rules.RuleInterfaces;
using Sieve.Models;

namespace service.Rules;

public class TransactionRules : ITransactionRules
{
    private readonly ITransactionRepository _transactionRepo;
    private readonly IUserRepository _userRepo;

    public TransactionRules(ITransactionRepository transactionRepo, IUserRepository userRepo)
    {
        _transactionRepo = transactionRepo ?? throw new ArgumentNullException(nameof(transactionRepo));
        _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
    }

    public async Task ValidateGetByIdAsync(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new InvalidRequestException("Angiv et id for at hente transaktionen.");

            var exists = await _transactionRepo.AsQueryable().AnyAsync(t => t.TransactionID == id);
            if (!exists)
                throw new ResourceNotFoundException($"Transaktionen med id '{id}' findes ikke.");
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Kunne ikke validere transaktionen.", ex);
        }
    }

    public Task ValidateGetAllAsync(SieveModel? parameters)
    {
        try
        {
            if (parameters == null) return Task.CompletedTask;

            EnsureValidPaging(parameters);
            return Task.CompletedTask;
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Kunne ikke validere transaktions listen.", ex);
        }
    }

    public async Task ValidateGetAllByUserIdAsync(string userId, TransactionQueryParameters? parameters)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidRequestException("Angiv et bruger id for at hente transaktioner.");

            var userExists = await _userRepo.AsQueryable().AnyAsync(u => u.UserID == userId);
            if (!userExists)
                throw new ResourceNotFoundException($"Brugeren med id '{userId}' findes ikke.");

            if (parameters != null)
                EnsureValidPaging(parameters);
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Kunne ikke validere transaktioner for brugeren.", ex);
        }
    }

    public async Task ValidateCreateAsync(CreateTransactionDto dto)
    {
        try
        {
            if (dto == null)
                throw new InvalidRequestException("CreateTransactionDto skal medtages.");

            if (string.IsNullOrWhiteSpace(dto.UserID))
                throw new InvalidRequestException("Angiv et UserID.");

            var userExists = await _userRepo.AsQueryable().AnyAsync(u => u.UserID == dto.UserID);
            if (!userExists)
                throw new ResourceNotFoundException($"Brugeren med id '{dto.UserID}' findes ikke.");

            if (dto.TransactionString == null)
                throw new InvalidRequestException("Angiv en TransactionString.");

            // Special-case used by TransactionService: when value is 'GUID' the service generates it.
            if (dto.TransactionString != "GUID" && string.IsNullOrWhiteSpace(dto.TransactionString))
                throw new InvalidRequestException("Angiv en TransactionString.");

            if (dto.TransactionString != "GUID" && dto.TransactionString.Length > 200)
                throw new RangeValidationException("TransactionString må højst være 200 tegn.");

            // Keep in sync with DTO annotations, but enforce at service layer too.
            if (dto.Amount < -1000m)
                throw new RangeValidationException("Beløbet må ikke være mindre end -1000.");
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Kunne ikke validere transaktionen til oprettelse.", ex);
        }
    }

    public async Task ValidateUpdateAsync(string id, UpdateTransactionDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new InvalidRequestException("Angiv et id for opdatering af transaktionen.");

            var exists = await _transactionRepo.AsQueryable().AnyAsync(t => t.TransactionID == id);
            if (!exists)
                throw new ResourceNotFoundException($"Transaktionen med id '{id}' findes ikke.");

            // UpdateTransactionDto properties are non-nullable in Contracts, so dto is expected.
            // Still, keep the interface consistent with other rule classes.
            if (dto == null) return;

            if (string.IsNullOrWhiteSpace(dto.TransactionString) == false)
            {
                if (dto.TransactionString.Length > 200)
                    throw new RangeValidationException("TransactionString må højst være 200 tegn.");
            }
            else if (dto.TransactionString != null)
            {
                // dto.TransactionString set but blank
                throw new InvalidRequestException("Angiv en gyldig TransactionString.");
            }

            if (dto.Amount.HasValue && dto.Amount.Value < 0m)
                throw new RangeValidationException("Beløbet må ikke være negativt.");

            if (!string.IsNullOrWhiteSpace(dto.UserID))
            {
                var userExists = await _userRepo.AsQueryable().AnyAsync(u => u.UserID == dto.UserID);
                if (!userExists)
                    throw new ResourceNotFoundException($"Brugeren med id '{dto.UserID}' findes ikke.");
            }
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"Kunne ikke validere transaktionen : {id}", ex);
        }
    }

    public async Task ValidateDeleteAsync(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new InvalidRequestException("Angiv et id for sletning af transaktionen.");

            var exists = await _transactionRepo.AsQueryable().AnyAsync(t => t.TransactionID == id);
            if (!exists)
                throw new ResourceNotFoundException($"Transaktionen med id '{id}' findes ikke.");
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Kunne ikke validere transaktionen til sletning.", ex);
        }
    }

    private static void EnsureValidPaging(SieveModel parameters)
    {
        if (parameters.Page.HasValue && parameters.Page.Value <= 0)
            throw new RangeValidationException("Side nummer skal være større end nul.");

        if (parameters.PageSize.HasValue && (parameters.PageSize.Value <= 0 || parameters.PageSize.Value > 1000))
            throw new RangeValidationException("PageSize skal være mellem 1 og 1000.");
    }
}
