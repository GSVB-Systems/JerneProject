using System.ComponentModel.DataAnnotations;

namespace Contracts.BoardNumberDTOs
{
    public class CreateBoardNumberDto : IValidatableObject
    {
        public int Number { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Number < 1 || Number > 16)
            {
                yield return new ValidationResult("Number must be between 1 and 16.", new[] { nameof(Number) });
            }
        }
    }
}