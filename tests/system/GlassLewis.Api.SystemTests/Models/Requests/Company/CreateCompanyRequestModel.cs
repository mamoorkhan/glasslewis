using System.ComponentModel.DataAnnotations;

namespace GlassLewis.Api.SystemTests.Models.Requests.Company;

/// <summary>  
/// Represents a request to create a company.  
/// </summary>  
public class CreateCompanyRequestModel
{
    /// <summary>  
    /// Aets the name of the company.  
    /// </summary>  
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>  
    /// Sets the stock ticker of the company.  
    /// </summary>  
    public string StockTicker { get; set; } = string.Empty;

    /// <summary>  
    /// Sets the exchange where the company is listed.  
    /// </summary>  
    public string Exchange { get; set; } = string.Empty;

    /// <summary>  
    /// Sets the ISIN (International Securities Identification Number) of the company.  
    /// </summary>  
    public string Isin { get; set; } = string.Empty;

    /// <summary>  
    ///Sets the website URL of the company.  
    /// </summary>  
    public string? Website { get; set; }
}
