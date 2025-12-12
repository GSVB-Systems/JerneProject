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

            if (WinningNumbers.Count != new HashSet<int>(WinningNumbers).Count)
            {
                yield return new ValidationResult("WinningNumbers must be unique.", new[] { nameof(WinningNumbers) });
            }
        }
    }
}