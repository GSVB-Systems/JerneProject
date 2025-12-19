// csharp
using System;
using System.Linq;
using Contracts.BoardDTOs;
using Contracts.TransactionDTOs;
using Microsoft.EntityFrameworkCore;
using service.Exceptions;
using service.Repositories.Interfaces;
using service.Rules.RuleInterfaces;

namespace service.Rules;

public class PurchaseRules : IPurchaseRules
{
    private readonly IUserRepository _userRepo;

    public PurchaseRules(IUserRepository userRepo)
    {
        _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
    }

    public async Task ValidateProcessPurchaseAsync(CreateBoardDto boardDto, CreateTransactionDto transactionDto)
    {
        // Basic presence check for transactionDto so we can inspect its date for the maintenance window.
        if (transactionDto == null)
            throw new InvalidRequestException("Transaction DTO skal medtages.");

        // Determine momentUtc from transaction DTO if provided, otherwise use now.
        var momentUtc = ExtractTransactionMomentUtc(transactionDto);

        // Use local timezone (same logic as previous PurchaseWindowValidator)
        var utc = (momentUtc?.ToUniversalTime()) ?? DateTime.UtcNow;
        var timezone = TimeZoneInfo.Local;
        var local = TimeZoneInfo.ConvertTimeFromUtc(utc, timezone);
        var dow = local.DayOfWeek;
        var tod = local.TimeOfDay;

        // Window: Saturday from 17:00 (inclusive) through the end of Sunday (all Sunday)
        var isSaturdayAfter17 = dow == DayOfWeek.Saturday && tod >= TimeSpan.FromHours(17);
        var isSunday = dow == DayOfWeek.Sunday;

        if (isSaturdayAfter17 || isSunday)
        {
            throw new BoardPurchaseNotAllowedException();
        }

        try
        {
            if (boardDto == null)
                throw new InvalidRequestException("Board DTO skal medtages.");

            // transactionDto null check already done above

            if (string.IsNullOrWhiteSpace(transactionDto.UserID))
                throw new InvalidRequestException("Angiv et UserID for købet.");

            // Ensure board has the same user (service uses transaction.UserID for the buyer).
            if (string.IsNullOrWhiteSpace(boardDto.UserID))
                throw new InvalidRequestException("Angiv et UserID på boardet.");

            if (!string.Equals(boardDto.UserID, transactionDto.UserID, StringComparison.Ordinal))
                throw new RangeValidationException("Boardets UserID skal matche transaktionens UserID.");

            var userExists = await _userRepo.AsQueryable().AnyAsync(u => u.UserID == transactionDto.UserID);
            if (!userExists)
                throw new ResourceNotFoundException($"Brugeren med id '{transactionDto.UserID}' findes ikke.");

            // Ensure provided boardsize exists in the configured price map.
            if (!Services.PurchaseService.PriceConfig.Values.TryGetValue(boardDto.BoardSize, out var price) || price <= 0)
                throw new RangeValidationException("BoardSize har ikke en gyldig pris konfiguration.");

            // Week must be positive since service multiplies price by Week.
            if (boardDto.Week <= 0)
                throw new RangeValidationException("Week skal være større end nul.");

            if (boardDto.Numbers == null || boardDto.Numbers.Count == 0)
                throw new InvalidRequestException("Numbers skal angives på boardet.");

            if (boardDto.Numbers.Count != boardDto.BoardSize)
                throw new RangeValidationException($"Antal Numbers skal matche BoardSize ({boardDto.BoardSize}).");

            foreach (var n in boardDto.Numbers)
            {
                if (n < 1 || n > 16)
                    throw new RangeValidationException("Alle Numbers skal være mellem 1 og 16.");
            }

            // PurchaseService will override Amount, but validate computed amount is within Transaction constraints.
            var computedAmount = -1m * price * boardDto.Week;
            if (computedAmount < -1000m)
                throw new RangeValidationException("Købets beløb må ikke være mindre end -1000.");
        }
        catch (RuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Kunne ikke validere købet.", ex);
        }
    }


    private static DateTime? ExtractTransactionMomentUtc(CreateTransactionDto dto)
    {
        // case-insensitive property lookup
        var prop = dto.GetType().GetProperties()
            .FirstOrDefault(p => string.Equals(p.Name, "TransactionDate", StringComparison.OrdinalIgnoreCase));
        if (prop == null) return null;

        var val = prop.GetValue(dto);
        if (val == null) return null;

        if (val is DateTime dt)
        {
            // If unspecified, assume the value was provided in UTC
            if (dt.Kind == DateTimeKind.Unspecified)
                dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            return dt.ToUniversalTime();
        }

        if (val is DateTimeOffset dtoff)
            return dtoff.UtcDateTime;

        if (val is string s)
        {
            // support ISO-8601 strings (with or without 'Z')
            if (DateTimeOffset.TryParse(s, out var parsed))
                return parsed.UtcDateTime;
        }

        return null;
    }
}