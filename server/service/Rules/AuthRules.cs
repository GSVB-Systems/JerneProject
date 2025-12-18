using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Contracts.Validation;
using dataaccess.Entities;
using service.Exceptions;
using service.Repositories.Interfaces;
using service.Rules.RuleInterfaces;
using Sieve.Models;

namespace service.Rules;

public class AuthRules : IAuthRules
{
    private static readonly PasswordComplexityAttribute PasswordAttribute = new();
    private readonly IAuthRepository _authRepository;

    public AuthRules(IAuthRepository authRepository)
    {
        _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
    }

    public async Task ValidateGetByIdAsync(string id)
    {
        try
        {
            ValidateId(id, "Bruger");

            var exists = await _authRepository.AsQueryable()
                .AnyAsync(u => u.UserID == id);
            if (!exists)
                throw new ResourceNotFoundException($"Brugeren med id '{id}' findes ikke.");
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"Kunne ikke validere henteforespørgslen for bruger med id '{id}'.", ex);
        }
    }

    public async Task ValidateGetAllAsync(SieveModel? parameters)
    {
        try
        {
            ValidatePagingParameters(parameters);
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Kunne ikke validere forespørgslen om at hente brugere.", ex);
        }
    }

    public async Task ValidateCreateAsync(User entity)
    {
        try
        {
            if (entity == null)
                throw new InvalidRequestException("Brugeren skal medtages for at kunne oprettes.");

            ValidateEmail(entity.Email);

            if (string.IsNullOrWhiteSpace(entity.Firstname))
                throw new InvalidRequestException("Fornavn skal angives.");

            if (string.IsNullOrWhiteSpace(entity.Lastname))
                throw new InvalidRequestException("Efternavn skal angives.");

            if (string.IsNullOrWhiteSpace(entity.Hash))
                throw new InvalidRequestException("Adgangskode er påkrævet.");

            var taken = await _authRepository.AsQueryable()
                .AnyAsync(u => u.Email == entity.Email);
            if (taken)
                throw new DuplicateResourceException($"Emailen '{entity.Email}' findes allerede i systemet.");
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Kunne ikke validere oprettelse af bruger.", ex);
        }
    }

    public async Task ValidateUpdateAsync(string id, User entity)
    {
        try
        {
            ValidateId(id, "opdatering");

            var exists = await _authRepository.AsQueryable()
                .AnyAsync(u => u.UserID == id);
            if (!exists)
                throw new ResourceNotFoundException($"Brugeren med id '{id}' findes ikke.");

            if (entity == null)
                return;

            if (!string.IsNullOrWhiteSpace(entity.Email))
            {
                ValidateEmail(entity.Email);

                var conflict = await _authRepository.AsQueryable()
                    .AnyAsync(u => u.UserID != id && u.Email == entity.Email);
                if (conflict)
                    throw new DuplicateResourceException($"Emailen '{entity.Email}' er allerede i brug af en anden bruger.");
            }
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"Kunne ikke validere opdatering af bruger med id '{id}'.", ex);
        }
    }

    public async Task ValidateDeleteAsync(string id)
    {
        try
        {
            ValidateId(id, "sletning");

            var exists = await _authRepository.AsQueryable()
                .AnyAsync(u => u.UserID == id);
            if (!exists)
                throw new ResourceNotFoundException($"Brugeren med id '{id}' findes ikke.");
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"Kunne ikke validere sletning af bruger med id '{id}'.", ex);
        }
    }

    public Task ValidateVerifyPasswordByEmailAsync(string email, string plainPassword)
    {
        try
        {
            ValidateEmail(email);
            ValidatePlainPassword(plainPassword, "eksisterende adgangskode");
            return Task.CompletedTask;
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Kunne ikke validere loginforspørgsel.", ex);
        }
    }

    public Task ValidateGetUserByEmailAsync(string email)
    {
        try
        {
            ValidateEmail(email);
            return Task.CompletedTask;
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Kunne ikke validere email-input til brugeropslag.", ex);
        }
    }

    public async Task ValidateUpdateUserPasswordAsync(string userId, string oldPassword, string newPassword)
    {
        try
        {
            ValidateId(userId, "opdatering af adgangskode");
            ValidatePlainPassword(oldPassword, "nuværende adgangskode");
            ValidatePlainPassword(newPassword, "ny adgangskode");

            if (oldPassword == newPassword)
                throw new RangeValidationException("Den nye adgangskode må ikke være den samme som den eksisterende.");

            EnsurePasswordComplexity(newPassword);

            var exists = await _authRepository.AsQueryable()
                .AnyAsync(u => u.UserID == userId);
            if (!exists)
                throw new ResourceNotFoundException($"Brugeren med id '{userId}' findes ikke.");
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"Kunne ikke validere adgangskodeskift for bruger med id '{userId}'.", ex);
        }
    }

    public async Task ValidateAdminResetUserPasswordAsync(string userId, string newPassword)
    {
        try
        {
            ValidateId(userId, "nulstilling af adgangskode");
            ValidatePlainPassword(newPassword, "ny adgangskode");
            EnsurePasswordComplexity(newPassword);

            var exists = await _authRepository.AsQueryable()
                .AnyAsync(u => u.UserID == userId);
            if (!exists)
                throw new ResourceNotFoundException($"Brugeren med id '{userId}' findes ikke.");
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"Kunne ikke validere administrator nulstilling af adgangskode for bruger med id '{userId}'.", ex);
        }
    }

    private static void ValidatePagingParameters(SieveModel? parameters)
    {
        if (parameters == null)
            return;

        if (parameters.Page.HasValue && parameters.Page.Value <= 0)
            throw new RangeValidationException("Side nummer skal være større end nul.");

        if (parameters.PageSize.HasValue && (parameters.PageSize.Value <= 0 || parameters.PageSize.Value > 1000))
            throw new RangeValidationException("PageSize skal være mellem 1 og 1000.");
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidRequestException("Email skal angives.");

        if (!IsValidEmail(email))
            throw new RangeValidationException("Angiv en gyldig Email adresse.");
    }

    private static void ValidatePlainPassword(string password, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidRequestException($"{fieldName} skal angives.");
    }

    private static void ValidateId(string id, string context)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new InvalidRequestException($"Id for {context} skal angives.");
    }

    private static void EnsurePasswordComplexity(string password)
    {
        var validationResult = PasswordAttribute.GetValidationResult(password, new ValidationContext(password));
        if (validationResult != ValidationResult.Success)
            throw new RangeValidationException(validationResult?.ErrorMessage ?? "Adgangskoden opfylder ikke kravene.");
    }

    private static bool IsValidEmail(string address)
    {
        try
        {
            var _ = new MailAddress(address);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

