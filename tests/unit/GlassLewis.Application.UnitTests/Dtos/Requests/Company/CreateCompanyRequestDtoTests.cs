using System.ComponentModel.DataAnnotations;
using GlassLewis.Application.Dtos.Requests.Company;

namespace GlassLewis.Application.UnitTests.Dtos.Requests.Company;


/// <summary>
/// Provides unit tests for the <see cref="CreateCompanyRequestDto"/> class.
/// </summary>
/// <remarks>This test class validates the behavior and functionality of the <see cref="CreateCompanyRequestDto"/>
/// class, including property getters and setters, default values, and validation rules.</remarks>
[Trait("Category", "UnitTests")]

public class CreateCompanyRequestDtoTests
{
    #region Property Tests

    [Fact]
    public void Name_Property_Should_Get_And_Set_Value()
    {
        // Arrange
        var dto = new CreateCompanyRequestDto();
        const string expectedName = "Test Company";

        // Act
        dto.Name = expectedName;

        // Assert
        Assert.Equal(expectedName, dto.Name);
    }

    [Fact]
    public void Name_Property_Should_Have_Default_Empty_String()
    {
        // Arrange & Act
        var dto = new CreateCompanyRequestDto();

        // Assert
        Assert.Equal(string.Empty, dto.Name);
    }

    [Fact]
    public void StockTicker_Property_Should_Get_And_Set_Value()
    {
        // Arrange
        var dto = new CreateCompanyRequestDto();
        const string expectedTicker = "AAPL";

        // Act
        dto.StockTicker = expectedTicker;

        // Assert
        Assert.Equal(expectedTicker, dto.StockTicker);
    }

    [Fact]
    public void StockTicker_Property_Should_Have_Default_Empty_String()
    {
        // Arrange & Act
        var dto = new CreateCompanyRequestDto();

        // Assert
        Assert.Equal(string.Empty, dto.StockTicker);
    }

    [Fact]
    public void Exchange_Property_Should_Get_And_Set_Value()
    {
        // Arrange
        var dto = new CreateCompanyRequestDto();
        const string expectedExchange = "NYSE";

        // Act
        dto.Exchange = expectedExchange;

        // Assert
        Assert.Equal(expectedExchange, dto.Exchange);
    }

    [Fact]
    public void Exchange_Property_Should_Have_Default_Empty_String()
    {
        // Arrange & Act
        var dto = new CreateCompanyRequestDto();

        // Assert
        Assert.Equal(string.Empty, dto.Exchange);
    }

    [Fact]
    public void Isin_Property_Should_Get_And_Set_Value()
    {
        // Arrange
        var dto = new CreateCompanyRequestDto();
        const string expectedIsin = "US0378331005";

        // Act
        dto.Isin = expectedIsin;

        // Assert
        Assert.Equal(expectedIsin, dto.Isin);
    }

    [Fact]
    public void Isin_Property_Should_Have_Default_Empty_String()
    {
        // Arrange & Act
        var dto = new CreateCompanyRequestDto();

        // Assert
        Assert.Equal(string.Empty, dto.Isin);
    }

    [Fact]
    public void Website_Property_Should_Get_And_Set_Value()
    {
        // Arrange
        var dto = new CreateCompanyRequestDto();
        const string expectedWebsite = "https://example.com";

        // Act
        dto.Website = expectedWebsite;

        // Assert
        Assert.Equal(expectedWebsite, dto.Website);
    }

    [Fact]
    public void Website_Property_Should_Have_Default_Null_Value()
    {
        // Arrange & Act
        var dto = new CreateCompanyRequestDto();

        // Assert
        Assert.Null(dto.Website);
    }

    [Fact]
    public void Website_Property_Should_Accept_Null_Value()
    {
        // Arrange
        var dto = new CreateCompanyRequestDto();

        // Act
        dto.Website = null!;

        // Assert
        Assert.Null(dto.Website);
    }

    #endregion

    #region Validation Tests - Name Property

    [Fact]
    public void Name_With_Valid_Value_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = "Valid Company Name";

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Fact]
    public void Name_With_Null_Value_Should_Fail_Required_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = null!;

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Name))
                                   && e.ErrorMessage!.Contains("required"));
    }

    [Fact]
    public void Name_With_Empty_String_Should_Fail_Required_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = string.Empty;

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Name))
                                   && e.ErrorMessage!.Contains("required"));
    }

    [Fact]
    public void Name_With_Whitespace_Should_Fail_Required_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = "   ";

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Name))
                                   && e.ErrorMessage!.Contains("required"));
    }

    [Fact]
    public void Name_With_200_Characters_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = new string('A', 200);

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Fact]
    public void Name_With_201_Characters_Should_Fail_StringLength_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = new string('A', 201);

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Name))
                                   && e.ErrorMessage!.Contains("Name cannot exceed 200 characters"));
    }

    [Fact]
    public void Name_With_1_Character_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = "A";

        // Act & Assert
        AssertValidationPasses(dto);
    }

    #endregion

    #region Validation Tests - StockTicker Property

    [Fact]
    public void StockTicker_With_Valid_Value_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.StockTicker = "AAPL";

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Fact]
    public void StockTicker_With_Null_Value_Should_Fail_Required_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.StockTicker = null!;

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.StockTicker))
                                   && e.ErrorMessage!.Contains("required"));
    }

    [Fact]
    public void StockTicker_With_Empty_String_Should_Fail_Required_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.StockTicker = string.Empty;

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.StockTicker))
                                   && e.ErrorMessage!.Contains("required"));
    }

    [Fact]
    public void StockTicker_With_10_Characters_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.StockTicker = "ABCDEFGHIJ";

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Fact]
    public void StockTicker_With_11_Characters_Should_Fail_StringLength_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.StockTicker = "ABCDEFGHIJK";

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.StockTicker))
                                   && e.ErrorMessage!.Contains("Stock ticker cannot exceed 10 characters"));
    }

    [Fact]
    public void StockTicker_With_1_Character_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.StockTicker = "A";

        // Act & Assert
        AssertValidationPasses(dto);
    }

    #endregion

    #region Validation Tests - Exchange Property

    [Fact]
    public void Exchange_With_Valid_Value_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Exchange = "NYSE";

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Fact]
    public void Exchange_With_Null_Value_Should_Fail_Required_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Exchange = null!;

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Exchange))
                                   && e.ErrorMessage!.Contains("required"));
    }

    [Fact]
    public void Exchange_With_Empty_String_Should_Fail_Required_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Exchange = string.Empty;

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Exchange))
                                   && e.ErrorMessage!.Contains("required"));
    }

    [Fact]
    public void Exchange_With_100_Characters_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Exchange = new string('A', 100);

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Fact]
    public void Exchange_With_101_Characters_Should_Fail_StringLength_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Exchange = new string('A', 101);

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Exchange))
                                   && e.ErrorMessage!.Contains("Exchange cannot exceed 100 characters"));
    }

    [Fact]
    public void Exchange_With_1_Character_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Exchange = "A";

        // Act & Assert
        AssertValidationPasses(dto);
    }

    #endregion

    #region Validation Tests - Isin Property

    [Fact]
    public void Isin_With_Valid_Value_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Isin = "US0378331005"; // Valid ISIN

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Fact]
    public void Isin_With_Null_Value_Should_Fail_Required_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Isin = null!;

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Isin))
                                   && e.ErrorMessage!.Contains("required"));
    }

    [Fact]
    public void Isin_With_Empty_String_Should_Fail_Required_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Isin = string.Empty;

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Isin)));
    }

    [Fact]
    public void Isin_With_Whitespace_Should_Fail_Required_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Isin = "   ";

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Isin)));
    }

    // Note: These tests assume IsinValidation attribute exists and works
    [Fact]
    public void Isin_With_Invalid_Format_Should_Fail_IsinValidation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Isin = "INVALID_ISIN";

        // Act & Assert
        var errors = GetValidationErrors(dto);
        // This test depends on your IsinValidation attribute implementation
        // Adjust the assertion based on your custom validation logic
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Isin)));
    }

    #endregion

    #region Validation Tests - Website Property

    [Fact]
    public void Website_With_Null_Value_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Website = null!;

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Fact]
    public void Website_With_Valid_Url_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Website = "https://www.example.com";

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Fact]
    public void Website_With_Valid_Http_Url_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Website = "http://www.example.com";

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Fact]
    public void Website_With_Invalid_Url_Should_Fail_Url_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Website = "not-a-valid-url";

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Website))
                                   && e.ErrorMessage!.Contains("Website must be a valid URL"));
    }

    [Fact]
    public void Website_With_Null_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Website = null!;

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Fact]
    public void Website_With_500_Characters_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        var longUrl = "https://www.example.com/" + new string('a', 500 - 25);
        dto.Website = longUrl;

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Fact]
    public void Website_With_501_Characters_Should_Fail_StringLength_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        var tooLongUrl = "https://www.example.com/" + new string('a', 501);
        dto.Website = tooLongUrl;

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Website))
                                   && e.ErrorMessage!.Contains("Website URL cannot exceed 500 characters"));
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://example.com")]
    [InlineData("https://www.example.com")]
    [InlineData("https://subdomain.example.com")]
    [InlineData("https://example.com/path")]
    [InlineData("https://example.com/path?query=value")]
    [InlineData("https://example.com:8080")]
    public void Website_With_Valid_Url_Formats_Should_Pass_Validation(string validUrl)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Website = validUrl;

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Theory]
    [InlineData("invalid-url")]
    [InlineData("javascript:alert('xss')")]
    [InlineData("mailto:test@example.com")]
    [InlineData("file:///path/to/file")]
    public void Website_With_Invalid_Url_Formats_Should_Fail_Validation(string invalidUrl)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Website = invalidUrl;

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Website)));
    }

    #endregion

    #region Combined Validation Tests

    [Fact]
    public void Valid_Dto_Should_Pass_All_Validations()
    {
        // Arrange
        var dto = CreateValidDto();

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Fact]
    public void Dto_With_All_Required_Fields_Null_Should_Fail_Multiple_Validations()
    {
        // Arrange
        var dto = new CreateCompanyRequestDto
        {
            Name = null!,
            StockTicker = null!,
            Exchange = null!,
            Isin = null!,
            Website = null
        };

        // Act
        var errors = GetValidationErrors(dto);

        // Assert
        Assert.True(errors.Count >= 4); // At least 4 required field errors
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Name)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.StockTicker)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Exchange)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Isin)));
    }

    [Fact]
    public void Dto_With_All_Fields_Exceeding_MaxLength_Should_Fail_Multiple_Validations()
    {
        // Arrange
        var dto = new CreateCompanyRequestDto
        {
            Name = new string('A', 201),
            StockTicker = new string('B', 11),
            Exchange = new string('C', 101),
            Isin = "US0378331005", // Valid ISIN
            Website = "https://www.example.com/" + new string('d', 501)
        };

        // Act
        var errors = GetValidationErrors(dto);

        // Assert
        Assert.True(errors.Count >= 4); // At least 4 length validation errors
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Name)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.StockTicker)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Exchange)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateCompanyRequestDto.Website)));
    }

    #endregion

    #region Object Construction Tests

    [Fact]
    public void CreateCompanyRequestDto_Should_Be_Instantiable()
    {
        // Act
        var dto = new CreateCompanyRequestDto();

        // Assert
        Assert.NotNull(dto);
        Assert.IsType<CreateCompanyRequestDto>(dto);
    }

    [Fact]
    public void CreateCompanyRequestDto_Should_Initialize_Properties_With_Default_Values()
    {
        // Act
        var dto = new CreateCompanyRequestDto();

        // Assert
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.StockTicker);
        Assert.Equal(string.Empty, dto.Exchange);
        Assert.Equal(string.Empty, dto.Isin);
        Assert.Null(dto.Website);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void Properties_Should_Handle_Unicode_Characters()
    {
        // Arrange
        var dto = new CreateCompanyRequestDto();
        const string unicodeName = "Tëst Çömpäny 🏢";
        const string unicodeWebsite = "https://tëst.çöm";

        // Act
        dto.Name = unicodeName;
        dto.Website = unicodeWebsite;

        // Assert
        Assert.Equal(unicodeName, dto.Name);
        Assert.Equal(unicodeWebsite, dto.Website);
    }

    [Fact]
    public void Properties_Should_Handle_Leading_And_Trailing_Whitespace()
    {
        // Arrange
        var dto = new CreateCompanyRequestDto();
        const string nameWithWhitespace = "  Test Company  ";

        // Act
        dto.Name = nameWithWhitespace;

        // Assert
        Assert.Equal(nameWithWhitespace, dto.Name);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Creates a valid DTO for testing
    /// </summary>
    private static CreateCompanyRequestDto CreateValidDto()
    {
        return new CreateCompanyRequestDto
        {
            Name = "Test Company",
            StockTicker = "TEST",
            Exchange = "NYSE",
            Isin = "US0378331005", // Valid Apple ISIN
            Website = "https://www.testcompany.com"
        };
    }

    /// <summary>
    /// Gets validation errors for a DTO
    /// </summary>
    private static List<ValidationResult> GetValidationErrors(object obj)
    {
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(obj, context, results, true);
        return results;
    }

    /// <summary>
    /// Asserts that validation passes for a DTO
    /// </summary>
    private static void AssertValidationPasses(object obj)
    {
        var errors = GetValidationErrors(obj);
        Assert.Empty(errors);
    }

    #endregion
}

/// <summary>
/// Contains unit tests for validating the attributes applied to properties of the <see cref="CreateCompanyRequestDto"/>
/// class.
/// </summary>
/// <remarks>This test class ensures that the properties of <see cref="CreateCompanyRequestDto"/> are correctly
/// decorated with validation attributes, such as <see cref="RequiredAttribute"/>, <see cref="StringLengthAttribute"/>,
/// and <see cref="UrlAttribute"/>, to enforce data integrity and constraints.</remarks>
[Trait("Category", "UnitTests")]
public class CreateCompanyRequestDtoAttributeTests
{
    [Fact]
    public void Name_Property_Should_Have_Required_Attribute()
    {
        // Arrange
        var property = typeof(CreateCompanyRequestDto).GetProperty(nameof(CreateCompanyRequestDto.Name));

        // Act
        var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false);

        // Assert
        Assert.NotNull(requiredAttribute);
        Assert.NotEmpty(requiredAttribute);
    }

    [Fact]
    public void Name_Property_Should_Have_StringLength_Attribute_With_Correct_MaxLength()
    {
        // Arrange
        var property = typeof(CreateCompanyRequestDto).GetProperty(nameof(CreateCompanyRequestDto.Name));

        // Act
        var stringLengthAttribute = property?.GetCustomAttributes(typeof(StringLengthAttribute), false)
            .FirstOrDefault() as StringLengthAttribute;

        // Assert
        Assert.NotNull(stringLengthAttribute);
        Assert.Equal(200, stringLengthAttribute.MaximumLength);
        Assert.Equal("Name cannot exceed 200 characters", stringLengthAttribute.ErrorMessage);
    }

    [Fact]
    public void StockTicker_Property_Should_Have_Required_Attribute()
    {
        // Arrange
        var property = typeof(CreateCompanyRequestDto).GetProperty(nameof(CreateCompanyRequestDto.StockTicker));

        // Act
        var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false);

        // Assert
        Assert.NotNull(requiredAttribute);
        Assert.NotEmpty(requiredAttribute);
    }

    [Fact]
    public void StockTicker_Property_Should_Have_StringLength_Attribute_With_Correct_MaxLength()
    {
        // Arrange
        var property = typeof(CreateCompanyRequestDto).GetProperty(nameof(CreateCompanyRequestDto.StockTicker));

        // Act
        var stringLengthAttribute = property?.GetCustomAttributes(typeof(StringLengthAttribute), false)
            .FirstOrDefault() as StringLengthAttribute;

        // Assert
        Assert.NotNull(stringLengthAttribute);
        Assert.Equal(10, stringLengthAttribute.MaximumLength);
        Assert.Equal("Stock ticker cannot exceed 10 characters", stringLengthAttribute.ErrorMessage);
    }

    [Fact]
    public void Exchange_Property_Should_Have_Required_Attribute()
    {
        // Arrange
        var property = typeof(CreateCompanyRequestDto).GetProperty(nameof(CreateCompanyRequestDto.Exchange));

        // Act
        var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false);

        // Assert
        Assert.NotNull(requiredAttribute);
        Assert.NotEmpty(requiredAttribute);
    }

    [Fact]
    public void Exchange_Property_Should_Have_StringLength_Attribute_With_Correct_MaxLength()
    {
        // Arrange
        var property = typeof(CreateCompanyRequestDto).GetProperty(nameof(CreateCompanyRequestDto.Exchange));

        // Act
        var stringLengthAttribute = property?.GetCustomAttributes(typeof(StringLengthAttribute), false)
            .FirstOrDefault() as StringLengthAttribute;

        // Assert
        Assert.NotNull(stringLengthAttribute);
        Assert.Equal(100, stringLengthAttribute.MaximumLength);
        Assert.Equal("Exchange cannot exceed 100 characters", stringLengthAttribute.ErrorMessage);
    }

    [Fact]
    public void Isin_Property_Should_Have_Required_Attribute_With_AllowEmptyStrings_False()
    {
        // Arrange
        var property = typeof(CreateCompanyRequestDto).GetProperty(nameof(CreateCompanyRequestDto.Isin));

        // Act
        var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false)
            .FirstOrDefault() as RequiredAttribute;

        // Assert
        Assert.NotNull(requiredAttribute);
        Assert.False(requiredAttribute.AllowEmptyStrings);
    }

    [Fact]
    public void Website_Property_Should_Have_Url_Attribute()
    {
        // Arrange
        var property = typeof(CreateCompanyRequestDto).GetProperty(nameof(CreateCompanyRequestDto.Website));

        // Act
        var urlAttribute = property?.GetCustomAttributes(typeof(UrlAttribute), false)
            .FirstOrDefault() as UrlAttribute;

        // Assert
        Assert.NotNull(urlAttribute);
        Assert.Equal("Website must be a valid URL", urlAttribute.ErrorMessage);
    }

    [Fact]
    public void Website_Property_Should_Have_StringLength_Attribute_With_Correct_MaxLength()
    {
        // Arrange
        var property = typeof(CreateCompanyRequestDto).GetProperty(nameof(CreateCompanyRequestDto.Website));

        // Act
        var stringLengthAttribute = property?.GetCustomAttributes(typeof(StringLengthAttribute), false)
            .FirstOrDefault() as StringLengthAttribute;

        // Assert
        Assert.NotNull(stringLengthAttribute);
        Assert.Equal(500, stringLengthAttribute.MaximumLength);
        Assert.Equal("Website URL cannot exceed 500 characters", stringLengthAttribute.ErrorMessage);
    }

    [Fact]
    public void Website_Property_Should_Be_Nullable()
    {
        // Arrange
        var property = typeof(CreateCompanyRequestDto).GetProperty(nameof(CreateCompanyRequestDto.Website));

        // Act
        var isNullable = Nullable.GetUnderlyingType(property?.PropertyType!) != null ||
                        !property?.PropertyType.IsValueType == true;

        // Assert
        Assert.True(isNullable);
    }
}
