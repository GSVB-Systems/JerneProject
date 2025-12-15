using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contracts.BoardDTOs
{
    public class CreateBoardDto : IValidatableObject
    {
        [Required]
        public int BoardSize { get; set; }

        public int Week { get; set; }

        [Required]
        public string UserID { get; set; } = string.Empty;

        [Required]
        public List<int> Numbers { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BoardSize <= 0)
            {
                yield return new ValidationResult("Board must be between 5-8", new[] { nameof(BoardSize) });
            }

            if (Numbers == null)
            {
                yield return new ValidationResult("Numbers is required.", new[] { nameof(Numbers) });
                yield break;
            }

            if (Numbers.Count != BoardSize)
            {
                yield return new ValidationResult($"Selected Numbers must match your board size: ({BoardSize}).", new[] { nameof(Numbers), nameof(BoardSize) });
            }
        }
    }
}