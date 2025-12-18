using System.ComponentModel.DataAnnotations;
using Contracts.Validation;

namespace Contracts.UserDTOs
{
    public class ResetPasswordDto
    {
        [Required]
        [PasswordComplexity(8)]
        public string NewPassword { get; set; } = string.Empty;
    }
}