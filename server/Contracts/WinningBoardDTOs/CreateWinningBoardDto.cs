using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contracts.WinningBoardDTOs
{
    public class CreateWinningBoardDto : IValidatableObject
    {
        [Required]
        public List<int> WinningNumbers { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (WinningNumbers == null)
            {
                yield return new ValidationResult("WinningNumbers is required.", new[] { nameof(WinningNumbers) });
                yield break;
            }

            var count = WinningNumbers.Count;
            if (count != 3 && count != 5)
            {
                yield return new ValidationResult("WinningNumbers must contain exactly 3 or 5 items.", new[] { nameof(WinningNumbers) });
            }

            foreach (var n in WinningNumbers)
            {
                if (n < 1 || n > 16)
                {
                    yield return new ValidationResult("Each winning number must be between 1 and 16.", new[] { nameof(WinningNumbers) });
                    break;
                }
            }

            if (WinningNumbers.Count != new HashSet<int>(WinningNumbers).Count)
            {
                yield return new ValidationResult("WinningNumbers must be unique.", new[] { nameof(WinningNumbers) });
            }
        }
    }
}