using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Contracts.BoardNumberDTOs;

namespace Contracts.BoardDTOs
{
    public class UpdateBoardDto : IValidatableObject
    {
        public int? BoardSize { get; set; }
        public bool? IsActive { get; set; }
        public int? Week { get; set; }
        
        public List<CreateBoardNumberDto> Numbers { get; set; } = null;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BoardSize.HasValue && (BoardSize.Value < 5 || BoardSize.Value > 8))
            {
                yield return new ValidationResult("BoardSize must be between 5 and 8.", new[] { nameof(BoardSize) });
            }

            if (Numbers != null)
            {
                // If BoardSize provided, Numbers count must match
                if (BoardSize.HasValue && Numbers.Count != BoardSize.Value)
                {
                    yield return new ValidationResult($"Selected Numbers must match your board size: ({BoardSize}).", new[] { nameof(Numbers), nameof(BoardSize) });
                }

                // validate each number
                foreach (var num in Numbers)
                {
                    if (num.Number < 1 || num.Number > 16)
                    {
                        yield return new ValidationResult("Each number must be between 1 and 16.", new[] { nameof(Numbers) });
                        break;
                    }
                }

                // uniqueness when IsRepeating is explicitly false
                var repeat = IsRepeating.GetValueOrDefault(false);
                if (!repeat)
                {
                    var values = Numbers.Select(n => n.Number).ToList();
                    if (values.Count != new HashSet<int>(values).Count)
                    {
                        yield return new ValidationResult("Numbers must be unique.", new[] { nameof(Numbers) });
                    }
                }
            }
        }
    }
}