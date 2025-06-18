using GlassLewis.Application.Dtos.Requests.Company;
using GlassLewis.Application.Dtos.Responses.Company;

namespace GlassLewis.Application.Services;

/// <summary>  
/// Interface for managing company-related operations.  
/// </summary>  
public interface ICompanyService
{
    /// <summary>  
    /// Retrieves all companies.  
    /// </summary>  
    /// <returns>A collection of company response DTOs.</returns>  
    public Task<IEnumerable<GetCompanyResponseDto>> GetAllCompaniesAsync();

    /// <summary>  
    /// Retrieves a company by its unique identifier.  
    /// </summary>  
    /// <param name="id">The unique identifier of the company.</param>  
    /// <returns>A company response DTO if found; otherwise, null.</returns>  
    public Task<GetCompanyResponseDto?> GetCompanyByIdAsync(Guid id);

    /// <summary>  
    /// Retrieves a company by its ISIN (International Securities Identification Number).  
    /// </summary>  
    /// <param name="isin">The ISIN of the company.</param>  
    /// <returns>A company response DTO if found; otherwise, null.</returns>  
    public Task<GetCompanyResponseDto?> GetCompanyByIsinAsync(string isin);

    /// <summary>  
    /// Creates a new company.  
    /// </summary>  
    /// <param name="companyDto">The company creation request DTO.</param>  
    /// <returns>The response DTO for the created company.</returns>  
    public Task<CreateCompanyResponseDto> CreateCompanyAsync(CreateCompanyRequestDto companyDto);

    /// <summary>  
    /// Updates an existing company.  
    /// </summary>  
    /// <param name="id">The unique identifier of the company to update.</param>  
    /// <param name="companyDto">The company update request DTO.</param>  
    /// <returns>The response DTO for the updated company if successful; otherwise, null.</returns>  
    public Task<UpdateCompanyResponseDto?> UpdateCompanyAsync(Guid id, UpdateCompanyRequestDto companyDto);

    /// <summary>  
    /// Partially updates an existing company.  
    /// </summary>  
    /// <param name="id">The unique identifier of the company to patch.</param>  
    /// <param name="companyDto">The company patch request DTO.</param>  
    /// <returns>The response DTO for the patched company if successful; otherwise, null.</returns>  
    public Task<PatchCompanyResponseDto?> PatchCompanyAsync(Guid id, PatchCompanyRequestDto companyDto);

    /// <summary>  
    /// Deletes a company by its unique identifier.  
    /// </summary>  
    /// <param name="id">The unique identifier of the company to delete.</param>  
    /// <returns>True if the company was successfully deleted; otherwise, false.</returns>  
    public Task<bool> DeleteCompanyAsync(Guid id);
}
