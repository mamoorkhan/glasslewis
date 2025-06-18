using System.ComponentModel.DataAnnotations;
using GlassLewis.Application.Attributes.Validation;

namespace GlassLewis.Application.Dtos.Requests.Company;

/// <summary>
/// Represents a request to update company details.
/// </summary>
public class UpdateCompanyRequestDto
{
    /// <summary>
    /// Sets the name of the company.
    /// </summary>
    [Required]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Sets the stock ticker of the company.
    /// </summary>
    [Required]
    [StringLength(10, ErrorMessage = "Stock ticker cannot exceed 10 characters")]
    public string StockTicker { get; set; } = string.Empty;

    /// <summary>
    /// Sets the exchange where the company is listed.
    /// </summary>
    [Required]
    [StringLength(100, ErrorMessage = "Exchange cannot exceed 100 characters")]
    public string Exchange { get; set; } = string.Empty;

    /// <summary>
    /// Sets the ISIN (International Securities Identification Number) of the company.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [IsinValidation]
    public string Isin { get; set; } = string.Empty;

    /// <summary>
    /// Sets the website URL of the company.
    /// </summary>
    [Url(ErrorMessage = "Website must be a valid URL")]
    [StringLength(500, ErrorMessage = "Website URL cannot exceed 500 characters")]
    public string? Website { get; set; }
}
