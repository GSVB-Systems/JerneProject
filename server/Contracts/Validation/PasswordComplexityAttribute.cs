using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Contracts.Validation;


[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class PasswordComplexityAttribute : ValidationAttribute
{
    public int MinimumLength { get; }

    
    private readonly Regex _regex;

    public PasswordComplexityAttribute(int minimumLength = 8)
    {
        MinimumLength = minimumLength;
        var pattern = $"^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z])(?=\\S+$).{{{MinimumLength},}}$";
        _regex = new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.Compiled);

        ErrorMessage = ErrorMessage ??
            $"Password must be at least {MinimumLength} characters and include at least one uppercase letter, one lowercase letter, one digit, and one special character; spaces are not allowed.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        
        if (value is null)
        {
            return ValidationResult.Success; 
        }

        if (value is not string s)
        {
            return new ValidationResult(ErrorMessage);
        }

        if (s.Length == 0)
        {
            return ValidationResult.Success; 
        }

        return _regex.IsMatch(s) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
    }
}

