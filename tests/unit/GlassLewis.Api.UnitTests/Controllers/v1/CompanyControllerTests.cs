using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using GlassLewis.Api.Controllers.v1;
using GlassLewis.Application.Dtos.Requests.Company;
using GlassLewis.Application.Dtos.Responses.Company;
using GlassLewis.Application.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Connections;

namespace GlassLewis.Api.UnitTests.Controllers.v1;

/// <summary>
/// Provides unit tests for the <see cref="CompanyController"/> class, ensuring its methods behave as expected under
/// various conditions.
/// </summary>
/// <remarks>This test class includes comprehensive test cases for all public methods of the <see
/// cref="CompanyController"/>. It verifies correct behavior for successful operations, error handling, and edge cases
/// such as invalid input or exceptions thrown by the underlying service.</remarks>
[Trait("Category", "UnitTests")]
public class CompanyControllerTests
{
    private readonly Mock<ICompanyService> _mockCompanyService;
    private readonly Mock<ILogger<CompanyController>> _mockLogger;
    private readonly CompanyController _controller;

    // Test data
    private readonly Guid _testCompanyId = Guid.NewGuid();
    private readonly string _testIsin = "US1234567890";

    private readonly GetCompanyResponseDto _sampleCompanyResponse = new()
    {
        Id = Guid.NewGuid(),
        Name = "Test Company",
        StockTicker = "TEST",
        Exchange = "NYSE",
        Isin = "US1234567890",
        Website = "https://test.com",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    private readonly CreateCompanyRequestDto _sampleCreateRequest = new()
    {
        Name = "New Company",
        StockTicker = "NEW",
        Exchange = "NASDAQ",
        Isin = "US0987654321",
        Website = "https://newcompany.com"
    };

    private readonly CreateCompanyResponseDto _sampleCreateResponse = new()
    {
        Id = Guid.NewGuid(),
        Name = "New Company",
        StockTicker = "NEW",
        Exchange = "NASDAQ",
        Isin = "US0987654321",
        Website = "https://newcompany.com"
    };

    private readonly UpdateCompanyRequestDto _sampleUpdateRequest = new()
    {
        Name = "Updated Company",
        StockTicker = "UPD",
        Exchange = "NYSE",
        Isin = "US1111111111",
        Website = "https://updated.com"
    };

    private readonly UpdateCompanyResponseDto _sampleUpdateResponse = new()
    {
        Name = "Updated Company",
        StockTicker = "UPD",
        Exchange = "NYSE",
        Isin = "US1111111111",
        Website = "https://updated.com"
    };

    private readonly PatchCompanyRequestDto _samplePatchRequest = new()
    {
        Name = "Patched Company"
    };

    private readonly PatchCompanyResponseDto _samplePatchResponse = new()
    {
        Name = "Patched Company",
        StockTicker = "TEST",
        Exchange = "NYSE",
        Isin = "US1234567890",
        Website = "https://test.com"
    };

    public CompanyControllerTests()
    {
        _mockCompanyService = new Mock<ICompanyService>();
        _mockLogger = new Mock<ILogger<CompanyController>>();
        _controller = new CompanyController(_mockCompanyService.Object, _mockLogger.Object);

        // Setup HttpContext for API versioning
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        _controller.HttpContext.Features.Set<IApiVersioningFeature>(new ApiVersioningFeature
        {
            RequestedApiVersion = new ApiVersion(1, 0)
        });
    }

    #region GetAllCompanies Tests

    [Fact]
    public async Task GetAllCompanies_ReturnsOkWithCompanies_WhenServiceReturnsData()
    {
        // Arrange
        var companies = new List<GetCompanyResponseDto> { _sampleCompanyResponse };
        _mockCompanyService.Setup(s => s.GetAllCompaniesAsync())
            .ReturnsAsync(companies);

        // Act
        var result = await _controller.GetAllCompanies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCompanies = Assert.IsAssignableFrom<IEnumerable<GetCompanyResponseDto>>(okResult.Value);
        Assert.Single(returnedCompanies);
        _mockCompanyService.Verify(s => s.GetAllCompaniesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsOkWithEmptyList_WhenNoCompaniesExist()
    {
        // Arrange
        var companies = Enumerable.Empty<GetCompanyResponseDto>();
        _mockCompanyService.Setup(s => s.GetAllCompaniesAsync())
            .ReturnsAsync(companies);

        // Act
        var result = await _controller.GetAllCompanies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCompanies = Assert.IsType<IEnumerable<GetCompanyResponseDto>>(okResult.Value, exactMatch: false);
        Assert.Empty(returnedCompanies);
        _mockCompanyService.Verify(s => s.GetAllCompaniesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsInternalServerError_WhenServiceThrowsException()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.GetAllCompaniesAsync())
            .ThrowsAsync(new DataException("Database error"));

        // Act
        var result = await _controller.GetAllCompanies();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving companies", statusCodeResult.Value);
        VerifyLoggerWasCalled(LogLevel.Error, "Error retrieving all companies");
    }

    #endregion

    #region GetCompanyById Tests

    [Fact]
    public async Task GetCompanyById_ReturnsOkWithCompany_WhenCompanyExists()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.GetCompanyByIdAsync(_testCompanyId))
            .ReturnsAsync(_sampleCompanyResponse);

        // Act
        var result = await _controller.GetCompanyById(_testCompanyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCompany = Assert.IsType<GetCompanyResponseDto>(okResult.Value);
        Assert.Equal(_sampleCompanyResponse.Name, returnedCompany.Name);
        _mockCompanyService.Verify(s => s.GetCompanyByIdAsync(_testCompanyId), Times.Once);
    }

    [Fact]
    public async Task GetCompanyById_ReturnsNotFound_WhenCompanyDoesNotExist()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.GetCompanyByIdAsync(_testCompanyId))
            .ReturnsAsync((GetCompanyResponseDto?)null);

        // Act
        var result = await _controller.GetCompanyById(_testCompanyId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal($"Company with ID {_testCompanyId} not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetCompanyById_ReturnsInternalServerError_WhenServiceThrowsException()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.GetCompanyByIdAsync(_testCompanyId))
            .ThrowsAsync(new ConnectionAbortedException("Database error"));

        // Act
        var result = await _controller.GetCompanyById(_testCompanyId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving the company", statusCodeResult.Value);
        VerifyLoggerWasCalled(LogLevel.Error, $"Error retrieving company with ID {_testCompanyId}");
    }

    #endregion

    #region GetCompanyByIsin Tests

    [Fact]
    public async Task GetCompanyByIsin_ReturnsOkWithCompany_WhenCompanyExists()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.GetCompanyByIsinAsync(_testIsin))
            .ReturnsAsync(_sampleCompanyResponse);

        // Act
        var result = await _controller.GetCompanyByIsin(_testIsin);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCompany = Assert.IsType<GetCompanyResponseDto>(okResult.Value);
        Assert.Equal(_sampleCompanyResponse.Name, returnedCompany.Name);
        _mockCompanyService.Verify(s => s.GetCompanyByIsinAsync(_testIsin), Times.Once);
    }

    [Fact]
    public async Task GetCompanyByIsin_ReturnsNotFound_WhenCompanyDoesNotExist()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.GetCompanyByIsinAsync(_testIsin))
            .ReturnsAsync((GetCompanyResponseDto?)null);

        // Act
        var result = await _controller.GetCompanyByIsin(_testIsin);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal($"Company with ISIN {_testIsin} not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetCompanyByIsin_ReturnsInternalServerError_WhenServiceThrowsException()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.GetCompanyByIsinAsync(_testIsin))
            .ThrowsAsync(new ConnectionAbortedException("Database error"));

        // Act
        var result = await _controller.GetCompanyByIsin(_testIsin);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving the company", statusCodeResult.Value);
        VerifyLoggerWasCalled(LogLevel.Error, $"Error retrieving company with ISIN {_testIsin}");
    }

    #endregion

    #region CreateCompany Tests

    [Fact]
    public async Task CreateCompany_ReturnsCreatedAtAction_WhenCompanyIsCreatedSuccessfully()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.CreateCompanyAsync(_sampleCreateRequest))
            .ReturnsAsync(_sampleCreateResponse);

        // Act
        var result = await _controller.CreateCompany(_sampleCreateRequest);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.NotNull(createdAtActionResult.RouteValues);
        Assert.True(createdAtActionResult.RouteValues.TryGetValue("id", out var idValue));
        Assert.Equal(_sampleCreateResponse.Id, idValue);
        Assert.Equal("1.0", createdAtActionResult.RouteValues["version"]);
        var returnedCompany = Assert.IsType<CreateCompanyResponseDto>(createdAtActionResult.Value);
        Assert.Equal(_sampleCreateResponse.Name, returnedCompany.Name);
        _mockCompanyService.Verify(s => s.CreateCompanyAsync(_sampleCreateRequest), Times.Once);
    }

    [Fact]
    public async Task CreateCompany_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.CreateCompany(_sampleCreateRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
        _mockCompanyService.Verify(s => s.CreateCompanyAsync(It.IsAny<CreateCompanyRequestDto>()), Times.Never);
    }

    [Fact]
    public async Task CreateCompany_ReturnsConflict_WhenServiceThrowsInvalidOperationException()
    {
        // Arrange
        var conflictMessage = "Company with this ISIN already exists";
        _mockCompanyService.Setup(s => s.CreateCompanyAsync(_sampleCreateRequest))
            .ThrowsAsync(new InvalidOperationException(conflictMessage));

        // Act
        var result = await _controller.CreateCompany(_sampleCreateRequest);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(conflictMessage, conflictResult.Value);
    }

    [Fact]
    public async Task CreateCompany_ReturnsInternalServerError_WhenServiceThrowsGenericException()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.CreateCompanyAsync(_sampleCreateRequest))
            .ThrowsAsync(new ConnectionAbortedException("Database error"));

        // Act
        var result = await _controller.CreateCompany(_sampleCreateRequest);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while creating the company", statusCodeResult.Value);
        VerifyLoggerWasCalled(LogLevel.Error, "Error creating company");
    }

    #endregion

    #region UpdateCompany Tests

    [Fact]
    public async Task UpdateCompany_ReturnsOkWithUpdatedCompany_WhenUpdateIsSuccessful()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.UpdateCompanyAsync(_testCompanyId, _sampleUpdateRequest))
            .ReturnsAsync(_sampleUpdateResponse);

        // Act
        var result = await _controller.UpdateCompany(_testCompanyId, _sampleUpdateRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCompany = Assert.IsType<UpdateCompanyResponseDto>(okResult.Value);
        Assert.Equal(_sampleUpdateResponse.Name, returnedCompany.Name);
        _mockCompanyService.Verify(s => s.UpdateCompanyAsync(_testCompanyId, _sampleUpdateRequest), Times.Once);
    }

    [Fact]
    public async Task UpdateCompany_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.UpdateCompany(_testCompanyId, _sampleUpdateRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
        _mockCompanyService.Verify(s => s.UpdateCompanyAsync(It.IsAny<Guid>(), It.IsAny<UpdateCompanyRequestDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCompany_ReturnsNotFound_WhenCompanyDoesNotExist()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.UpdateCompanyAsync(_testCompanyId, _sampleUpdateRequest))
            .ReturnsAsync((UpdateCompanyResponseDto?)null);

        // Act
        var result = await _controller.UpdateCompany(_testCompanyId, _sampleUpdateRequest);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal($"Company with ID {_testCompanyId} not found", notFoundResult.Value);
    }

    [Fact]
    public async Task UpdateCompany_ReturnsConflict_WhenServiceThrowsInvalidOperationException()
    {
        // Arrange
        var conflictMessage = "Another company with this ISIN already exists";
        _mockCompanyService.Setup(s => s.UpdateCompanyAsync(_testCompanyId, _sampleUpdateRequest))
            .ThrowsAsync(new InvalidOperationException(conflictMessage));

        // Act
        var result = await _controller.UpdateCompany(_testCompanyId, _sampleUpdateRequest);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(conflictMessage, conflictResult.Value);
    }

    [Fact]
    public async Task UpdateCompany_ReturnsInternalServerError_WhenServiceThrowsGenericException()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.UpdateCompanyAsync(_testCompanyId, _sampleUpdateRequest))
            .ThrowsAsync(new ConnectionAbortedException("Database error"));

        // Act
        var result = await _controller.UpdateCompany(_testCompanyId, _sampleUpdateRequest);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while updating the company", statusCodeResult.Value);
        VerifyLoggerWasCalled(LogLevel.Error, "Error updating company");
    }

    #endregion

    #region PatchCompany Tests

    [Fact]
    public async Task PatchCompany_ReturnsOkWithPatchedCompany_WhenPatchIsSuccessful()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.PatchCompanyAsync(_testCompanyId, _samplePatchRequest))
            .ReturnsAsync(_samplePatchResponse);

        // Act
        var result = await _controller.PatchCompany(_testCompanyId, _samplePatchRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCompany = Assert.IsType<PatchCompanyResponseDto>(okResult.Value);
        Assert.Equal(_samplePatchResponse.Name, returnedCompany.Name);
        _mockCompanyService.Verify(s => s.PatchCompanyAsync(_testCompanyId, _samplePatchRequest), Times.Once);
    }

    [Fact]
    public async Task PatchCompany_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Name cannot be empty");

        // Act
        var result = await _controller.PatchCompany(_testCompanyId, _samplePatchRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
        _mockCompanyService.Verify(s => s.PatchCompanyAsync(It.IsAny<Guid>(), It.IsAny<PatchCompanyRequestDto>()), Times.Never);
    }

    [Fact]
    public async Task PatchCompany_ReturnsNotFound_WhenCompanyDoesNotExist()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.PatchCompanyAsync(_testCompanyId, _samplePatchRequest))
            .ReturnsAsync((PatchCompanyResponseDto?)null);

        // Act
        var result = await _controller.PatchCompany(_testCompanyId, _samplePatchRequest);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal($"Company with ID {_testCompanyId} not found", notFoundResult.Value);
    }

    [Fact]
    public async Task PatchCompany_ReturnsConflict_WhenServiceThrowsInvalidOperationException()
    {
        // Arrange
        var conflictMessage = "Cannot patch company due to business rule violation";
        _mockCompanyService.Setup(s => s.PatchCompanyAsync(_testCompanyId, _samplePatchRequest))
            .ThrowsAsync(new InvalidOperationException(conflictMessage));

        // Act
        var result = await _controller.PatchCompany(_testCompanyId, _samplePatchRequest);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(conflictMessage, conflictResult.Value);
    }

    [Fact]
    public async Task PatchCompany_ReturnsInternalServerError_WhenServiceThrowsGenericException()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.PatchCompanyAsync(_testCompanyId, _samplePatchRequest))
            .ThrowsAsync(new ConnectionAbortedException("Database error"));

        // Act
        var result = await _controller.PatchCompany(_testCompanyId, _samplePatchRequest);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while patching the company", statusCodeResult.Value);
        VerifyLoggerWasCalled(LogLevel.Error, "Error patching company");
    }

    #endregion

    #region DeleteCompany Tests

    [Fact]
    public async Task DeleteCompany_ReturnsNoContent_WhenCompanyIsDeletedSuccessfully()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.DeleteCompanyAsync(_testCompanyId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteCompany(_testCompanyId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockCompanyService.Verify(s => s.DeleteCompanyAsync(_testCompanyId), Times.Once);
    }

    [Fact]
    public async Task DeleteCompany_ReturnsNotFound_WhenCompanyDoesNotExist()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.DeleteCompanyAsync(_testCompanyId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteCompany(_testCompanyId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"Company with ID {_testCompanyId} not found", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteCompany_ReturnsInternalServerError_WhenServiceThrowsException()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.DeleteCompanyAsync(_testCompanyId))
            .ThrowsAsync(new ConnectionAbortedException("Database error"));

        // Act
        var result = await _controller.DeleteCompany(_testCompanyId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while deleting the company", statusCodeResult.Value);
        VerifyLoggerWasCalled(LogLevel.Error, "Error deleting company");
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_SetsProperties_WhenCalledWithValidArguments()
    {
        // Arrange & Act
        var controller = new CompanyController(_mockCompanyService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(controller);
    }

    #endregion

    #region API Versioning Tests

    [Fact]
    public async Task CreateCompany_UsesCorrectApiVersion_WhenVersionIsNotSet()
    {
        // Arrange
        _controller.HttpContext.Features.Set<IApiVersioningFeature>(new ApiVersioningFeature()
        {
            RequestedApiVersion = null!
        });

        _mockCompanyService.Setup(s => s.CreateCompanyAsync(_sampleCreateRequest))
            .ReturnsAsync(_sampleCreateResponse);

        // Act
        var result = await _controller.CreateCompany(_sampleCreateRequest);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.NotNull(createdAtActionResult.RouteValues);
        Assert.True(createdAtActionResult.RouteValues.TryGetValue("version", out var versionValue));
        Assert.Equal("1.0", versionValue);
    }

    #endregion

    #region Helper Methods

    private void VerifyLoggerWasCalled(LogLevel logLevel, string message)
    {
        _mockLogger.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    #endregion
}

public class ApiVersioningFeature : IApiVersioningFeature
{
    public ApiVersion? RequestedApiVersion { get; set; }
    public string? RawRequestedApiVersion { get; set; }
    public string? RouteParameter { get; set; }
    public IReadOnlyList<string> RawRequestedApiVersions { get; set; } = [];
}
