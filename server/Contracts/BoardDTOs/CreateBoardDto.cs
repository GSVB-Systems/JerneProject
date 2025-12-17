using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contracts.BoardDTOs
{
    public class CreateBoardDto : IValidatableObject
    {
        [Required]
        [Range(5, 8, ErrorMessage = "BoardSize must be between 5 and 8.")]
        public int BoardSize { get; set; }

        public int Week { get; set; }

        [Required]
        [MinLength(1)]
        public string UserID { get; set; } = string.Empty;

        [Required]
        public List<int> Numbers { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Numbers == null)
            {
                yield return new ValidationResult("Numbers is required.", new[] { nameof(Numbers) });
                yield break;
            }

            if (Numbers.Count != BoardSize)
            {
                yield return new ValidationResult($"Selected Numbers must match your board size: ({BoardSize}).", new[] { nameof(Numbers), nameof(BoardSize) });
            }

            foreach (var n in Numbers)
            {
                if (n < 1 || n > 16)
                {
                    yield return new ValidationResult("Each number must be between 1 and 16.", new[] { nameof(Numbers) });
                    break;
                }
            }
        }
    }
}