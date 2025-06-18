namespace GlassLewis.Api.SystemTests.Models.Responses.Company;

/// <summary>
/// Represents the response model for patching a company's information.
/// </summary>
public class PatchCompanyResponseModel
{
    /// <summary>
    /// Gets the name of the company.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets the stock ticker of the company.
    /// </summary>
    public string? StockTicker { get; set; }

    /// <summary>
    /// Gets the exchange where the company's stock is listed.
    /// </summary>
    public string? Exchange { get; set; }

    /// <summary>
    /// Gets the ISIN (International Securities Identification Number) of the company.
    /// </summary>
    public string? Isin { get; set; }

    /// <summary>
    /// Gets the website of the company.
    /// </summary>
    public string? Website { get; set; }
}
