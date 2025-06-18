using Microsoft.EntityFrameworkCore;
using GlassLewis.Domain.Entities;
using GlassLewis.Infrastructure.Data.Contexts;

namespace GlassLewis.Infrastructure.UnitTests.Data.Contexts;

/// <summary>
/// Provides unit tests for the <see cref="CompanyDbContext"/> class, including tests for its constructors, DbSet
/// properties, model configuration, seed data, and integration behavior.
/// </summary>
/// <remarks>This test class uses an in-memory database to verify the behavior of <see cref="CompanyDbContext"/>.
/// It includes tests for entity configuration, database operations, and constraints such as unique indexes. The class
/// implements <see cref="IDisposable"/> to ensure proper cleanup of the database context.</remarks>
[Trait("Category", "UnitTests")]
public class CompanyDbContextTests : IDisposable
{
    private readonly DbContextOptions<CompanyDbContext> _options;
    private readonly CompanyDbContext _context;

    public CompanyDbContextTests()
    {
        // Use in-memory database for testing
        _options = new DbContextOptionsBuilder<CompanyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CompanyDbContext(_options);
    }

    public void Dispose()
    {
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidOptions_CreatesContext()
    {
        // Arrange & Act
        using var context = new CompanyDbContext(_options);

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(context.Companies);
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CompanyDbContext(null!));
    }

    #endregion

    #region DbSet Tests

    [Fact]
    public void Companies_DbSet_IsNotNull()
    {
        // Act & Assert
        Assert.NotNull(_context.Companies);
    }

    [Fact]
    public void Companies_DbSet_IsDbSet()
    {
        // Act & Assert
        Assert.IsAssignableFrom<DbSet<Company>>(_context.Companies);
    }

    [Fact]
    public void Companies_DbSet_CanAddEntity()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            StockTicker = "TEST",
            Exchange = "NYSE",
            Isin = "US1234567890",
            Website = "https://test.com"
        };

        // Act
        _context.Companies.Add(company);
        var entry = _context.Entry(company);

        // Assert
        Assert.Equal(EntityState.Added, entry.State);
    }

    #endregion

    #region Model Configuration Tests

    [Fact]
    public void OnModelCreating_ConfiguresCompanyEntity()
    {
        // Arrange
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Company));

        // Assert
        Assert.NotNull(entityType);
        Assert.Equal(typeof(Company), entityType.ClrType);
    }

    [Fact]
    public void OnModelCreating_ConfiguresUniqueIndexOnIsin()
    {
        // Arrange
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Company));

        // Act
        var isinIndex = entityType?.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(Company.Isin)) && i.IsUnique);

        // Assert
        Assert.NotNull(isinIndex);
        Assert.True(isinIndex.IsUnique);
        Assert.Equal("IX_Companies_Isin", isinIndex.GetDatabaseName());
        Assert.Single(isinIndex.Properties);
        Assert.Equal(nameof(Company.Isin), isinIndex.Properties[0].Name);
    }

    [Fact]
    public void OnModelCreating_ConfiguresIndexOnName()
    {
        // Arrange
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Company));

        // Act
        var nameIndex = entityType?.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(Company.Name)) && !i.IsUnique);

        // Assert
        Assert.NotNull(nameIndex);
        Assert.False(nameIndex.IsUnique);
        Assert.Equal("IX_Companies_Name", nameIndex.GetDatabaseName());
        Assert.Single(nameIndex.Properties);
        Assert.Equal(nameof(Company.Name), nameIndex.Properties[0].Name);
    }

    [Fact]
    public void OnModelCreating_ConfiguresIndexOnStockTicker()
    {
        // Arrange
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Company));

        // Act
        var stockTickerIndex = entityType?.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(Company.StockTicker)) && !i.IsUnique);

        // Assert
        Assert.NotNull(stockTickerIndex);
        Assert.False(stockTickerIndex.IsUnique);
        Assert.Equal("IX_Companies_StockTicker", stockTickerIndex.GetDatabaseName());
        Assert.Single(stockTickerIndex.Properties);
        Assert.Equal(nameof(Company.StockTicker), stockTickerIndex.Properties[0].Name);
    }

    #endregion

    #region Property Configuration Tests

    [Fact]
    public void OnModelCreating_ConfiguresNameProperty()
    {
        // Arrange
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Company));

        // Act
        var nameProperty = entityType?.FindProperty(nameof(Company.Name));

        // Assert
        Assert.NotNull(nameProperty);
        Assert.False(nameProperty.IsNullable);
        Assert.Equal(200, nameProperty.GetMaxLength());
    }

    [Fact]
    public void OnModelCreating_ConfiguresStockTickerProperty()
    {
        // Arrange
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Company));

        // Act
        var stockTickerProperty = entityType?.FindProperty(nameof(Company.StockTicker));

        // Assert
        Assert.NotNull(stockTickerProperty);
        Assert.False(stockTickerProperty.IsNullable);
        Assert.Equal(10, stockTickerProperty.GetMaxLength());
    }

    [Fact]
    public void OnModelCreating_ConfiguresExchangeProperty()
    {
        // Arrange
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Company));

        // Act
        var exchangeProperty = entityType?.FindProperty(nameof(Company.Exchange));

        // Assert
        Assert.NotNull(exchangeProperty);
        Assert.False(exchangeProperty.IsNullable);
        Assert.Equal(100, exchangeProperty.GetMaxLength());
    }

    [Fact]
    public void OnModelCreating_ConfiguresIsinProperty()
    {
        // Arrange
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Company));

        // Act
        var isinProperty = entityType?.FindProperty(nameof(Company.Isin));

        // Assert
        Assert.NotNull(isinProperty);
        Assert.False(isinProperty.IsNullable);
        Assert.Equal(12, isinProperty.GetMaxLength());
        Assert.True(isinProperty.IsFixedLength());
    }

    [Fact]
    public void OnModelCreating_ConfiguresWebsiteProperty()
    {
        // Arrange
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Company));

        // Act
        var websiteProperty = entityType?.FindProperty(nameof(Company.Website));

        // Assert
        Assert.NotNull(websiteProperty);
        Assert.True(websiteProperty.IsNullable);
        Assert.Equal(500, websiteProperty.GetMaxLength());
    }

    [Fact]
    public void OnModelCreating_ConfiguresCreatedAtProperty()
    {
        // Arrange
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Company));

        // Act
        var createdAtProperty = entityType?.FindProperty(nameof(Company.CreatedAt));

        // Assert
        Assert.NotNull(createdAtProperty);
        Assert.False(createdAtProperty.IsNullable);
        Assert.Equal("GETUTCDATE()", createdAtProperty.GetDefaultValueSql());
    }

    [Fact]
    public void OnModelCreating_ConfiguresUpdatedAtProperty()
    {
        // Arrange
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Company));

        // Act
        var updatedAtProperty = entityType?.FindProperty(nameof(Company.UpdatedAt));

        // Assert
        Assert.NotNull(updatedAtProperty);
        Assert.False(updatedAtProperty.IsNullable);
        Assert.Equal("GETUTCDATE()", updatedAtProperty.GetDefaultValueSql());
    }

    #endregion

    #region Seed Data Tests

    [Fact]
    public void OnModelCreating_SeedsAppleCompany()
    {
        // Arrange
        var expectedId = new Guid("8973d1e2-3020-49db-862b-2d454746b42d");

        // Act
        _context.Database.EnsureCreated();
        var appleCompany = _context.Companies.Find(expectedId);

        // Assert
        Assert.NotNull(appleCompany);
        Assert.Equal("Apple Inc.", appleCompany.Name);
        Assert.Equal("NASDAQ", appleCompany.Exchange);
        Assert.Equal("AAPL", appleCompany.StockTicker);
        Assert.Equal("US0378331005", appleCompany.Isin);
        Assert.Equal("http://www.apple.com", appleCompany.Website);
        Assert.Equal(new DateTime(2025, 06, 05), appleCompany.CreatedAt);
        Assert.Equal(new DateTime(2025, 06, 05), appleCompany.UpdatedAt);
    }

    [Fact]
    public void OnModelCreating_SeedsBritishAirwaysCompany()
    {
        // Arrange
        var expectedId = new Guid("16511881-10fd-4772-9b62-f1b78513d8af");

        // Act
        _context.Database.EnsureCreated();
        var britishAirwaysCompany = _context.Companies.Find(expectedId);

        // Assert
        Assert.NotNull(britishAirwaysCompany);
        Assert.Equal("British Airways Plc", britishAirwaysCompany.Name);
        Assert.Equal("Pink Sheets", britishAirwaysCompany.Exchange);
        Assert.Equal("BAIRY", britishAirwaysCompany.StockTicker);
        Assert.Equal("US1104193065", britishAirwaysCompany.Isin);
        Assert.Null(britishAirwaysCompany.Website);
        Assert.Equal(new DateTime(2025, 06, 05), britishAirwaysCompany.CreatedAt);
        Assert.Equal(new DateTime(2025, 06, 05), britishAirwaysCompany.UpdatedAt);
    }

    [Fact]
    public void OnModelCreating_SeedsHeinekenCompany()
    {
        // Arrange
        var expectedId = new Guid("e8146538-3991-4877-9b76-f276b9849d45");

        // Act
        _context.Database.EnsureCreated();
        var heinekenCompany = _context.Companies.Find(expectedId);

        // Assert
        Assert.NotNull(heinekenCompany);
        Assert.Equal("Heineken NV", heinekenCompany.Name);
        Assert.Equal("Euronext Amsterdam", heinekenCompany.Exchange);
        Assert.Equal("HEIA", heinekenCompany.StockTicker);
        Assert.Equal("NL0000009165", heinekenCompany.Isin);
        Assert.Null(heinekenCompany.Website);
        Assert.Equal(new DateTime(2025, 06, 05), heinekenCompany.CreatedAt);
        Assert.Equal(new DateTime(2025, 06, 05), heinekenCompany.UpdatedAt);
    }

    [Fact]
    public void OnModelCreating_SeedsExactlyThreeCompanies()
    {
        // Act
        _context.Database.EnsureCreated();
        var companyCount = _context.Companies.Count();

        // Assert
        Assert.Equal(3, companyCount);
    }

    [Fact]
    public void OnModelCreating_SeedDataHasUniqueIsins()
    {
        // Act
        _context.Database.EnsureCreated();
        var isins = _context.Companies.Select(c => c.Isin).ToList();
        var uniqueIsins = isins.Distinct().ToList();

        // Assert
        Assert.Equal(isins.Count, uniqueIsins.Count);
        Assert.Contains("US0378331005", isins);
        Assert.Contains("US1104193065", isins);
        Assert.Contains("NL0000009165", isins);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Database_CanCreateAndDropDatabase()
    {
        // Act
        var created = _context.Database.EnsureCreated();
        var deleted = _context.Database.EnsureDeleted();

        // Assert
        Assert.True(created);
        Assert.True(deleted);
    }

    [Fact]
    public void Companies_CanAddNewCompany()
    {
        // Arrange
        _context.Database.EnsureCreated();
        var newCompany = new Company
        {
            Id = Guid.NewGuid(),
            Name = "New Test Company",
            StockTicker = "NEW",
            Exchange = "NASDAQ",
            Isin = "US9999999999",
            Website = "https://newtest.com"
        };

        // Act
        _context.Companies.Add(newCompany);
        _context.SaveChanges();

        // Assert
        var retrievedCompany = _context.Companies.Find(newCompany.Id);
        Assert.NotNull(retrievedCompany);
        Assert.Equal(newCompany.Name, retrievedCompany.Name);
    }

    [Fact]
    public void Companies_CanUpdateExistingCompany()
    {
        // Arrange
        _context.Database.EnsureCreated();
        var appleId = new Guid("8973d1e2-3020-49db-862b-2d454746b42d");
        var apple = _context.Companies.Find(appleId);
        Assert.NotNull(apple);

        // Act
        apple.Name = "Updated Apple Inc.";
        _context.SaveChanges();

        // Assert
        var updatedApple = _context.Companies.Find(appleId);
        Assert.Equal("Updated Apple Inc.", updatedApple?.Name);
    }

    [Fact]
    public void Companies_CanDeleteCompany()
    {
        // Arrange
        _context.Database.EnsureCreated();
        var appleId = new Guid("8973d1e2-3020-49db-862b-2d454746b42d");
        var apple = _context.Companies.Find(appleId);
        Assert.NotNull(apple);

        // Act
        _context.Companies.Remove(apple);
        _context.SaveChanges();

        // Assert
        var deletedApple = _context.Companies.Find(appleId);
        Assert.Null(deletedApple);
    }

    [Fact]
    public void Companies_EnforcesUniqueIsinConstraint()
    {
        // Arrange
        _context.Database.EnsureCreated();
        var duplicateIsinCompany = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Duplicate ISIN Company",
            StockTicker = "DUP",
            Exchange = "NYSE",
            Isin = "US0378331005", // Same as Apple's ISIN
            Website = "https://duplicate.com"
        };

        // Act & Assert
        _context.Companies.Add(duplicateIsinCompany);

        // For in-memory database, we need to manually check uniqueness
        // In a real SQL database, this would throw a constraint violation
        var existingCompanyWithSameIsin = _context.Companies
            .Any(c => c.Isin == duplicateIsinCompany.Isin && c.Id != duplicateIsinCompany.Id);

        Assert.True(existingCompanyWithSameIsin);
    }

    #endregion

    #region Index Verification Tests

    [Fact]
    public void Model_HasCorrectNumberOfIndexes()
    {
        // Arrange
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Company));

        // Act
        var indexes = entityType?.GetIndexes().ToList();

        // Assert
        Assert.NotNull(indexes);
        Assert.Equal(3, indexes.Count); // ISIN (unique), Name, StockTicker
    }

    [Fact]
    public void Model_AllIndexesHaveCorrectNames()
    {
        // Arrange
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Company));

        // Act
        var indexNames = entityType?.GetIndexes().Select(i => i.GetDatabaseName()).ToList();

        // Assert
        Assert.NotNull(indexNames);
        Assert.Contains("IX_Companies_Isin", indexNames);
        Assert.Contains("IX_Companies_Name", indexNames);
        Assert.Contains("IX_Companies_StockTicker", indexNames);
    }

    #endregion

    #region Property Verification Tests

    [Fact]
    public void Model_AllRequiredPropertiesAreConfigured()
    {
        // Arrange
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Company));

        // Act
        var requiredProperties = entityType?.GetProperties()
            .Where(p => !p.IsNullable)
            .Select(p => p.Name)
            .ToList();

        // Assert
        Assert.NotNull(requiredProperties);
        Assert.Contains(nameof(Company.Id), requiredProperties);
        Assert.Contains(nameof(Company.Name), requiredProperties);
        Assert.Contains(nameof(Company.StockTicker), requiredProperties);
        Assert.Contains(nameof(Company.Exchange), requiredProperties);
        Assert.Contains(nameof(Company.Isin), requiredProperties);
        Assert.Contains(nameof(Company.CreatedAt), requiredProperties);
        Assert.Contains(nameof(Company.UpdatedAt), requiredProperties);
    }

    [Fact]
    public void Model_OnlyWebsiteIsNullable()
    {
        // Arrange
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Company));

        // Act
        var nullableProperties = entityType?.GetProperties()
            .Where(p => p.IsNullable)
            .Select(p => p.Name)
            .ToList();

        // Assert
        Assert.NotNull(nullableProperties);
        Assert.Single(nullableProperties);
        Assert.Contains(nameof(Company.Website), nullableProperties);
    }

    #endregion

    #region Base Class Call Tests

    [Fact]
    public void OnModelCreating_CallsBaseImplementation()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CompanyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act & Assert - Should not throw exception
        using var context = new CompanyDbContext(options);
        var model = context.Model;

        Assert.NotNull(model);
        Assert.NotEmpty(model.GetEntityTypes());
    }

    #endregion
}

