using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GlassLewis.Application.Attributes.Validation;

/// <summary>
/// Validation attribute to ensure a string conforms to the ISIN (International Securities Identification Number) format.
/// </summary>
public class IsinValidationAttribute : ValidationAttribute
{
    /// <summary>
    /// Regular expression to validate ISIN format.
    /// </summary>
    private static readonly Regex IsinRegex = new(@"^[A-Z]{2}[A-Z0-9]{9}[0-9]{1}$", RegexOptions.Compiled);

    /// <summary>
    /// Validates whether the provided value matches the ISIN format.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="validationContext">The context of the validation operation.</param>
    /// <returns>A <see cref="ValidationResult"/> indicating whether the value is valid.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var isin = value.ToString() ?? string.Empty;

        if (!IsinRegex.IsMatch(isin))
        {
            return new ValidationResult("ISIN format is invalid. It must start with two letters followed by 9 alphanumeric characters and end with a digit", [validationContext?.MemberName!]);
        }

        return ValidationResult.Success;
    }
}
