using AutoMapper;
using GlassLewis.Application.Dtos.Requests.Company;
using GlassLewis.Application.Dtos.Responses.Company;
using GlassLewis.Domain.Entities;
using GlassLewis.Domain.Interfaces;

namespace GlassLewis.Application.Services;

/// <summary>
/// Service for managing company-related operations.
/// </summary>
public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyService"/> class.
    /// </summary>
    /// <param name="companyRepository">The repository for company data access.</param>
    /// <param name="mapper">The mapper for object transformations.</param>
    public CompanyService(ICompanyRepository companyRepository, IMapper mapper)
    {
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all companies.
    /// </summary>
    /// <returns>A collection of company response DTOs.</returns>
    public async Task<IEnumerable<GetCompanyResponseDto>> GetAllCompaniesAsync()
    {
        var companies = await _companyRepository.GetAllAsync();
        return companies.Select(_mapper.Map<GetCompanyResponseDto>);
    }

    /// <summary>
    /// Retrieves a company by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the company.</param>
    /// <returns>A company response DTO or null if not found.</returns>
    public async Task<GetCompanyResponseDto?> GetCompanyByIdAsync(Guid id)
    {
        var company = await _companyRepository.GetByIdAsync(id);
        return company == null ? null : _mapper.Map<GetCompanyResponseDto>(company);
    }

    /// <summary>
    /// Retrieves a company by its ISIN.
    /// </summary>
    /// <param name="isin">The ISIN of the company.</param>
    /// <returns>A company response DTO or null if not found.</returns>
    public async Task<GetCompanyResponseDto?> GetCompanyByIsinAsync(string isin)
    {
        var company = await _companyRepository.GetByIsinAsync(isin);
        return company == null ? null : _mapper.Map<GetCompanyResponseDto>(company);
    }

    /// <summary>
    /// Creates a new company.
    /// </summary>
    /// <param name="companyDto">The DTO containing company creation details.</param>
    /// <returns>The response DTO of the created company.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a company with the same ISIN already exists.</exception>
    public async Task<CreateCompanyResponseDto> CreateCompanyAsync(CreateCompanyRequestDto companyDto)
    {
        if (await _companyRepository.ExistsByIsinAsync(companyDto.Isin))
        {
            throw new InvalidOperationException($"A company with ISIN {companyDto.Isin} already exists.");
        }

        var company = _mapper.Map<Company>(companyDto);
        var createdCompany = await _companyRepository.CreateAsync(company);
        return _mapper.Map<CreateCompanyResponseDto>(createdCompany);
    }

    /// <summary>
    /// Updates an existing company.
    /// </summary>
    /// <param name="id">The unique identifier of the company.</param>
    /// <param name="companyDto">The DTO containing company update details.</param>
    /// <returns>The response DTO of the updated company or null if not found.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a company with the same ISIN already exists.</exception>
    public async Task<UpdateCompanyResponseDto?> UpdateCompanyAsync(Guid id, UpdateCompanyRequestDto companyDto)
    {
        var existingCompany = await _companyRepository.GetByIdAsync(id);
        if (existingCompany == null)
        {
            return null;
        }

        if (await _companyRepository.ExistsByIsinAsync(companyDto.Isin, id))
        {
            throw new InvalidOperationException($"A company with ISIN {companyDto.Isin} already exists.");
        }

        var updatedCompany = _mapper.Map(companyDto, existingCompany);
        var result = await _companyRepository.UpdateAsync(id, updatedCompany);
        return _mapper.Map<UpdateCompanyResponseDto>(result);
    }

    /// <summary>
    /// Partially updates an existing company.
    /// </summary>
    /// <param name="id">The unique identifier of the company.</param>
    /// <param name="companyDto">The DTO containing partial update details.</param>
    /// <returns>The response DTO of the patched company or null if not found.</returns>
    /// <exception cref="ArgumentException">Thrown if validation fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown if a company with the same ISIN already exists.</exception>
    public async Task<PatchCompanyResponseDto?> PatchCompanyAsync(Guid id, PatchCompanyRequestDto companyDto)
    {
        var validationResults = companyDto.Validate().ToList();
        if (validationResults.Count != 0)
        {
            var errorMessage = string.Join("; ", validationResults.Select(vr => vr.ErrorMessage));
            throw new ArgumentException($"Validation failed: {errorMessage}");
        }

        var existingCompany = await _companyRepository.GetByIdAsync(id);
        if (existingCompany == null)
        {
            return null;
        }

        if (companyDto.HasProperty(nameof(companyDto.Isin)) &&
            !string.IsNullOrEmpty(companyDto.Isin) &&
            companyDto.Isin != existingCompany.Isin)
        {
            if (await _companyRepository.ExistsByIsinAsync(companyDto.Isin, id))
            {
                throw new InvalidOperationException($"A company with ISIN {companyDto.Isin} already exists.");
            }
        }

        var fieldsToUpdate = new Dictionary<string, object?>();

        foreach (var propertyName in companyDto.GetProvidedProperties())
        {
            fieldsToUpdate[propertyName] = companyDto.GetPropertyValue(propertyName);
        }

        var result = await _companyRepository.PatchAsync(id, fieldsToUpdate);
        return _mapper.Map<PatchCompanyResponseDto>(result);
    }

    /// <summary>
    /// Deletes a company by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the company.</param>
    /// <returns>True if the company was deleted successfully; otherwise, false.</returns>
    public async Task<bool> DeleteCompanyAsync(Guid id)
    {
        return await _companyRepository.DeleteAsync(id);
    }
}
