using GlassLewis.Domain.Entities;
using GlassLewis.Domain.Interfaces;
using GlassLewis.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GlassLewis.Infrastructure.Repositories;

/// <summary>
/// Repository for managing Company entities.
/// Provides methods for CRUD operations and additional functionalities.
/// </summary>
public class CompanyRepository : ICompanyRepository
{
    private readonly CompanyDbContext _context;
    private readonly ILogger<CompanyRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for Company entities.</param>
    /// <param name="logger">The logger instance for logging operations.</param>
    public CompanyRepository(CompanyDbContext context, ILogger<CompanyRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all companies ordered by their name.
    /// </summary>
    /// <returns>A collection of all companies.</returns>
    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        try
        {
            return await _context.Companies
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all companies");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a company by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the company.</param>
    /// <returns>The company if found; otherwise, null.</returns>
    public async Task<Company?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company with ID {CompanyId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a company by its ISIN (International Securities Identification Number).
    /// </summary>
    /// <param name="isin">The ISIN of the company.</param>
    /// <returns>The company if found; otherwise, null.</returns>
    public async Task<Company?> GetByIsinAsync(string isin)
    {
        try
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Isin == isin);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company with ISIN {Isin}", isin);
            throw;
        }
    }

    /// <summary>
    /// Creates a new company entity.
    /// </summary>
    /// <param name="company">The company entity to create.</param>
    /// <returns>The created company entity.</returns>
    public async Task<Company> CreateAsync(Company company)
    {
        try
        {
            company.CreatedAt = DateTime.UtcNow;
            company.UpdatedAt = DateTime.UtcNow;

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created company with ID {CompanyId}", company.Id);
            return company;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating company with ISIN {Isin}", company.Isin);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing company entity.
    /// </summary>
    /// <param name="id">The unique identifier of the company to update.</param>
    /// <param name="company">The updated company entity.</param>
    /// <returns>The updated company entity if found; otherwise, null.</returns>
    public async Task<Company?> UpdateAsync(Guid id, Company company)
    {
        try
        {
            var existingCompany = await _context.Companies.FindAsync(id);
            if (existingCompany == null)
                return null;

            existingCompany.Name = company.Name;
            existingCompany.StockTicker = company.StockTicker;
            existingCompany.Exchange = company.Exchange;
            existingCompany.Isin = company.Isin;
            existingCompany.Website = company.Website;
            existingCompany.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated company with ID {CompanyId}", id);
            return existingCompany;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company with ID {CompanyId}", id);
            throw;
        }
    }

    /// <summary>
    /// Partially updates an existing company entity.
    /// </summary>
    /// <param name="id">The unique identifier of the company to patch.</param>
    /// <param name="fieldsToUpdate">A dictionary of fields and their new values.</param>
    /// <returns>The patched company entity if found; otherwise, null.</returns>
    public async Task<Company?> PatchAsync(Guid id, Dictionary<string, object?> fieldsToUpdate)
    {
        try
        {
            var existingCompany = await _context.Companies.FindAsync(id);
            if (existingCompany is null)
                return null;

            if (fieldsToUpdate is null || fieldsToUpdate.Count == 0)
            {
                _logger.LogWarning("No fields provided for patching company with ID {CompanyId}", id);
                return existingCompany;
            }

            var companyType = typeof(Company);
            var updatedFields = new List<string>();

            var allowedFields = new HashSet<string>
            {
                nameof(Company.Name),
                nameof(Company.StockTicker),
                nameof(Company.Exchange),
                nameof(Company.Isin),
                nameof(Company.Website)
            };

            foreach (var (fieldName, fieldValue) in fieldsToUpdate)
            {
                if (string.IsNullOrWhiteSpace(fieldName))
                {
                    _logger.LogWarning("Empty field name provided for patching company with ID {CompanyId}", id);
                    continue;
                }

                if (!allowedFields.Contains(fieldName))
                {
                    _logger.LogWarning("Field '{FieldName}' is not allowed for patching company with ID {CompanyId}",
                        fieldName, id);
                    continue;
                }

                var propertyInfo = companyType.GetProperty(fieldName);
                if (propertyInfo is null || !propertyInfo.CanWrite)
                {
                    _logger.LogWarning("Property '{PropertyName}' not found or not writable for company with ID {CompanyId}",
                        fieldName, id);
                    continue;
                }

                try
                {
                    var convertedValue = ConvertValueForProperty(fieldValue, propertyInfo.PropertyType);
                    propertyInfo.SetValue(existingCompany, convertedValue);
                    updatedFields.Add(fieldName);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to set property '{PropertyName}' for company with ID {CompanyId}",
                        fieldName, id);
                }
            }

            if (updatedFields.Count == 0)
            {
                _logger.LogInformation("No valid fields updated for company with ID {CompanyId}", id);
                return existingCompany;
            }

            existingCompany.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Patched company with ID {CompanyId}. Updated fields: {UpdatedFields}",
                id, string.Join(", ", updatedFields));

            return existingCompany;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error patching company with ID {CompanyId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a company entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the company to delete.</param>
    /// <returns>True if the company was deleted; otherwise, false.</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
                return false;

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted company with ID {CompanyId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting company with ID {CompanyId}", id);
            throw;
        }
    }

    /// <summary>
    /// Checks if a company exists by its ISIN.
    /// </summary>
    /// <param name="isin">The ISIN of the company.</param>
    /// <returns>True if the company exists; otherwise, false.</returns>
    public async Task<bool> ExistsByIsinAsync(string isin)
    {
        try
        {
            return await _context.Companies.AnyAsync(c => c.Isin == isin);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if company exists with ISIN {Isin}", isin);
            throw;
        }
    }

    /// <summary>
    /// Checks if a company exists by its ISIN, excluding a specific ID.
    /// </summary>
    /// <param name="isin">The ISIN of the company.</param>
    /// <param name="excludeId">The ID to exclude from the check.</param>
    /// <returns>True if the company exists; otherwise, false.</returns>
    public async Task<bool> ExistsByIsinAsync(string isin, Guid excludeId)
    {
        try
        {
            return await _context.Companies.AnyAsync(c => c.Isin == isin && c.Id != excludeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if company exists with ISIN {Isin} excluding ID {ExcludeId}", isin, excludeId);
            throw;
        }
    }

    /// <summary>
    /// Safely converts a value to the target property type.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="targetType">The target property type.</param>
    /// <returns>The converted value.</returns>
    private static object? ConvertValueForProperty(object? value, Type targetType)
    {
        if (value is null)
            return null;

        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlyingType == typeof(string))
        {
            var stringValue = value.ToString();
            return string.IsNullOrWhiteSpace(stringValue) ? null : stringValue.Trim();
        }

        try
        {
            return Convert.ChangeType(value, underlyingType, System.Globalization.CultureInfo.InvariantCulture);
        }
        catch
        {
            throw new InvalidCastException($"Cannot convert value '{value}' to type '{targetType.Name}'");
        }
    }
}
