using System.ComponentModel.DataAnnotations;
using GlassLewis.Application.Attributes.Validation;

namespace GlassLewis.Application.Dtos.Requests.Company;

/// <summary>
/// Represents a DTO for patching company details.
/// </summary>
public class PatchCompanyRequestDto
{
    private readonly Dictionary<string, object?> _values = [];

    /// <summary>
    /// Sets the name of the company.
    /// </summary>
    public string? Name
    {
        get => GetValue<string>(nameof(Name));
        set => SetValue(nameof(Name), value);
    }

    /// <summary>
    /// Sets the stock ticker of the company.
    /// </summary>
    public string? StockTicker
    {
        get => GetValue<string>(nameof(StockTicker));
        set => SetValue(nameof(StockTicker), value);
    }

    /// <summary>
    /// Sets the exchange where the company is listed.
    /// </summary>
    public string? Exchange
    {
        get => GetValue<string>(nameof(Exchange));
        set => SetValue(nameof(Exchange), value);
    }

    /// <summary>
    /// Sets the ISIN (International Securities Identification Number) of the company.
    /// </summary>
    public string? Isin
    {
        get => GetValue<string>(nameof(Isin));
        set => SetValue(nameof(Isin), value);
    }

    /// <summary>
    /// Sets the website URL of the company.
    /// </summary>
    public string? Website
    {
        get => GetValue<string>(nameof(Website));
        set => SetValue(nameof(Website), value);
    }

    private T? GetValue<T>(string propertyName)
    {
        return _values.TryGetValue(propertyName, out object? value) ? (T?)value : default;
    }

    private void SetValue(string propertyName, object? value)
    {
        _values[propertyName] = value;
    }

    /// <summary>
    /// Checks if a property has been provided.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>True if the property has been provided; otherwise, false.</returns>
    public bool HasProperty(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) return false;
        return _values.ContainsKey(propertyName);
    }

    /// <summary>
    /// Gets the value of a provided property.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The value of the property, or null if not provided.</returns>
    public object? GetPropertyValue(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) return null;
        return _values.TryGetValue(propertyName, out var value) ? value : null;
    }

    /// <summary>
    /// Gets the names of all provided properties.
    /// </summary>
    /// <returns>An enumerable of property names.</returns>
    public IEnumerable<string> GetProvidedProperties()
    {
        return _values.Keys;
    }

    /// <summary>
    /// Validates the provided properties based on custom rules.
    /// </summary>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate()
    {
        var results = new List<ValidationResult>();

        if (HasProperty(nameof(Name)))
        {
            var name = Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                results.Add(new ValidationResult("Name cannot be null or whitespace", [nameof(Name)]));
            }
            else if (name.Length > 200)
            {
                results.Add(new ValidationResult("Name cannot exceed 200 characters", [nameof(Name)]));
            }
        }

        if (HasProperty(nameof(StockTicker)))
        {
            var stockTicker = StockTicker;
            if (stockTicker?.Length > 10)
            {
                results.Add(new ValidationResult("Stock ticker cannot exceed 10 characters", [nameof(StockTicker)]));
            }
        }

        if (HasProperty(nameof(Exchange)))
        {
            var exchange = Exchange;
            if (exchange?.Length > 100)
            {
                results.Add(new ValidationResult("Exchange cannot exceed 100 characters", [nameof(Exchange)]));
            }
        }

        if (HasProperty(nameof(Isin)))
        {
            var isin = Isin;
            if (!string.IsNullOrEmpty(isin))
            {
                var isinValidator = new IsinValidationAttribute();
                if (!isinValidator.IsValid(isin))
                {
                    results.Add(new ValidationResult("Invalid ISIN format", [nameof(Isin)]));
                }
            }
        }

        if (HasProperty(nameof(Website)))
        {
            var website = Website;
            if (!string.IsNullOrEmpty(website))
            {
                if (website.Length > 500)
                {
                    results.Add(new ValidationResult("Website URL cannot exceed 500 characters", [nameof(Website)]));
                }

                if (!Uri.TryCreate(website, UriKind.Absolute, out _))
                {
                    results.Add(new ValidationResult("Website must be a valid URL", [nameof(Website)]));
                }
            }
        }

        return results;
    }
}
