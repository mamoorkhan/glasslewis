using System.Text.Json.Serialization;

namespace GlassLewis.Api.SystemTests.Models.Responses.Company;

/// <summary>
/// Represents the response model for creating a company.
/// </summary>
public class CreateCompanyResponseModel
{
    /// <summary>
    /// Gets the unique identifier of the company.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets the name of the company.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the stock ticker symbol of the company.
    /// </summary>
    [JsonPropertyName("stockTicker")]
    public string StockTicker { get; set; } = string.Empty;

    /// <summary>
    /// Gets the stock exchange where the company is listed.
    /// </summary>
    [JsonPropertyName("exchange")]
    public string Exchange { get; set; } = string.Empty;

    /// <summary>
    /// Gets the International Securities Identification Number (ISIN) of the company.
    /// </summary>
    [JsonPropertyName("isin")]
    public string Isin { get; set; } = string.Empty;

    /// <summary>
    /// Gets the website URL of the company.
    /// </summary>
    [JsonPropertyName("website")]
    public string? Website { get; set; }
}
