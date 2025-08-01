namespace GlassLewis.Application.Dtos.Responses.Company;

/// <summary>
/// Represents the response DTO for updating a company.
/// </summary>
public class UpdateCompanyResponseDto
{
    /// <summary>
    /// Gets the name of the company.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the stock ticker of the company.
    /// </summary>
    public string StockTicker { get; set; } = string.Empty;

    /// <summary>
    /// Gets the exchange where the company is listed.
    /// </summary>
    public string Exchange { get; set; } = string.Empty;

    /// <summary>
    /// Gets the ISIN (International Securities Identification Number) of the company.
    /// </summary>
    public string Isin { get; set; } = string.Empty;

    /// <summary>
    /// Gets the website of the company.
    /// </summary>
    public string? Website { get; set; }
}
