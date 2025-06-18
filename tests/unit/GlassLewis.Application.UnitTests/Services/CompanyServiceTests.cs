using AutoMapper;
using Moq;
using GlassLewis.Application.Services;
using GlassLewis.Application.Dtos.Requests.Company;
using GlassLewis.Application.Dtos.Responses.Company;
using GlassLewis.Domain.Entities;
using GlassLewis.Domain.Interfaces;

namespace GlassLewis.Application.UnitTests.Services;

/// <summary>
/// Provides unit tests for the <see cref="CompanyService"/> class, ensuring its methods behave as expected.
/// </summary>
/// <remarks>This test class uses the <see cref="Mock{T}"/> framework to mock dependencies such as <see
/// cref="ICompanyRepository"/> and <see cref="IMapper"/>. It verifies the correctness of <see cref="CompanyService"/>
/// methods under various scenarios, including valid inputs, edge cases, and error conditions.</remarks>
[Trait("Category", "UnitTests")]
public class CompanyServiceTests
{
    private readonly Mock<ICompanyRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CompanyService _service;

    // Test data
    private readonly Guid _testCompanyId = Guid.NewGuid();
    private readonly string _testIsin = "US1234567890";

    private readonly Company _sampleCompany = new Company
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

    private readonly GetCompanyResponseDto _sampleGetResponse = new GetCompanyResponseDto
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

    private readonly CreateCompanyRequestDto _sampleCreateRequest = new CreateCompanyRequestDto
    {
        Name = "New Company",
        StockTicker = "NEW",
        Exchange = "NASDAQ",
        Isin = "US0987654321",
        Website = "https://newcompany.com"
    };

    private readonly CreateCompanyResponseDto _sampleCreateResponse = new CreateCompanyResponseDto
    {
        Id = Guid.NewGuid(),
        Name = "New Company",
        StockTicker = "NEW",
        Exchange = "NASDAQ",
        Isin = "US0987654321",
        Website = "https://newcompany.com"
    };

    private readonly UpdateCompanyRequestDto _sampleUpdateRequest = new UpdateCompanyRequestDto
    {
        Name = "Updated Company",
        StockTicker = "UPD",
        Exchange = "NYSE",
        Isin = "US1111111111",
        Website = "https://updated.com"
    };

    private readonly UpdateCompanyResponseDto _sampleUpdateResponse = new UpdateCompanyResponseDto
    {
        Name = "Updated Company",
        StockTicker = "UPD",
        Exchange = "NYSE",
        Isin = "US1111111111",
        Website = "https://updated.com"
    };

    private readonly PatchCompanyResponseDto _samplePatchResponse = new PatchCompanyResponseDto
    {
        Name = "Patched Company",
        StockTicker = "TEST",
        Exchange = "NYSE",
        Isin = "US1234567890",
        Website = "https://test.com"
    };

    public CompanyServiceTests()
    {
        _mockRepository = new Mock<ICompanyRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new CompanyService(_mockRepository.Object, _mockMapper.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_SetsProperties_WhenCalledWithValidArguments()
    {
        // Arrange & Act
        var service = new CompanyService(_mockRepository.Object, _mockMapper.Object);

        // Assert
        Assert.NotNull(service);
    }

    #endregion

    #region GetAllCompaniesAsync Tests

    [Fact]
    public async Task GetAllCompaniesAsync_ReturnsAllCompanies_WhenCompaniesExist()
    {
        // Arrange
        var companies = new List<Company> { _sampleCompany };
        var expectedResponses = new List<GetCompanyResponseDto> { _sampleGetResponse };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(companies);
        _mockMapper.Setup(m => m.Map<GetCompanyResponseDto>(_sampleCompany))
            .Returns(_sampleGetResponse);

        // Act
        var result = await _service.GetAllCompaniesAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(_sampleGetResponse.Name, result.First().Name);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<GetCompanyResponseDto>(_sampleCompany), Times.AtLeastOnce);
    }

    [Fact]
    public async Task GetAllCompaniesAsync_ReturnsEmptyCollection_WhenNoCompaniesExist()
    {
        // Arrange
        var companies = new List<Company>();
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(companies);

        // Act
        var result = await _service.GetAllCompaniesAsync();

        // Assert
        Assert.Empty(result);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<GetCompanyResponseDto>(It.IsAny<Company>()), Times.Never);
    }

    [Fact]
    public async Task GetAllCompaniesAsync_ReturnsMultipleCompanies_WhenMultipleCompaniesExist()
    {
        // Arrange
        var company1 = new Company { Id = Guid.NewGuid(), Name = "Company 1" };
        var company2 = new Company { Id = Guid.NewGuid(), Name = "Company 2" };
        var companies = new List<Company> { company1, company2 };

        var response1 = new GetCompanyResponseDto { Id = company1.Id, Name = "Company 1" };
        var response2 = new GetCompanyResponseDto { Id = company2.Id, Name = "Company 2" };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(companies);
        _mockMapper.Setup(m => m.Map<GetCompanyResponseDto>(company1))
            .Returns(response1);
        _mockMapper.Setup(m => m.Map<GetCompanyResponseDto>(company2))
            .Returns(response2);

        // Act
        var result = await _service.GetAllCompaniesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        _mockMapper.Verify(m => m.Map<GetCompanyResponseDto>(It.IsAny<Company>()), Times.Exactly(2));
    }

    #endregion

    #region GetCompanyByIdAsync Tests

    [Fact]
    public async Task GetCompanyByIdAsync_ReturnsCompany_WhenCompanyExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(_testCompanyId))
            .ReturnsAsync(_sampleCompany);
        _mockMapper.Setup(m => m.Map<GetCompanyResponseDto>(_sampleCompany))
            .Returns(_sampleGetResponse);

        // Act
        var result = await _service.GetCompanyByIdAsync(_testCompanyId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_sampleGetResponse.Name, result.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(_testCompanyId), Times.Once);
        _mockMapper.Verify(m => m.Map<GetCompanyResponseDto>(_sampleCompany), Times.Once);
    }

    [Fact]
    public async Task GetCompanyByIdAsync_ReturnsNull_WhenCompanyDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(_testCompanyId))
            .ReturnsAsync((Company?)null);

        // Act
        var result = await _service.GetCompanyByIdAsync(_testCompanyId);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIdAsync(_testCompanyId), Times.Once);
        _mockMapper.Verify(m => m.Map<GetCompanyResponseDto>(It.IsAny<Company>()), Times.Never);
    }

    #endregion

    #region GetCompanyByIsinAsync Tests

    [Fact]
    public async Task GetCompanyByIsinAsync_ReturnsCompany_WhenCompanyExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIsinAsync(_testIsin))
            .ReturnsAsync(_sampleCompany);
        _mockMapper.Setup(m => m.Map<GetCompanyResponseDto>(_sampleCompany))
            .Returns(_sampleGetResponse);

        // Act
        var result = await _service.GetCompanyByIsinAsync(_testIsin);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_sampleGetResponse.Name, result.Name);
        _mockRepository.Verify(r => r.GetByIsinAsync(_testIsin), Times.Once);
        _mockMapper.Verify(m => m.Map<GetCompanyResponseDto>(_sampleCompany), Times.Once);
    }

    [Fact]
    public async Task GetCompanyByIsinAsync_ReturnsNull_WhenCompanyDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIsinAsync(_testIsin))
            .ReturnsAsync((Company?)null);

        // Act
        var result = await _service.GetCompanyByIsinAsync(_testIsin);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIsinAsync(_testIsin), Times.Once);
        _mockMapper.Verify(m => m.Map<GetCompanyResponseDto>(It.IsAny<Company>()), Times.Never);
    }

    #endregion

    #region CreateCompanyAsync Tests

    [Fact]
    public async Task CreateCompanyAsync_ReturnsCreatedCompany_WhenCompanyDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsByIsinAsync(_sampleCreateRequest.Isin))
            .ReturnsAsync(false);
        _mockMapper.Setup(m => m.Map<Company>(_sampleCreateRequest))
            .Returns(_sampleCompany);
        _mockRepository.Setup(r => r.CreateAsync(_sampleCompany))
            .ReturnsAsync(_sampleCompany);
        _mockMapper.Setup(m => m.Map<CreateCompanyResponseDto>(_sampleCompany))
            .Returns(_sampleCreateResponse);

        // Act
        var result = await _service.CreateCompanyAsync(_sampleCreateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_sampleCreateResponse.Name, result.Name);
        _mockRepository.Verify(r => r.ExistsByIsinAsync(_sampleCreateRequest.Isin), Times.Once);
        _mockRepository.Verify(r => r.CreateAsync(_sampleCompany), Times.Once);
        _mockMapper.Verify(m => m.Map<Company>(_sampleCreateRequest), Times.Once);
        _mockMapper.Verify(m => m.Map<CreateCompanyResponseDto>(_sampleCompany), Times.Once);
    }

    [Fact]
    public async Task CreateCompanyAsync_ThrowsInvalidOperationException_WhenCompanyWithIsinAlreadyExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsByIsinAsync(_sampleCreateRequest.Isin))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateCompanyAsync(_sampleCreateRequest));

        Assert.Equal($"A company with ISIN {_sampleCreateRequest.Isin} already exists.", exception.Message);
        _mockRepository.Verify(r => r.ExistsByIsinAsync(_sampleCreateRequest.Isin), Times.Once);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Company>()), Times.Never);
    }

    #endregion

    #region UpdateCompanyAsync Tests

    [Fact]
    public async Task UpdateCompanyAsync_ReturnsUpdatedCompany_WhenCompanyExistsAndIsinIsUnique()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(_testCompanyId))
            .ReturnsAsync(_sampleCompany);
        _mockRepository.Setup(r => r.ExistsByIsinAsync(_sampleUpdateRequest.Isin, _testCompanyId))
            .ReturnsAsync(false);
        _mockMapper.Setup(m => m.Map(_sampleUpdateRequest, _sampleCompany))
            .Returns(_sampleCompany);
        _mockRepository.Setup(r => r.UpdateAsync(_testCompanyId, _sampleCompany))
            .ReturnsAsync(_sampleCompany);
        _mockMapper.Setup(m => m.Map<UpdateCompanyResponseDto>(_sampleCompany))
            .Returns(_sampleUpdateResponse);

        // Act
        var result = await _service.UpdateCompanyAsync(_testCompanyId, _sampleUpdateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_sampleUpdateResponse.Name, result.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(_testCompanyId), Times.Once);
        _mockRepository.Verify(r => r.ExistsByIsinAsync(_sampleUpdateRequest.Isin, _testCompanyId), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(_testCompanyId, _sampleCompany), Times.Once);
    }

    [Fact]
    public async Task UpdateCompanyAsync_ReturnsNull_WhenCompanyDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(_testCompanyId))
            .ReturnsAsync((Company?)null);

        // Act
        var result = await _service.UpdateCompanyAsync(_testCompanyId, _sampleUpdateRequest);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIdAsync(_testCompanyId), Times.Once);
        _mockRepository.Verify(r => r.ExistsByIsinAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Company>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCompanyAsync_ThrowsInvalidOperationException_WhenIsinAlreadyExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(_testCompanyId))
            .ReturnsAsync(_sampleCompany);
        _mockRepository.Setup(r => r.ExistsByIsinAsync(_sampleUpdateRequest.Isin, _testCompanyId))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UpdateCompanyAsync(_testCompanyId, _sampleUpdateRequest));

        Assert.Equal($"A company with ISIN {_sampleUpdateRequest.Isin} already exists.", exception.Message);
        _mockRepository.Verify(r => r.GetByIdAsync(_testCompanyId), Times.Once);
        _mockRepository.Verify(r => r.ExistsByIsinAsync(_sampleUpdateRequest.Isin, _testCompanyId), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Company>()), Times.Never);
    }

    #endregion

    #region PatchCompanyAsync Tests

    [Fact]
    public async Task PatchCompanyAsync_ReturnsPatched_WhenValidationPassesAndCompanyExists()
    {
        // Arrange
        var validPatchRequest = new PatchCompanyRequestDto
        {
            Name = "Patched Company"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(_testCompanyId))
            .ReturnsAsync(_sampleCompany);
        _mockRepository.Setup(r => r.PatchAsync(_testCompanyId, It.IsAny<Dictionary<string, object?>>()))
            .ReturnsAsync(_sampleCompany);
        _mockMapper.Setup(m => m.Map<PatchCompanyResponseDto>(_sampleCompany))
            .Returns(_samplePatchResponse);

        // Act
        var result = await _service.PatchCompanyAsync(_testCompanyId, validPatchRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_samplePatchResponse.Name, result.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(_testCompanyId), Times.Once);
        _mockRepository.Verify(r => r.PatchAsync(_testCompanyId, It.IsAny<Dictionary<string, object?>>()), Times.Once);
    }

    [Fact]
    public async Task PatchCompanyAsync_ThrowsArgumentException_WhenValidationFails()
    {
        // Arrange
        var invalidPatchRequest = new PatchCompanyRequestDto
        {
            Name = "InvalidNameWithMoreThan200Characters" +
                   "InvalidNameWithMoreThan200Characters" +
                   "InvalidNameWithMoreThan200Characters" +
                   "InvalidNameWithMoreThan200Characters" +
                   "InvalidNameWithMoreThan200Characters" +
                   "InvalidNameWithMoreThan200Characters" +
                   "InvalidNameWithMoreThan200Characters" +
                   "InvalidNameWithMoreThan200Characters"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.PatchCompanyAsync(_testCompanyId, invalidPatchRequest));

        Assert.StartsWith("Validation failed:", exception.Message);
        _mockRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task PatchCompanyAsync_ReturnsNull_WhenCompanyDoesNotExist()
    {
        // Arrange
        var validPatchRequest = new PatchCompanyRequestDto
        {
            Name = "Patched Company"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(_testCompanyId))
            .ReturnsAsync((Company?)null);

        // Act
        var result = await _service.PatchCompanyAsync(_testCompanyId, validPatchRequest);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIdAsync(_testCompanyId), Times.Once);
        _mockRepository.Verify(r => r.PatchAsync(It.IsAny<Guid>(), It.IsAny<Dictionary<string, object?>>()), Times.Never);
    }

    [Fact]
    public async Task PatchCompanyAsync_ThrowsInvalidOperationException_WhenIsinAlreadyExists()
    {
        // Arrange
        var patchRequestWithIsin = new PatchCompanyRequestDto
        {
            Isin = "US9999999999"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(_testCompanyId))
            .ReturnsAsync(_sampleCompany);
        _mockRepository.Setup(r => r.ExistsByIsinAsync(patchRequestWithIsin.Isin, _testCompanyId))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.PatchCompanyAsync(_testCompanyId, patchRequestWithIsin));

        Assert.Equal($"A company with ISIN {patchRequestWithIsin.Isin} already exists.", exception.Message);
        _mockRepository.Verify(r => r.ExistsByIsinAsync(patchRequestWithIsin.Isin, _testCompanyId), Times.Once);
        _mockRepository.Verify(r => r.PatchAsync(It.IsAny<Guid>(), It.IsAny<Dictionary<string, object?>>()), Times.Never);
    }

    [Fact]
    public async Task PatchCompanyAsync_DoesNotCheckIsin_WhenIsinIsNotProvided()
    {
        // Arrange
        var patchRequestWithoutIsin = new PatchCompanyRequestDto
        {
            Name = "Patched Company"
            // No ISIN property set
        };

        _mockRepository.Setup(r => r.GetByIdAsync(_testCompanyId))
            .ReturnsAsync(_sampleCompany);
        _mockRepository.Setup(r => r.PatchAsync(_testCompanyId, It.IsAny<Dictionary<string, object?>>()))
            .ReturnsAsync(_sampleCompany);
        _mockMapper.Setup(m => m.Map<PatchCompanyResponseDto>(_sampleCompany))
            .Returns(_samplePatchResponse);

        // Act
        var result = await _service.PatchCompanyAsync(_testCompanyId, patchRequestWithoutIsin);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.ExistsByIsinAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task PatchCompanyAsync_DoesNotCheckIsin_WhenIsinIsEmpty()
    {
        // Arrange
        var patchRequestWithEmptyIsin = new PatchCompanyRequestDto
        {
            Isin = string.Empty
        };

        _mockRepository.Setup(r => r.GetByIdAsync(_testCompanyId))
            .ReturnsAsync(_sampleCompany);
        _mockRepository.Setup(r => r.PatchAsync(_testCompanyId, It.IsAny<Dictionary<string, object?>>()))
            .ReturnsAsync(_sampleCompany);
        _mockMapper.Setup(m => m.Map<PatchCompanyResponseDto>(_sampleCompany))
            .Returns(_samplePatchResponse);

        // Act
        var result = await _service.PatchCompanyAsync(_testCompanyId, patchRequestWithEmptyIsin);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.ExistsByIsinAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task PatchCompanyAsync_DoesNotCheckIsin_WhenIsinIsSameAsExisting()
    {
        // Arrange
        var patchRequestWithSameIsin = new PatchCompanyRequestDto
        {
            Isin = _sampleCompany.Isin // Same as existing company
        };

        _mockRepository.Setup(r => r.GetByIdAsync(_testCompanyId))
            .ReturnsAsync(_sampleCompany);
        _mockRepository.Setup(r => r.PatchAsync(_testCompanyId, It.IsAny<Dictionary<string, object?>>()))
            .ReturnsAsync(_sampleCompany);
        _mockMapper.Setup(m => m.Map<PatchCompanyResponseDto>(_sampleCompany))
            .Returns(_samplePatchResponse);

        // Act
        var result = await _service.PatchCompanyAsync(_testCompanyId, patchRequestWithSameIsin);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.ExistsByIsinAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
    }

    #endregion

    #region DeleteCompanyAsync Tests

    [Fact]
    public async Task DeleteCompanyAsync_ReturnsTrue_WhenCompanyIsDeleted()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(_testCompanyId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteCompanyAsync(_testCompanyId);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(_testCompanyId), Times.Once);
    }

    [Fact]
    public async Task DeleteCompanyAsync_ReturnsFalse_WhenCompanyIsNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(_testCompanyId))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteCompanyAsync(_testCompanyId);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.DeleteAsync(_testCompanyId), Times.Once);
    }

    #endregion
}
