namespace GlassLewis.Application.Dtos.Responses.Company;

/// <summary>
/// Represents the response DTO for creating a company.
/// </summary>
public class CreateCompanyResponseDto
{
    /// <summary>
    /// Gets the unique identifier of the company.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets the name of the company.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the stock ticker symbol of the company.
    /// </summary>
    public string StockTicker { get; set; } = string.Empty;

    /// <summary>
    /// Gets the stock exchange where the company is listed.
    /// </summary>
    public string Exchange { get; set; } = string.Empty;

    /// <summary>
    /// Gets the International Securities Identification Number (ISIN) of the company.
    /// </summary>
    public string Isin { get; set; } = string.Empty;

    /// <summary>
    /// Gets the website URL of the company.
    /// </summary>
    public string? Website { get; set; }
}
