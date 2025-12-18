using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contracts.WinningBoardDTOs
{
    public class UpdateWinningBoardDto : IValidatableObject
    {
        [Required]
        public List<int> WinningNumbers { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (WinningNumbers == null)
            {
                yield return new ValidationResult("Udfyld felterne med de trukkede numre.", new[] { nameof(WinningNumbers) });
                yield break;
            }

            var count = WinningNumbers.Count;
            if (count != 3 && count != 5)
            {
                yield return new ValidationResult("Der må kun trækkes 3 eller 5 numre", new[] { nameof(WinningNumbers) });
            }

            foreach (var n in WinningNumbers)
            {
                if (n < 1 || n > 16)
                {
                    yield return new ValidationResult("De trukkede numre må kun være mellem 1 og 16", new[] { nameof(WinningNumbers) });
                    break;
                }
            }

            if (WinningNumbers.Count != new HashSet<int>(WinningNumbers).Count)
            {
                yield return new ValidationResult("De trukkede numre skal være unikke", new[] { nameof(WinningNumbers) });
            }
        }
    }
}