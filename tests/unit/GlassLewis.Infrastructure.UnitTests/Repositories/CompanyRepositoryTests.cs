using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using GlassLewis.Domain.Entities;
using GlassLewis.Infrastructure.Data.Contexts;
using GlassLewis.Infrastructure.Repositories;

namespace GlassLewis.Infrastructure.UnitTests.Repositories;

/// <summary>
/// Provides unit tests for the <see cref="CompanyRepository"/> class, ensuring its methods behave as expected.
/// </summary>
/// <remarks>This test class uses an in-memory database to isolate tests and avoid external dependencies. It
/// verifies the functionality of <see cref="CompanyRepository"/> methods, including CRUD operations, data retrieval,
/// and error handling.</remarks>
[Trait("Category", "UnitTests")]
public class CompanyRepositoryTests : IDisposable
{
    private readonly CompanyDbContext _context;
    private readonly Mock<ILogger<CompanyRepository>> _mockLogger;
    private readonly CompanyRepository _repository;
    private readonly DbContextOptions<CompanyDbContext> _options;

    public CompanyRepositoryTests()
    {
        // Create in-memory database with unique name per test
        _options = new DbContextOptionsBuilder<CompanyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CompanyDbContext(_options);
        _mockLogger = new Mock<ILogger<CompanyRepository>>();
        _repository = new CompanyRepository(_context, _mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidParameters_CreatesRepository()
    {
        // Arrange, Act & Assert
        var repository = new CompanyRepository(_context, _mockLogger.Object);
        Assert.NotNull(repository);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsAllCompaniesOrderedByName()
    {
        // Arrange
        var companies = new[]
        {
            CreateTestCompany("Zebra Company", "ZEB"),
            CreateTestCompany("Alpha Company", "ALP"),
            CreateTestCompany("Beta Company", "BET")
        };

        _context.Companies.AddRange(companies);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();
        var resultList = result.ToList();

        // Assert
        Assert.Equal(3, resultList.Count);
        Assert.Equal("Alpha Company", resultList[0].Name);
        Assert.Equal("Beta Company", resultList[1].Name);
        Assert.Equal("Zebra Company", resultList[2].Name);
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyDatabase_ReturnsEmptyCollection()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenDatabaseThrowsException_LogsErrorAndRethrows()
    {
        // Arrange
        var mockContext = new Mock<CompanyDbContext>(_options);
        mockContext.Setup(c => c.Companies).Throws(new InvalidOperationException("Database error"));
        var repository = new CompanyRepository(mockContext.Object, _mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => repository.GetAllAsync());
        Assert.Equal("Database error", exception.Message);
        VerifyLoggerWasCalled(LogLevel.Error, "Error retrieving all companies");
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCompany()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(company.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(company.Id, result.Id);
        Assert.Equal("Test Company", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenDatabaseThrowsException_LogsErrorAndRethrows()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var mockContext = new Mock<CompanyDbContext>(_options);
        mockContext.Setup(c => c.Companies).Throws(new InvalidOperationException("Database error"));
        var repository = new CompanyRepository(mockContext.Object, _mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => repository.GetByIdAsync(testId));
        Assert.Equal("Database error", exception.Message);
        VerifyLoggerWasCalled(LogLevel.Error, $"Error retrieving company with ID {testId}");
    }

    #endregion

    #region GetByIsinAsync Tests

    [Fact]
    public async Task GetByIsinAsync_WithValidIsin_ReturnsCompany()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIsinAsync("US1234567890");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("US1234567890", result.Isin);
        Assert.Equal("Test Company", result.Name);
    }

    [Fact]
    public async Task GetByIsinAsync_WithNonExistentIsin_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIsinAsync("US9999999999");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIsinAsync_WhenDatabaseThrowsException_LogsErrorAndRethrows()
    {
        // Arrange
        var testIsin = "US1234567890";
        var mockContext = new Mock<CompanyDbContext>(_options);
        mockContext.Setup(c => c.Companies).Throws(new InvalidOperationException("Database error"));
        var repository = new CompanyRepository(mockContext.Object, _mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => repository.GetByIsinAsync(testIsin));
        Assert.Equal("Database error", exception.Message);
        VerifyLoggerWasCalled(LogLevel.Error, $"Error retrieving company with ISIN {testIsin}");
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidCompany_CreatesAndReturnsCompany()
    {
        // Arrange
        var company = CreateTestCompany("New Company", "NEW");
        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = await _repository.CreateAsync(company);
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(company.Id, result.Id);
        Assert.Equal("New Company", result.Name);
        Assert.True(result.CreatedAt >= beforeCreation && result.CreatedAt <= afterCreation);
        Assert.True(result.UpdatedAt >= beforeCreation && result.UpdatedAt <= afterCreation);

        // Verify it was saved to database
        var savedCompany = await _context.Companies.FindAsync(company.Id);
        Assert.NotNull(savedCompany);
        VerifyLoggerWasCalled(LogLevel.Information, $"Created company with ID {company.Id}");
    }

    [Fact]
    public async Task CreateAsync_WhenDatabaseThrowsException_LogsErrorAndRethrows()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        var mockContext = new Mock<CompanyDbContext>(_options);
        mockContext.Setup(c => c.Companies).Throws(new InvalidOperationException("Database error"));
        var repository = new CompanyRepository(mockContext.Object, _mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => repository.CreateAsync(company));
        Assert.Equal("Database error", exception.Message);
        VerifyLoggerWasCalled(LogLevel.Error, $"Error creating company with ISIN {company.Isin}");
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidIdAndCompany_UpdatesAndReturnsCompany()
    {
        // Arrange
        var originalCompany = CreateTestCompany("Original Company", "ORIG", "US1111111111");
        _context.Companies.Add(originalCompany);
        await _context.SaveChangesAsync();

        var updatedCompany = CreateTestCompany("Updated Company", "UPD", "US2222222222", "https://updated.com");
        var beforeUpdate = DateTime.UtcNow;

        // Act
        var result = await _repository.UpdateAsync(originalCompany.Id, updatedCompany);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(originalCompany.Id, result.Id); // ID should remain the same
        Assert.Equal("Updated Company", result.Name);
        Assert.Equal("UPD", result.StockTicker);
        Assert.Equal("US2222222222", result.Isin);
        Assert.Equal("https://updated.com", result.Website);
        Assert.True(result.UpdatedAt >= beforeUpdate && result.UpdatedAt <= afterUpdate);

        VerifyLoggerWasCalled(LogLevel.Information, $"Updated company with ID {originalCompany.Id}");
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updatedCompany = CreateTestCompany("Updated Company", "UPD");

        // Act
        var result = await _repository.UpdateAsync(nonExistentId, updatedCompany);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenDatabaseThrowsException_LogsErrorAndRethrows()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var company = CreateTestCompany("Test Company", "TEST");
        var mockContext = new Mock<CompanyDbContext>(_options);
        mockContext.Setup(c => c.Companies).Throws(new InvalidOperationException("Database error"));
        var repository = new CompanyRepository(mockContext.Object, _mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => repository.UpdateAsync(testId, company));
        Assert.Equal("Database error", exception.Message);
        VerifyLoggerWasCalled(LogLevel.Error, $"Error updating company with ID {testId}");
    }

    #endregion

    #region PatchAsync Tests

    [Fact]
    public async Task PatchAsync_WithValidFieldsToUpdate_UpdatesAndReturnsCompany()
    {
        // Arrange
        var company = CreateTestCompany("Original Company", "ORIG");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var fieldsToUpdate = new Dictionary<string, object?>
        {
            { nameof(Company.Name), "Patched Company" },
            { nameof(Company.StockTicker), "PATCH" }
        };

        var beforeUpdate = DateTime.UtcNow;

        // Act
        var result = await _repository.PatchAsync(company.Id, fieldsToUpdate);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Patched Company", result.Name);
        Assert.Equal("PATCH", result.StockTicker);
        Assert.True(result.UpdatedAt >= beforeUpdate && result.UpdatedAt <= afterUpdate);

        VerifyLoggerWasCalled(LogLevel.Information, $"Patched company with ID {company.Id}. Updated fields: Name, StockTicker");
    }

    [Fact]
    public async Task PatchAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var fieldsToUpdate = new Dictionary<string, object?> { { nameof(Company.Name), "Test" } };

        // Act
        var result = await _repository.PatchAsync(nonExistentId, fieldsToUpdate);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task PatchAsync_WithNullFieldsToUpdate_ReturnsUnchangedCompany()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.PatchAsync(company.Id, null!);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Company", result.Name);
        VerifyLoggerWasCalled(LogLevel.Warning, $"No fields provided for patching company with ID {company.Id}");
    }

    [Fact]
    public async Task PatchAsync_WithEmptyFieldsToUpdate_ReturnsUnchangedCompany()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var emptyFields = new Dictionary<string, object?>();

        // Act
        var result = await _repository.PatchAsync(company.Id, emptyFields);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Company", result.Name);
        VerifyLoggerWasCalled(LogLevel.Warning, $"No fields provided for patching company with ID {company.Id}");
    }

    [Fact]
    public async Task PatchAsync_WithEmptyFieldName_LogsWarningAndContinues()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var fieldsToUpdate = new Dictionary<string, object?>
        {
            { "", "Invalid Field" },
            { nameof(Company.Name), "Valid Update" }
        };

        // Act
        var result = await _repository.PatchAsync(company.Id, fieldsToUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Valid Update", result.Name);
        VerifyLoggerWasCalled(LogLevel.Warning, $"Empty field name provided for patching company with ID {company.Id}");
    }

    [Fact]
    public async Task PatchAsync_WithDisallowedField_LogsWarningAndContinues()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var fieldsToUpdate = new Dictionary<string, object?>
        {
            { "Id", Guid.NewGuid() }, // Not allowed
            { nameof(Company.Name), "Valid Update" }
        };

        // Act
        var result = await _repository.PatchAsync(company.Id, fieldsToUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Valid Update", result.Name);
        Assert.NotEqual(fieldsToUpdate["Id"], result.Id); // ID should not be changed
        VerifyLoggerWasCalled(LogLevel.Warning, $"Field 'Id' is not allowed for patching company with ID {company.Id}");
    }

    [Fact]
    public async Task PatchAsync_WithNonExistentProperty_LogsWarningAndContinues()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var fieldsToUpdate = new Dictionary<string, object?>
        {
            { "NonExistentProperty", "Invalid" },
            { nameof(Company.Name), "Valid Update" }
        };

        // Act
        var result = await _repository.PatchAsync(company.Id, fieldsToUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Valid Update", result.Name);
        VerifyLoggerWasCalled(LogLevel.Warning, $"Field 'NonExistentProperty' is not allowed for patching company with ID {company.Id}");
    }

    [Fact]
    public async Task PatchAsync_WithInvalidValueConversion_LogsWarningAndContinues()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var fieldsToUpdate = new Dictionary<string, object?>
        {
            { nameof(Company.Name), "Valid Update" },
            { nameof(Company.Website), "   " } // Will be converted to null
        };

        // Act
        var result = await _repository.PatchAsync(company.Id, fieldsToUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Valid Update", result.Name);
        Assert.Null(result.Website);
    }

    [Fact]
    public async Task PatchAsync_WithNoValidFields_ReturnsUnchangedCompany()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var fieldsToUpdate = new Dictionary<string, object?>
        {
            { "InvalidField", "Invalid" }
        };

        // Act
        var result = await _repository.PatchAsync(company.Id, fieldsToUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Company", result.Name);
        VerifyLoggerWasCalled(LogLevel.Information, $"No valid fields updated for company with ID {company.Id}");
    }

    [Fact]
    public async Task PatchAsync_WhenDatabaseThrowsException_LogsErrorAndRethrows()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var fieldsToUpdate = new Dictionary<string, object?> { { nameof(Company.Name), "Test" } };
        var mockContext = new Mock<CompanyDbContext>(_options);
        mockContext.Setup(c => c.Companies).Throws(new InvalidOperationException("Database error"));
        var repository = new CompanyRepository(mockContext.Object, _mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => repository.PatchAsync(testId, fieldsToUpdate));
        Assert.Equal("Database error", exception.Message);
        VerifyLoggerWasCalled(LogLevel.Error, $"Error patching company with ID {testId}");
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesCompanyAndReturnsTrue()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(company.Id);

        // Assert
        Assert.True(result);
        var deletedCompany = await _context.Companies.FindAsync(company.Id);
        Assert.Null(deletedCompany);
        VerifyLoggerWasCalled(LogLevel.Information, $"Deleted company with ID {company.Id}");
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteAsync(nonExistentId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenDatabaseThrowsException_LogsErrorAndRethrows()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var mockContext = new Mock<CompanyDbContext>(_options);
        mockContext.Setup(c => c.Companies).Throws(new InvalidOperationException("Database error"));
        var repository = new CompanyRepository(mockContext.Object, _mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => repository.DeleteAsync(testId));
        Assert.Equal("Database error", exception.Message);
        VerifyLoggerWasCalled(LogLevel.Error, $"Error deleting company with ID {testId}");
    }

    #endregion

    #region ExistsByIsinAsync Tests

    [Fact]
    public async Task ExistsByIsinAsync_WithExistingIsin_ReturnsTrue()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByIsinAsync("US1234567890");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByIsinAsync_WithNonExistentIsin_ReturnsFalse()
    {
        // Act
        var result = await _repository.ExistsByIsinAsync("US9999999999");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsByIsinAsync_WhenDatabaseThrowsException_LogsErrorAndRethrows()
    {
        // Arrange
        var testIsin = "US1234567890";
        var mockContext = new Mock<CompanyDbContext>(_options);
        mockContext.Setup(c => c.Companies).Throws(new InvalidOperationException("Database error"));
        var repository = new CompanyRepository(mockContext.Object, _mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => repository.ExistsByIsinAsync(testIsin));
        Assert.Equal("Database error", exception.Message);
        VerifyLoggerWasCalled(LogLevel.Error, $"Error checking if company exists with ISIN {testIsin}");
    }

    #endregion

    #region ExistsByIsinAsync with ExcludeId Tests

    [Fact]
    public async Task ExistsByIsinAsync_WithExcludeId_WhenIsinExistsButExcluded_ReturnsFalse()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByIsinAsync("US1234567890", company.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsByIsinAsync_WithExcludeId_WhenIsinExistsAndNotExcluded_ReturnsTrue()
    {
        // Arrange
        var company1 = CreateTestCompany("Company 1", "COM1");
        var company2 = CreateTestCompany("Company 2", "COM2", "US0987654321");
        _context.Companies.AddRange(company1, company2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByIsinAsync("US1234567890", company2.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByIsinAsync_WithExcludeId_WhenIsinDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var excludeId = Guid.NewGuid();

        // Act
        var result = await _repository.ExistsByIsinAsync("US9999999999", excludeId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsByIsinAsync_WithExcludeId_WhenDatabaseThrowsException_LogsErrorAndRethrows()
    {
        // Arrange
        var testIsin = "US1234567890";
        var excludeId = Guid.NewGuid();
        var mockContext = new Mock<CompanyDbContext>(_options);
        mockContext.Setup(c => c.Companies).Throws(new InvalidOperationException("Database error"));
        var repository = new CompanyRepository(mockContext.Object, _mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => repository.ExistsByIsinAsync(testIsin, excludeId));
        Assert.Equal("Database error", exception.Message);
        VerifyLoggerWasCalled(LogLevel.Error, $"Error checking if company exists with ISIN {testIsin} excluding ID {excludeId}");
    }

    #endregion

    #region ConvertValueForProperty Tests (Private Method)

    [Fact]
    public async Task PatchAsync_ConvertValueForProperty_HandlesNullValue()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var fieldsToUpdate = new Dictionary<string, object?>
        {
            { nameof(Company.Website), null }
        };

        // Act
        var result = await _repository.PatchAsync(company.Id, fieldsToUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Website);
    }

    [Fact]
    public async Task PatchAsync_ConvertValueForProperty_HandlesStringConversion()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var fieldsToUpdate = new Dictionary<string, object?>
        {
            { nameof(Company.Name), "  Trimmed Name  " }
        };

        // Act
        var result = await _repository.PatchAsync(company.Id, fieldsToUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Trimmed Name", result.Name);
    }

    [Fact]
    public async Task PatchAsync_ConvertValueForProperty_HandlesEmptyStringAsNull()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST", "US1234567890", "https://test.com");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var fieldsToUpdate = new Dictionary<string, object?>
        {
            { nameof(Company.Website), "   " } // Whitespace only
        };

        // Act
        var result = await _repository.PatchAsync(company.Id, fieldsToUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Website);
    }

    [Fact]
    public async Task PatchAsync_ConvertValueForProperty_HandlesTypeConversion()
    {
        // Arrange
        var company = CreateTestCompany("Test Company", "TEST");
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        // Test with number that should convert to string
        var fieldsToUpdate = new Dictionary<string, object?>
        {
            { nameof(Company.StockTicker), 123 } // Number to string
        };

        // Act
        var result = await _repository.PatchAsync(company.Id, fieldsToUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("123", result.StockTicker);
    }

    #endregion

    #region Helper Methods

    private static Company CreateTestCompany(string name, string stockTicker, string isin = "US1234567890", string? website = null)
    {
        return new Company
        {
            Id = Guid.NewGuid(),
            Name = name,
            StockTicker = stockTicker,
            Exchange = "NYSE",
            Isin = isin,
            Website = website,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private void VerifyLoggerWasCalled(LogLevel logLevel, string message)
    {
        _mockLogger.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce);
    }

    #endregion
}
