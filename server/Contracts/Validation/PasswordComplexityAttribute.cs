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
            $"Adgangskoden skal minimum være {MinimumLength} Karakterer lang og indeholde mindst et stort bogstav, et lille bogstav, et tal og et specialtegn.";
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

