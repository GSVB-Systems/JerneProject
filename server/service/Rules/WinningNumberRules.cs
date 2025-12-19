using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Contracts.WinningNumberDTOs;
using Sieve.Models;
using service.Rules.RuleInterfaces;

namespace service.Rules
{
    public class WinningNumberRules : IWinningNumberRules
    {
        private const int MinNumber = 1;
        private const int MaxNumber = 16;
        private const int MaxPageSize = 100;
        private const int MaxParamLength = 200;
        private static readonly string[] AllowedFields = { "WinningNumberID", "WinningBoardID", "Number" };
        private static readonly StringComparer FieldComparer = StringComparer.OrdinalIgnoreCase;
        private static readonly Regex FilterFieldRegex = new(@"^\s*([A-Za-z0-9_]+)", RegexOptions.Compiled);

        public void ValidateGetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id skal være angivet", nameof(id));
        }

        public void ValidateGetAll(SieveModel? parameters)
        {
            if (parameters == null) return;

            if (parameters.Page.HasValue && parameters.Page.Value < 1)
                throw new ValidationException("Side (Page) skal være større end eller lig med 1.");

            if (parameters.PageSize.HasValue)
            {
                if (parameters.PageSize.Value < 1 || parameters.PageSize.Value > MaxPageSize)
                    throw new ValidationException($"PageSize skal være mellem 1 og {MaxPageSize}.");
            }

            if (!string.IsNullOrWhiteSpace(parameters.Sorts))
            {
                if (parameters.Sorts.Length > MaxParamLength)
                    throw new ValidationException("Sorts-parameteren er for lang.");

                var sorts = parameters.Sorts.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => s.Length > 0);

                foreach (var s in sorts)
                {
                    var field = s.StartsWith("-") ? s.Substring(1).Trim() : s;
                    if (!AllowedFields.Contains(field, FieldComparer))
                        throw new ValidationException($"Sort-feltet '{field}' er ikke tilladt.");
                }
            }

            if (!string.IsNullOrWhiteSpace(parameters.Filters))
            {
                if (parameters.Filters.Length > MaxParamLength)
                    throw new ValidationException("Filters-parameteren er for lang.");

                var filters = parameters.Filters.Split(',')
                    .Select(f => f.Trim())
                    .Where(f => f.Length > 0);

                foreach (var f in filters)
                {
                    var m = FilterFieldRegex.Match(f);
                    if (!m.Success)
                        throw new ValidationException("Ugyldigt filterudtryk.");

                    var field = m.Groups[1].Value;
                    if (!AllowedFields.Contains(field, FieldComparer))
                        throw new ValidationException($"Filter-feltet '{field}' er ikke tilladt.");
                }
            }
        }

        public void ValidateCreate(CreateWinningNumberDto dto)
        {
            if (dto == null)
                throw new ValidationException("CreateWinningNumberDto skal være angivet.");

            ValidateNumber(dto.Number);
        }

        public void ValidateUpdate(string id, UpdateWinningNumberDto dto)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id skal være angivet", nameof(id));

            if (dto == null)
                throw new ValidationException("UpdateWinningNumberDto skal være angivet.");

            if (dto.Number.HasValue)
                ValidateNumber(dto.Number.Value);
        }

        public void ValidateDelete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id skal være angivet", nameof(id));
        }

        private void ValidateNumber(int number)
        {
            if (number < MinNumber || number > MaxNumber)
                throw new ValidationException($"Tal skal være mellem {MinNumber} og {MaxNumber} inklusive.");
        }
    }
}