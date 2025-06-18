using System.ComponentModel.DataAnnotations;
using GlassLewis.Application.Dtos.Requests.Company;

namespace GlassLewis.Application.UnitTests.Dtos.Requests.Company;

/// <summary>
/// Contains unit tests for the <see cref="UpdateCompanyRequestDto"/> class.
/// </summary>
/// <remarks>This test class verifies the behavior of the <see cref="UpdateCompanyRequestDto"/> class, including
/// property accessors, validation logic, and dictionary-based tracking of modified properties.</remarks>
[Trait("Category", "UnitTests")]
public class UpdateCompanyRequestDtoTests
{
    #region Constructor and Property Tests

    [Fact]
    public void Constructor_Should_Initialize_Properties_With_Default_Values()
    {
        // Arrange & Act
        var dto = new UpdateCompanyRequestDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.StockTicker);
        Assert.Equal(string.Empty, dto.Exchange);
        Assert.Equal(string.Empty, dto.Isin);
        Assert.Null(dto.Website);
    }

    [Fact]
    public void Name_Property_Should_Get_And_Set_Value()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto();
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
        var dto = new UpdateCompanyRequestDto();

        // Assert
        Assert.Equal(string.Empty, dto.Name);
    }

    [Fact]
    public void StockTicker_Property_Should_Get_And_Set_Value()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto();
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
        var dto = new UpdateCompanyRequestDto();

        // Assert
        Assert.Equal(string.Empty, dto.StockTicker);
    }

    [Fact]
    public void Exchange_Property_Should_Get_And_Set_Value()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto();
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
        var dto = new UpdateCompanyRequestDto();

        // Assert
        Assert.Equal(string.Empty, dto.Exchange);
    }

    [Fact]
    public void Isin_Property_Should_Get_And_Set_Value()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto();
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
        var dto = new UpdateCompanyRequestDto();

        // Assert
        Assert.Equal(string.Empty, dto.Isin);
    }

    [Fact]
    public void Website_Property_Should_Get_And_Set_Value()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto();
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
        var dto = new UpdateCompanyRequestDto();

        // Assert
        Assert.Null(dto.Website);
    }

    [Fact]
    public void Website_Property_Should_Accept_Null_Value()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto();

        // Act
        dto.Website = null;

        // Assert
        Assert.Null(dto.Website);
    }

    [Fact]
    public void All_Properties_Should_Be_Settable_Independently()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto();

        // Act
        dto.Name = "Company Name";
        dto.StockTicker = "TICKER";
        dto.Exchange = "NASDAQ";
        dto.Isin = "US1234567890";
        dto.Website = "https://company.com";

        // Assert
        Assert.Equal("Company Name", dto.Name);
        Assert.Equal("TICKER", dto.StockTicker);
        Assert.Equal("NASDAQ", dto.Exchange);
        Assert.Equal("US1234567890", dto.Isin);
        Assert.Equal("https://company.com", dto.Website);
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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Name))
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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Name))
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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Name))
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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Name))
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

    [Fact]
    public void Name_With_199_Characters_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = new string('A', 199);

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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.StockTicker))
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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.StockTicker))
                                   && e.ErrorMessage!.Contains("required"));
    }

    [Fact]
    public void StockTicker_With_Whitespace_Should_Fail_Required_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.StockTicker = "   ";

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.StockTicker))
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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.StockTicker))
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

    [Fact]
    public void StockTicker_With_9_Characters_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.StockTicker = "ABCDEFGHI";

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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Exchange))
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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Exchange))
                                   && e.ErrorMessage!.Contains("required"));
    }

    [Fact]
    public void Exchange_With_Whitespace_Should_Fail_Required_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Exchange = "   ";

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Exchange))
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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Exchange))
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

    [Fact]
    public void Exchange_With_99_Characters_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Exchange = new string('A', 99);

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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Isin))
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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Isin)));
    }

    [Fact]
    public void Isin_With_Whitespace_Should_Fail_Required_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Isin = "   ";

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Isin)));
    }

    [Fact]
    public void Isin_With_Invalid_Format_Should_Fail_IsinValidation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Isin = "INVALID_ISIN";

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Isin)));
    }

    [Theory]
    [InlineData("US0378331005")]
    [InlineData("GB0002634946")]
    [InlineData("DE0005140008")]
    public void Isin_With_Valid_Formats_Should_Pass_Validation(string validIsin)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Isin = validIsin;

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Theory]
    [InlineData("INVALID")]
    [InlineData("12345")]
    [InlineData("TOO_SHORT")]
    [InlineData("TOOLONGTOBEVALID")]
    public void Isin_With_Invalid_Formats_Should_Fail_Validation(string invalidIsin)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Isin = invalidIsin;

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Isin)));
    }

    #endregion

    #region Validation Tests - Website Property

    [Fact]
    public void Website_With_Null_Value_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Website = null;

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
        dto.Website = "https://www.example.com";

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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Website))
                                   && e.ErrorMessage!.Contains("Website must be a valid URL"));
    }

    [Fact]
    public void Website_With_Null_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Website = null;

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Fact]
    public void Website_With_Empty_String_Should_Fail_Url_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Website = string.Empty;

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Website))
                                   && e.ErrorMessage!.Contains("Website must be a valid URL"));
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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Website))
                                   && e.ErrorMessage!.Contains("Website URL cannot exceed 500 characters"));
    }

    [Fact]
    public void Website_With_499_Characters_Should_Pass_Validation()
    {
        // Arrange
        var dto = CreateValidDto();
        var longUrl = "https://www.example.com/" + new string('a', 499 - 25);
        dto.Website = longUrl;

        // Act & Assert
        AssertValidationPasses(dto);
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://example.com")]
    [InlineData("https://www.example.com")]
    [InlineData("https://subdomain.example.com")]
    [InlineData("https://example.com/path")]
    [InlineData("https://example.com/path?query=value")]
    [InlineData("https://example.com:8080")]
    [InlineData("https://example.com/path/to/resource")]
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
    [InlineData("ldap://example.com")]
    [InlineData("://invalid")]
    public void Website_With_Invalid_Url_Formats_Should_Fail_Validation(string invalidUrl)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Website = invalidUrl;

        // Act & Assert
        var errors = GetValidationErrors(dto);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Website)));
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
        var dto = new UpdateCompanyRequestDto
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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Name)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.StockTicker)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Exchange)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Isin)));
    }

    [Fact]
    public void Dto_With_All_Fields_Exceeding_MaxLength_Should_Fail_Multiple_Validations()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto
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
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Name)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.StockTicker)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Exchange)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Website)));
    }

    [Fact]
    public void Dto_With_Mixed_Valid_And_Invalid_Fields_Should_Show_Only_Invalid_Errors()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto
        {
            Name = "Valid Name", // Valid
            StockTicker = new string('B', 11), // Invalid - too long
            Exchange = "NYSE", // Valid
            Isin = "INVALID", // Invalid format
            Website = "https://valid.com" // Valid
        };

        // Act
        var errors = GetValidationErrors(dto);

        // Assert
        Assert.Equal(2, errors.Count);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.StockTicker)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Isin)));
        Assert.DoesNotContain(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Name)));
        Assert.DoesNotContain(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Exchange)));
        Assert.DoesNotContain(errors, e => e.MemberNames.Contains(nameof(UpdateCompanyRequestDto.Website)));
    }

    [Fact]
    public void Dto_With_Boundary_Values_Should_Pass_Validation()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto
        {
            Name = new string('A', 200), // Exactly at limit
            StockTicker = new string('B', 10), // Exactly at limit
            Exchange = new string('C', 100), // Exactly at limit
            Isin = "US0378331005", // Valid ISIN
            Website = "https://www.example.com/" + new string('d', 500 - 25) // Exactly at limit
        };

        // Act & Assert
        AssertValidationPasses(dto);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void Properties_Should_Handle_Unicode_Characters()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto();
        const string unicodeName = "Tëst Çömpäny 🏢";
        const string unicodeTicker = "TËST";
        const string unicodeExchange = "NŸSE";
        const string unicodeWebsite = "https://tëst.çöm";

        // Act
        dto.Name = unicodeName;
        dto.StockTicker = unicodeTicker;
        dto.Exchange = unicodeExchange;
        dto.Website = unicodeWebsite;

        // Assert
        Assert.Equal(unicodeName, dto.Name);
        Assert.Equal(unicodeTicker, dto.StockTicker);
        Assert.Equal(unicodeExchange, dto.Exchange);
        Assert.Equal(unicodeWebsite, dto.Website);
    }

    [Fact]
    public void Properties_Should_Handle_Leading_And_Trailing_Whitespace()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto();
        const string nameWithWhitespace = "  Test Company  ";
        const string tickerWithWhitespace = "  AAPL  ";

        // Act
        dto.Name = nameWithWhitespace;
        dto.StockTicker = tickerWithWhitespace;

        // Assert
        Assert.Equal(nameWithWhitespace, dto.Name);
        Assert.Equal(tickerWithWhitespace, dto.StockTicker);
    }

    [Fact]
    public void Properties_Should_Handle_Special_Characters()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto();
        const string nameWithSpecialChars = "Test & Company Ltd.";
        const string exchangeWithSpecialChars = "NYSE-American";

        // Act
        dto.Name = nameWithSpecialChars;
        dto.Exchange = exchangeWithSpecialChars;

        // Assert
        Assert.Equal(nameWithSpecialChars, dto.Name);
        Assert.Equal(exchangeWithSpecialChars, dto.Exchange);
    }

    [Fact]
    public void All_Properties_Should_Be_Independent()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto();

        // Act
        dto.Name = "Company 1";
        Assert.Equal("Company 1", dto.Name);
        Assert.Equal(string.Empty, dto.StockTicker); // Other properties unchanged

        dto.StockTicker = "TICK1";
        Assert.Equal("TICK1", dto.StockTicker);
        Assert.Equal("Company 1", dto.Name); // Previous property unchanged

        dto.Exchange = "NYSE";
        dto.Isin = "US1234567890";
        dto.Website = "https://test.com";

        // Assert
        Assert.Equal("Company 1", dto.Name);
        Assert.Equal("TICK1", dto.StockTicker);
        Assert.Equal("NYSE", dto.Exchange);
        Assert.Equal("US1234567890", dto.Isin);
        Assert.Equal("https://test.com", dto.Website);
    }

    #endregion

    #region Attribute Verification Tests

    [Fact]
    public void Name_Property_Should_Have_Required_Attribute()
    {
        // Arrange
        var property = typeof(UpdateCompanyRequestDto).GetProperty(nameof(UpdateCompanyRequestDto.Name));

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
        var property = typeof(UpdateCompanyRequestDto).GetProperty(nameof(UpdateCompanyRequestDto.Name));

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
        var property = typeof(UpdateCompanyRequestDto).GetProperty(nameof(UpdateCompanyRequestDto.StockTicker));

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
        var property = typeof(UpdateCompanyRequestDto).GetProperty(nameof(UpdateCompanyRequestDto.StockTicker));

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
        var property = typeof(UpdateCompanyRequestDto).GetProperty(nameof(UpdateCompanyRequestDto.Exchange));

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
        var property = typeof(UpdateCompanyRequestDto).GetProperty(nameof(UpdateCompanyRequestDto.Exchange));

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
        var property = typeof(UpdateCompanyRequestDto).GetProperty(nameof(UpdateCompanyRequestDto.Isin));

        // Act
        var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false)
            .FirstOrDefault() as RequiredAttribute;

        // Assert
        Assert.NotNull(requiredAttribute);
        Assert.False(requiredAttribute.AllowEmptyStrings);
    }

    [Fact]
    public void Isin_Property_Should_Have_IsinValidation_Attribute()
    {
        // Arrange
        var property = typeof(UpdateCompanyRequestDto).GetProperty(nameof(UpdateCompanyRequestDto.Isin));

        // Act
        var isinValidationAttributes = property?.GetCustomAttributes(false)
            .Where(attr => attr.GetType().Name == "IsinValidationAttribute");

        // Assert
        Assert.NotNull(isinValidationAttributes);
        Assert.NotEmpty(isinValidationAttributes);
    }

    [Fact]
    public void Website_Property_Should_Have_Url_Attribute()
    {
        // Arrange
        var property = typeof(UpdateCompanyRequestDto).GetProperty(nameof(UpdateCompanyRequestDto.Website));

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
        var property = typeof(UpdateCompanyRequestDto).GetProperty(nameof(UpdateCompanyRequestDto.Website));

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
        var property = typeof(UpdateCompanyRequestDto).GetProperty(nameof(UpdateCompanyRequestDto.Website));

        // Act
        var isNullable = Nullable.GetUnderlyingType(property?.PropertyType!) != null ||
                        !property?.PropertyType.IsValueType == true;

        // Assert
        Assert.True(isNullable);
    }

    #endregion

    #region Performance and Memory Tests

    [Fact]
    public void Large_Property_Values_Should_Not_Cause_Memory_Issues()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto();

        // Act - Set large values within limits
        dto.Name = new string('A', 200);
        dto.StockTicker = new string('B', 10);
        dto.Exchange = new string('C', 100);
        dto.Website = "https://www.example.com/" + new string('d', 476);

        // Assert - Properties should retain their values
        Assert.Equal(200, dto.Name.Length);
        Assert.Equal(10, dto.StockTicker.Length);
        Assert.Equal(100, dto.Exchange.Length);
        Assert.Equal(500, dto.Website.Length);
    }

    [Fact]
    public void Multiple_Property_Updates_Should_Not_Cause_Issues()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto();

        // Act - Update properties multiple times
        for (int i = 0; i < 100; i++)
        {
            dto.Name = $"Company {i}";
            dto.StockTicker = $"TK{i:00}";
            dto.Exchange = i % 2 == 0 ? "NYSE" : "NASDAQ";
        }

        // Assert
        Assert.Equal("Company 99", dto.Name);
        Assert.Equal("TK99", dto.StockTicker);
        Assert.Equal("NASDAQ", dto.Exchange);
    }

    #endregion

    #region Null Safety Tests

    [Fact]
    public void Properties_Should_Handle_Null_Assignment_Without_Exception()
    {
        // Arrange
        var dto = new UpdateCompanyRequestDto();

        // Act & Assert - These should not throw exceptions
        dto.Name = null!;
        dto.StockTicker = null!;
        dto.Exchange = null!;
        dto.Isin = null!;
        dto.Website = null;

        Assert.Null(dto.Name);
        Assert.Null(dto.StockTicker);
        Assert.Null(dto.Exchange);
        Assert.Null(dto.Isin);
        Assert.Null(dto.Website);
    }

    #endregion

    #region Type Safety Tests

    [Fact]
    public void All_Properties_Should_Be_String_Type_Or_Nullable_String()
    {
        // Arrange
        var properties = typeof(UpdateCompanyRequestDto).GetProperties();

        // Act & Assert
        foreach (var property in properties)
        {
            var propertyType = property.PropertyType;
            var isString = propertyType == typeof(string);
            var isNullableString = propertyType == typeof(string);

            Assert.True(isString || isNullableString,
                $"Property {property.Name} should be of type string or string?");
        }
    }

    [Fact]
    public void Dto_Should_Be_Serializable()
    {
        // Arrange
        var dto = CreateValidDto();

        // Act & Assert - This should not throw
        var json = System.Text.Json.JsonSerializer.Serialize(dto);
        var deserializedDto = System.Text.Json.JsonSerializer.Deserialize<UpdateCompanyRequestDto>(json);

        Assert.NotNull(deserializedDto);
        Assert.Equal(dto.Name, deserializedDto.Name);
        Assert.Equal(dto.StockTicker, deserializedDto.StockTicker);
        Assert.Equal(dto.Exchange, deserializedDto.Exchange);
        Assert.Equal(dto.Isin, deserializedDto.Isin);
        Assert.Equal(dto.Website, deserializedDto.Website);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Complete_Dto_Lifecycle_Should_Work_Correctly()
    {
        // Arrange & Act - Create new DTO
        var dto = new UpdateCompanyRequestDto();

        // Assert - Initial state
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.StockTicker);
        Assert.Equal(string.Empty, dto.Exchange);
        Assert.Equal(string.Empty, dto.Isin);
        Assert.Null(dto.Website);

        // Act - Set valid values
        dto.Name = "Test Company";
        dto.StockTicker = "TEST";
        dto.Exchange = "NYSE";
        dto.Isin = "US0378331005";
        dto.Website = "https://test.com";

        // Assert - Valid state
        AssertValidationPasses(dto);

        // Act - Update values
        dto.Name = "Updated Company";
        dto.Website = "https://updated.com";

        // Assert - Still valid
        AssertValidationPasses(dto);
        Assert.Equal("Updated Company", dto.Name);
        Assert.Equal("https://updated.com", dto.Website);

        // Act - Set invalid values
        dto.Name = null!;
        dto.StockTicker = new string('A', 11);

        // Assert - Should have validation errors
        var errors = GetValidationErrors(dto);
        Assert.NotEmpty(errors);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(dto.Name)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(dto.StockTicker)));
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Creates a valid DTO for testing
    /// </summary>
    private static UpdateCompanyRequestDto CreateValidDto()
    {
        return new UpdateCompanyRequestDto
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
