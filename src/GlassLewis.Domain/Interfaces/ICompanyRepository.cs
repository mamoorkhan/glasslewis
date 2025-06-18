using GlassLewis.Domain.Entities;

namespace GlassLewis.Domain.Interfaces;

/// <summary>
/// Repository interface for managing Company entities.
/// </summary>
public interface ICompanyRepository
{
    /// <summary>
    /// Retrieves all companies.
    /// </summary>
    /// <returns>A collection of all companies.</returns>
    Task<IEnumerable<Company>> GetAllAsync();

    /// <summary>
    /// Retrieves a company by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the company.</param>
    /// <returns>The company if found; otherwise, null.</returns>
    Task<Company?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a company by its ISIN (International Securities Identification Number).
    /// </summary>
    /// <param name="isin">The ISIN of the company.</param>
    /// <returns>The company if found; otherwise, null.</returns>
    Task<Company?> GetByIsinAsync(string isin);

    /// <summary>
    /// Creates a new company.
    /// </summary>
    /// <param name="company">The company to create.</param>
    /// <returns>The created company.</returns>
    Task<Company> CreateAsync(Company company);

    /// <summary>
    /// Updates an existing company.
    /// </summary>
    /// <param name="id">The unique identifier of the company to update.</param>
    /// <param name="company">The updated company details.</param>
    /// <returns>The updated company if successful; otherwise, null.</returns>
    Task<Company?> UpdateAsync(Guid id, Company company);

    /// <summary>
    /// Partially updates an existing company.
    /// </summary>
    /// <param name="id">The unique identifier of the company to update.</param>
    /// <param name="fieldsToUpdate">A dictionary of fields to update and their new values.</param>
    /// <returns>The updated company if successful; otherwise, null.</returns>
    Task<Company?> PatchAsync(Guid id, Dictionary<string, object?> fieldsToUpdate);

    /// <summary>
    /// Deletes a company by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the company to delete.</param>
    /// <returns>True if the deletion was successful; otherwise, false.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Checks if a company exists by its ISIN.
    /// </summary>
    /// <param name="isin">The ISIN of the company.</param>
    /// <returns>True if the company exists; otherwise, false.</returns>
    Task<bool> ExistsByIsinAsync(string isin);

    /// <summary>
    /// Checks if a company exists by its ISIN, excluding a specific company by its unique identifier.
    /// </summary>
    /// <param name="isin">The ISIN of the company.</param>
    /// <param name="excludeId">The unique identifier of the company to exclude.</param>
    /// <returns>True if the company exists; otherwise, false.</returns>
    Task<bool> ExistsByIsinAsync(string isin, Guid excludeId);
}