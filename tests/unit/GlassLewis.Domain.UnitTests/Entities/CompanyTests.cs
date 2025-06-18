using System.ComponentModel.DataAnnotations;
using GlassLewis.Domain.Entities;
using System.Reflection;

namespace GlassLewis.Domain.UnitTests.Entities;

/// <summary>
/// Contains unit tests for the <see cref="Company"/> class, verifying the behavior of its properties, default values,
/// validation attributes, and data validation logic.
/// </summary>
/// <remarks>This test class is categorized as "UnitTests" using the <see cref="TraitAttribute"/>. It includes
/// tests for property getters and setters, validation attributes, and edge cases to ensure the <see cref="Company"/>
/// class behaves as expected under various conditions.</remarks>
[Trait("Category", "UnitTests")]
public class CompanyTests
{
    #region Property Tests

    [Fact]
    public void Id_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var company = new Company();
        var expectedId = Guid.NewGuid();

        // Act
        company.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, company.Id);
    }

    [Fact]
    public void Name_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var company = new Company();
        const string expectedName = "Test Company";

        // Act
        company.Name = expectedName;

        // Assert
        Assert.Equal(expectedName, company.Name);
    }

    [Fact]
    public void StockTicker_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var company = new Company();
        const string expectedTicker = "TEST";

        // Act
        company.StockTicker = expectedTicker;

        // Assert
        Assert.Equal(expectedTicker, company.StockTicker);
    }

    [Fact]
    public void Exchange_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var company = new Company();
        const string expectedExchange = "NYSE";

        // Act
        company.Exchange = expectedExchange;

        // Assert
        Assert.Equal(expectedExchange, company.Exchange);
    }

    [Fact]
    public void Isin_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var company = new Company();
        const string expectedIsin = "US1234567890";

        // Act
        company.Isin = expectedIsin;

        // Assert
        Assert.Equal(expectedIsin, company.Isin);
    }

    [Fact]
    public void Website_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var company = new Company();
        const string expectedWebsite = "https://example.com";

        // Act
        company.Website = expectedWebsite;

        // Assert
        Assert.Equal(expectedWebsite, company.Website);
    }

    [Fact]
    public void Website_SetToNull_ReturnsNull()
    {
        // Arrange
        var company = new Company();

        // Act
        company.Website = null;

        // Assert
        Assert.Null(company.Website);
    }

    [Fact]
    public void CreatedAt_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var company = new Company();
        var expectedDate = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        company.CreatedAt = expectedDate;

        // Assert
        Assert.Equal(expectedDate, company.CreatedAt);
    }

    [Fact]
    public void UpdatedAt_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var company = new Company();
        var expectedDate = new DateTime(2023, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        // Act
        company.UpdatedAt = expectedDate;

        // Assert
        Assert.Equal(expectedDate, company.UpdatedAt);
    }

    #endregion

    #region Default Values Tests

    [Fact]
    public void Constructor_SetsDefaultValues_WhenCreated()
    {
        // Arrange & Act
        var company = new Company();

        // Assert
        Assert.Equal(Guid.Empty, company.Id);
        Assert.Equal(string.Empty, company.Name);
        Assert.Equal(string.Empty, company.StockTicker);
        Assert.Equal(string.Empty, company.Exchange);
        Assert.Equal(string.Empty, company.Isin);
        Assert.Null(company.Website);

        // CreatedAt and UpdatedAt should be set to approximately now (within 1 second)
        var now = DateTime.UtcNow;
        Assert.True((now - company.CreatedAt).TotalSeconds < 1);
        Assert.True((now - company.UpdatedAt).TotalSeconds < 1);
    }

    [Fact]
    public void Constructor_SetsTimestampsToUtcNow_WhenCreated()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var company = new Company();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(company.CreatedAt >= beforeCreation);
        Assert.True(company.CreatedAt <= afterCreation);
        Assert.True(company.UpdatedAt >= beforeCreation);
        Assert.True(company.UpdatedAt <= afterCreation);
    }

    #endregion

    #region Validation Attribute Tests

    [Fact]
    public void Id_HasKeyAttribute()
    {
        // Arrange
        var property = typeof(Company).GetProperty(nameof(Company.Id));

        // Act
        var keyAttribute = property?.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(keyAttribute);
        Assert.IsType<KeyAttribute>(keyAttribute);
    }

    [Fact]
    public void Name_HasRequiredAttribute()
    {
        // Arrange
        var property = typeof(Company).GetProperty(nameof(Company.Name));

        // Act
        var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(requiredAttribute);
        Assert.IsType<RequiredAttribute>(requiredAttribute);
    }

    [Fact]
    public void Name_HasStringLengthAttribute_WithCorrectMaxLength()
    {
        // Arrange
        var property = typeof(Company).GetProperty(nameof(Company.Name));

        // Act
        var stringLengthAttribute = property?.GetCustomAttributes(typeof(StringLengthAttribute), false)
            .FirstOrDefault() as StringLengthAttribute;

        // Assert
        Assert.NotNull(stringLengthAttribute);
        Assert.Equal(200, stringLengthAttribute.MaximumLength);
    }

    [Fact]
    public void StockTicker_HasRequiredAttribute()
    {
        // Arrange
        var property = typeof(Company).GetProperty(nameof(Company.StockTicker));

        // Act
        var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(requiredAttribute);
        Assert.IsType<RequiredAttribute>(requiredAttribute);
    }

    [Fact]
    public void StockTicker_HasStringLengthAttribute_WithCorrectMaxLength()
    {
        // Arrange
        var property = typeof(Company).GetProperty(nameof(Company.StockTicker));

        // Act
        var stringLengthAttribute = property?.GetCustomAttributes(typeof(StringLengthAttribute), false)
            .FirstOrDefault() as StringLengthAttribute;

        // Assert
        Assert.NotNull(stringLengthAttribute);
        Assert.Equal(10, stringLengthAttribute.MaximumLength);
    }

    [Fact]
    public void Exchange_HasRequiredAttribute()
    {
        // Arrange
        var property = typeof(Company).GetProperty(nameof(Company.Exchange));

        // Act
        var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(requiredAttribute);
        Assert.IsType<RequiredAttribute>(requiredAttribute);
    }

    [Fact]
    public void Exchange_HasStringLengthAttribute_WithCorrectMaxLength()
    {
        // Arrange
        var property = typeof(Company).GetProperty(nameof(Company.Exchange));

        // Act
        var stringLengthAttribute = property?.GetCustomAttributes(typeof(StringLengthAttribute), false)
            .FirstOrDefault() as StringLengthAttribute;

        // Assert
        Assert.NotNull(stringLengthAttribute);
        Assert.Equal(100, stringLengthAttribute.MaximumLength);
    }

    [Fact]
    public void Isin_HasRequiredAttribute()
    {
        // Arrange
        var property = typeof(Company).GetProperty(nameof(Company.Isin));

        // Act
        var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(requiredAttribute);
        Assert.IsType<RequiredAttribute>(requiredAttribute);
    }

    [Fact]
    public void Isin_HasStringLengthAttribute_WithCorrectLengths()
    {
        // Arrange
        var property = typeof(Company).GetProperty(nameof(Company.Isin));

        // Act
        var stringLengthAttribute = property?.GetCustomAttributes(typeof(StringLengthAttribute), false)
            .FirstOrDefault() as StringLengthAttribute;

        // Assert
        Assert.NotNull(stringLengthAttribute);
        Assert.Equal(12, stringLengthAttribute.MaximumLength);
        Assert.Equal(12, stringLengthAttribute.MinimumLength);
    }

    [Fact]
    public void Website_HasUrlAttribute()
    {
        // Arrange
        var property = typeof(Company).GetProperty(nameof(Company.Website));

        // Act
        var urlAttribute = property?.GetCustomAttributes(typeof(UrlAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(urlAttribute);
        Assert.IsType<UrlAttribute>(urlAttribute);
    }

    [Fact]
    public void Website_HasStringLengthAttribute_WithCorrectMaxLength()
    {
        // Arrange
        var property = typeof(Company).GetProperty(nameof(Company.Website));

        // Act
        var stringLengthAttribute = property?.GetCustomAttributes(typeof(StringLengthAttribute), false)
            .FirstOrDefault() as StringLengthAttribute;

        // Assert
        Assert.NotNull(stringLengthAttribute);
        Assert.Equal(500, stringLengthAttribute.MaximumLength);
    }

    [Fact]
    public void Website_DoesNotHaveRequiredAttribute()
    {
        // Arrange
        var property = typeof(Company).GetProperty(nameof(Company.Website));

        // Act
        var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

        // Assert
        Assert.Null(requiredAttribute);
    }

    #endregion

    #region Data Validation Tests

    [Fact]
    public void ValidateObject_ReturnsValid_WhenAllRequiredPropertiesAreSet()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            StockTicker = "TEST",
            Exchange = "NYSE",
            Isin = "US1234567890",
            Website = "https://example.com"
        };

        var validationContext = new ValidationContext(company);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(company, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ValidateObject_ReturnsValid_WhenWebsiteIsNull()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            StockTicker = "TEST",
            Exchange = "NYSE",
            Isin = "US1234567890",
            Website = null
        };

        var validationContext = new ValidationContext(company);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(company, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ValidateObject_ReturnsInvalid_WhenNameIsEmpty()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            StockTicker = "TEST",
            Exchange = "NYSE",
            Isin = "US1234567890"
        };

        var validationContext = new ValidationContext(company);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(company, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(Company.Name)));
    }

    [Fact]
    public void ValidateObject_ReturnsInvalid_WhenNameExceedsMaxLength()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = new string('A', 201), // 201 characters, exceeds 200 limit
            StockTicker = "TEST",
            Exchange = "NYSE",
            Isin = "US1234567890"
        };

        var validationContext = new ValidationContext(company);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(company, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(Company.Name)));
    }

    [Fact]
    public void ValidateObject_ReturnsInvalid_WhenStockTickerExceedsMaxLength()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            StockTicker = "VERYLONGTICKER", // 13 characters, exceeds 10 limit
            Exchange = "NYSE",
            Isin = "US1234567890"
        };

        var validationContext = new ValidationContext(company);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(company, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(Company.StockTicker)));
    }

    [Fact]
    public void ValidateObject_ReturnsInvalid_WhenExchangeExceedsMaxLength()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            StockTicker = "TEST",
            Exchange = new string('X', 101), // 101 characters, exceeds 100 limit
            Isin = "US1234567890"
        };

        var validationContext = new ValidationContext(company);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(company, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(Company.Exchange)));
    }

    [Fact]
    public void ValidateObject_ReturnsInvalid_WhenIsinIsNotExactly12Characters()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            StockTicker = "TEST",
            Exchange = "NYSE",
            Isin = "US123456789" // 11 characters, not exactly 12
        };

        var validationContext = new ValidationContext(company);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(company, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(Company.Isin)));
    }

    [Fact]
    public void ValidateObject_ReturnsInvalid_WhenWebsiteIsInvalidUrl()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            StockTicker = "TEST",
            Exchange = "NYSE",
            Isin = "US1234567890",
            Website = "not-a-valid-url"
        };

        var validationContext = new ValidationContext(company);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(company, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(Company.Website)));
    }

    [Fact]
    public void ValidateObject_ReturnsInvalid_WhenWebsiteExceedsMaxLength()
    {
        // Arrange
        var longUrl = "https://example.com/" + new string('a', 500); // Exceeds 500 character limit
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            StockTicker = "TEST",
            Exchange = "NYSE",
            Isin = "US1234567890",
            Website = longUrl
        };

        var validationContext = new ValidationContext(company);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(company, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(Company.Website)));
    }

    #endregion

    #region Edge Cases and Boundary Tests

    [Fact]
    public void Name_AtMaxLength_IsValid()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = new string('A', 200), // Exactly 200 characters
            StockTicker = "TEST",
            Exchange = "NYSE",
            Isin = "US1234567890"
        };

        var validationContext = new ValidationContext(company);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(company, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void StockTicker_AtMaxLength_IsValid()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            StockTicker = "1234567890", // Exactly 10 characters
            Exchange = "NYSE",
            Isin = "US1234567890"
        };

        var validationContext = new ValidationContext(company);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(company, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void Exchange_AtMaxLength_IsValid()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            StockTicker = "TEST",
            Exchange = new string('X', 100), // Exactly 100 characters
            Isin = "US1234567890"
        };

        var validationContext = new ValidationContext(company);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(company, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void Website_AtMaxLength_IsValid()
    {
        // Arrange
        var website = "https://example.com/" + new string('a', 480); // Exactly 500 characters total
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            StockTicker = "TEST",
            Exchange = "NYSE",
            Isin = "US1234567890",
            Website = website
        };

        var validationContext = new ValidationContext(company);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(company, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void AllProperties_CanBeSetToNonDefaultValues()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Custom Company";
        var stockTicker = "CUSTOM";
        var exchange = "CUSTOM_EXCHANGE";
        var isin = "US9876543210";
        var website = "https://custom.com";
        var createdAt = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var updatedAt = new DateTime(2023, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        // Act
        var company = new Company
        {
            Id = id,
            Name = name,
            StockTicker = stockTicker,
            Exchange = exchange,
            Isin = isin,
            Website = website,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        // Assert
        Assert.Equal(id, company.Id);
        Assert.Equal(name, company.Name);
        Assert.Equal(stockTicker, company.StockTicker);
        Assert.Equal(exchange, company.Exchange);
        Assert.Equal(isin, company.Isin);
        Assert.Equal(website, company.Website);
        Assert.Equal(createdAt, company.CreatedAt);
        Assert.Equal(updatedAt, company.UpdatedAt);
    }

    #endregion

    #region Property Type Tests

    [Fact]
    public void Properties_HaveCorrectTypes()
    {
        // Arrange
        var companyType = typeof(Company);

        // Act & Assert
        Assert.Equal(typeof(Guid), companyType.GetProperty(nameof(Company.Id))?.PropertyType);
        Assert.Equal(typeof(string), companyType.GetProperty(nameof(Company.Name))?.PropertyType);
        Assert.Equal(typeof(string), companyType.GetProperty(nameof(Company.StockTicker))?.PropertyType);
        Assert.Equal(typeof(string), companyType.GetProperty(nameof(Company.Exchange))?.PropertyType);
        Assert.Equal(typeof(string), companyType.GetProperty(nameof(Company.Isin))?.PropertyType);
        Assert.Equal(typeof(string), companyType.GetProperty(nameof(Company.Website))?.PropertyType);
        Assert.Equal(typeof(DateTime), companyType.GetProperty(nameof(Company.CreatedAt))?.PropertyType);
        Assert.Equal(typeof(DateTime), companyType.GetProperty(nameof(Company.UpdatedAt))?.PropertyType);
    }

    [Fact]
    public void Website_IsNullable()
    {
        // Arrange
        var websiteProperty = typeof(Company).GetProperty(nameof(Company.Website));

        // Act
        var isNullable = websiteProperty?.PropertyType == typeof(string);
        var nullabilityContext = new NullabilityInfoContext();
        var nullabilityInfo = nullabilityContext.Create(websiteProperty!);

        // Assert
        Assert.True(isNullable);
        Assert.Equal(NullabilityState.Nullable, nullabilityInfo.WriteState);
    }

    #endregion
}
