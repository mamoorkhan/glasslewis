namespace GlassLewis.Application.Dtos.Responses.Company;

/// <summary>
/// Represents the response DTO for a company.
/// </summary>
public class GetCompanyResponseDto
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

    /// <summary>
    /// Gets the date and time when the company was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time when the company was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
