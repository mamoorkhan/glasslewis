using Asp.Versioning;
using GlassLewis.Application.Dtos.Requests.Company;
using GlassLewis.Application.Dtos.Responses.Company;
using GlassLewis.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace GlassLewis.Api.Controllers.v1;

/// <summary>
/// Controller for managing company-related operations.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes:Read")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<CompanyController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyController"/> class.
    /// </summary>
    /// <param name="companyService">The service for company operations.</param>
    /// <param name="logger">The logger instance.</param>
    public CompanyController(ICompanyService companyService, ILogger<CompanyController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all companies.
    /// </summary>
    /// <returns>A list of companies.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetCompanyResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetCompanyResponseDto>>> GetAllCompanies()
    {
        try
        {
            var companies = await _companyService.GetAllCompaniesAsync();
            return Ok(companies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all companies");
            return StatusCode(500, "An error occurred while retrieving companies");
        }
    }

    /// <summary>
    /// Retrieves a company by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the company.</param>
    /// <returns>The company details.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetCompanyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetCompanyResponseDto>> GetCompanyById(Guid id)
    {
        try
        {
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
            {
                return NotFound($"Company with ID {id} not found");
            }

            return Ok(company);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company with ID {CompanyId}", id);
            return StatusCode(500, "An error occurred while retrieving the company");
        }
    }

    /// <summary>
    /// Retrieves a company by its ISIN.
    /// </summary>
    /// <param name="isin">The ISIN of the company.</param>
    /// <returns>The company details.</returns>
    [HttpGet("isin/{isin}")]
    [ProducesResponseType(typeof(GetCompanyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetCompanyResponseDto>> GetCompanyByIsin(string isin)
    {
        try
        {
            var company = await _companyService.GetCompanyByIsinAsync(isin);
            if (company == null)
            {
                return NotFound($"Company with ISIN {isin} not found");
            }

            return Ok(company);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company with ISIN {Isin}", isin);
            return StatusCode(500, "An error occurred while retrieving the company");
        }
    }

    /// <summary>
    /// Creates a new company.
    /// </summary>
    /// <param name="request">The request containing company details.</param>
    /// <returns>The created company details.</returns>
    [HttpPost]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes:Write")]
    [ProducesResponseType(typeof(CreateCompanyResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateCompanyResponseDto>> CreateCompany([FromBody] CreateCompanyRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _companyService.CreateCompanyAsync(request);
            var version = HttpContext?.GetRequestedApiVersion()?.ToString() ?? "1.0";
            return CreatedAtAction(nameof(GetCompanyById), new { version, id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating company");
            return StatusCode(500, "An error occurred while creating the company");
        }
    }

    /// <summary>
    /// Updates an existing company.
    /// </summary>
    /// <param name="id">The unique identifier of the company.</param>
    /// <param name="request">The request containing updated company details.</param>
    /// <returns>The updated company details.</returns>
    [HttpPut("{id:guid}")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes:Write")]
    [ProducesResponseType(typeof(UpdateCompanyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateCompanyResponseDto>> UpdateCompany(Guid id, [FromBody] UpdateCompanyRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _companyService.UpdateCompanyAsync(id, request);
            if (response == null)
            {
                return NotFound($"Company with ID {id} not found");
            }

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company");
            return StatusCode(500, "An error occurred while updating the company");
        }
    }

    /// <summary>
    /// Partially updates a company's details.
    /// </summary>
    /// <param name="id">The unique identifier of the company to patch.</param>
    /// <param name="request">The patch request containing the fields to update.</param>
    /// <returns>The patched company details.</returns>
    [HttpPatch("{id:guid}")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes:Write")]
    [ProducesResponseType(typeof(PatchCompanyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PatchCompanyResponseDto>> PatchCompany(Guid id, [FromBody] PatchCompanyRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _companyService.PatchCompanyAsync(id, request);
            if (response == null)
            {
                return NotFound($"Company with ID {id} not found");
            }

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error patching company");
            return StatusCode(500, "An error occurred while patching the company");
        }
    }

    /// <summary>
    /// Deletes a company by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the company to delete.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpDelete("{id:guid}")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes:Write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        try
        {
            var deleted = await _companyService.DeleteCompanyAsync(id);
            if (!deleted)
            {
                return NotFound($"Company with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting company");
            return StatusCode(500, "An error occurred while deleting the company");
        }
    }
}
