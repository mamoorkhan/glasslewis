namespace GlassLewis.Api.SystemTests.Models.Requests.Company;

/// <summary>
/// Represents a request for patching company details.
/// </summary>
public class PatchCompanyRequestModel
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
        return _values.ContainsKey(propertyName);
    }

    /// <summary>
    /// Gets the value of a provided property.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The value of the property, or null if not provided.</returns>
    public object? GetPropertyValue(string propertyName)
    {
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
}
