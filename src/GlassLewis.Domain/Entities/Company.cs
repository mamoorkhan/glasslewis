using System.ComponentModel.DataAnnotations;

namespace GlassLewis.Domain.Entities;

/// <summary>  
/// Represents a company entity with details such as name, stock ticker, exchange, ISIN, website, and timestamps.  
/// </summary>  
public class Company
{
    /// <summary>  
    /// Gets or sets the unique identifier for the company.  
    /// </summary>  
    [Key]
    public Guid Id { get; set; }

    /// <summary>  
    /// Gets or sets the name of the company.  
    /// </summary>  
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>  
    /// Gets or sets the stock ticker symbol of the company.  
    /// </summary>  
    [Required]
    [StringLength(10)]
    public string StockTicker { get; set; } = string.Empty;

    /// <summary>  
    /// Gets or sets the exchange where the company's stock is listed.  
    /// </summary>  
    [Required]
    [StringLength(100)]
    public string Exchange { get; set; } = string.Empty;

    /// <summary>  
    /// Gets or sets the International Securities Identification Number (ISIN) of the company.  
    /// </summary>  
    [Required]
    [StringLength(12, MinimumLength = 12)]
    public string Isin { get; set; } = string.Empty;

    /// <summary>  
    /// Gets or sets the website URL of the company.  
    /// </summary>  
    [Url]
    [StringLength(500)]
    public string? Website { get; set; }

    /// <summary>  
    /// Gets or sets the timestamp when the company entity was created.  
    /// </summary>  
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>  
    /// Gets or sets the timestamp when the company entity was last updated.  
    /// </summary>  
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}