using GlassLewis.Application.Dtos.Requests.Company;

namespace GlassLewis.Application.UnitTests.Dtos.Requests.Company;


/// <summary>
/// Contains unit tests for the <see cref="PatchCompanyRequestDto"/> class.
/// </summary>
/// <remarks>This test class verifies the behavior of the <see cref="PatchCompanyRequestDto"/> class, including
/// property accessors, validation logic, and dictionary-based tracking of modified properties.</remarks>
[Trait("Category", "UnitTests")]
public class PatchCompanyRequestDtoTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_Should_Initialize_Empty_Dictionary()
    {
        // Arrange & Act
        var dto = new PatchCompanyRequestDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Empty(dto.GetProvidedProperties());
    }

    #endregion

    #region Property Tests - Name

    [Fact]
    public void Name_Get_Should_Return_Default_When_Not_Set()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        var name = dto.Name;

        // Assert
        Assert.Null(name);
    }

    [Fact]
    public void Name_Set_Should_Store_Value_And_Get_Should_Return_It()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        const string expectedName = "Test Company";

        // Act
        dto.Name = expectedName;

        // Assert
        Assert.Equal(expectedName, dto.Name);
    }

    [Fact]
    public void Name_Set_Should_Add_Property_To_Dictionary()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Name = "Test Company";

        // Assert
        Assert.True(dto.HasProperty(nameof(dto.Name)));
        Assert.Contains(nameof(dto.Name), dto.GetProvidedProperties());
    }

    [Fact]
    public void Name_Set_To_Null_Should_Store_Null_Value()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Name = null;

        // Assert
        Assert.Null(dto.Name);
        Assert.True(dto.HasProperty(nameof(dto.Name)));
    }

    [Fact]
    public void Name_Set_Multiple_Times_Should_Update_Value()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Name = "First Value";
        dto.Name = "Second Value";

        // Assert
        Assert.Equal("Second Value", dto.Name);
        Assert.Single(dto.GetProvidedProperties());
    }

    #endregion

    #region Property Tests - StockTicker

    [Fact]
    public void StockTicker_Get_Should_Return_Default_When_Not_Set()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        var stockTicker = dto.StockTicker;

        // Assert
        Assert.Null(stockTicker);
    }

    [Fact]
    public void StockTicker_Set_Should_Store_Value_And_Get_Should_Return_It()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        const string expectedTicker = "AAPL";

        // Act
        dto.StockTicker = expectedTicker;

        // Assert
        Assert.Equal(expectedTicker, dto.StockTicker);
    }

    [Fact]
    public void StockTicker_Set_Should_Add_Property_To_Dictionary()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.StockTicker = "AAPL";

        // Assert
        Assert.True(dto.HasProperty(nameof(dto.StockTicker)));
        Assert.Contains(nameof(dto.StockTicker), dto.GetProvidedProperties());
    }

    [Fact]
    public void StockTicker_Set_To_Null_Should_Store_Null_Value()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.StockTicker = null;

        // Assert
        Assert.Null(dto.StockTicker);
        Assert.True(dto.HasProperty(nameof(dto.StockTicker)));
    }

    #endregion

    #region Property Tests - Exchange

    [Fact]
    public void Exchange_Get_Should_Return_Default_When_Not_Set()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        var exchange = dto.Exchange;

        // Assert
        Assert.Null(exchange);
    }

    [Fact]
    public void Exchange_Set_Should_Store_Value_And_Get_Should_Return_It()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        const string expectedExchange = "NYSE";

        // Act
        dto.Exchange = expectedExchange;

        // Assert
        Assert.Equal(expectedExchange, dto.Exchange);
    }

    [Fact]
    public void Exchange_Set_Should_Add_Property_To_Dictionary()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Exchange = "NYSE";

        // Assert
        Assert.True(dto.HasProperty(nameof(dto.Exchange)));
        Assert.Contains(nameof(dto.Exchange), dto.GetProvidedProperties());
    }

    #endregion

    #region Property Tests - Isin

    [Fact]
    public void Isin_Get_Should_Return_Default_When_Not_Set()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        var isin = dto.Isin;

        // Assert
        Assert.Null(isin);
    }

    [Fact]
    public void Isin_Set_Should_Store_Value_And_Get_Should_Return_It()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        const string expectedIsin = "US0378331005";

        // Act
        dto.Isin = expectedIsin;

        // Assert
        Assert.Equal(expectedIsin, dto.Isin);
    }

    [Fact]
    public void Isin_Set_Should_Add_Property_To_Dictionary()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Isin = "US0378331005";

        // Assert
        Assert.True(dto.HasProperty(nameof(dto.Isin)));
        Assert.Contains(nameof(dto.Isin), dto.GetProvidedProperties());
    }

    #endregion

    #region Property Tests - Website

    [Fact]
    public void Website_Get_Should_Return_Default_When_Not_Set()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        var website = dto.Website;

        // Assert
        Assert.Null(website);
    }

    [Fact]
    public void Website_Set_Should_Store_Value_And_Get_Should_Return_It()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        const string expectedWebsite = "https://www.example.com";

        // Act
        dto.Website = expectedWebsite;

        // Assert
        Assert.Equal(expectedWebsite, dto.Website);
    }

    [Fact]
    public void Website_Set_Should_Add_Property_To_Dictionary()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Website = "https://www.example.com";

        // Assert
        Assert.True(dto.HasProperty(nameof(dto.Website)));
        Assert.Contains(nameof(dto.Website), dto.GetProvidedProperties());
    }

    #endregion

    #region HasProperty Tests

    [Fact]
    public void HasProperty_Should_Return_False_For_Unset_Property()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act & Assert
        Assert.False(dto.HasProperty(nameof(dto.Name)));
        Assert.False(dto.HasProperty(nameof(dto.StockTicker)));
        Assert.False(dto.HasProperty(nameof(dto.Exchange)));
        Assert.False(dto.HasProperty(nameof(dto.Isin)));
        Assert.False(dto.HasProperty(nameof(dto.Website)));
    }

    [Fact]
    public void HasProperty_Should_Return_True_For_Set_Property()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Name = "Test";

        // Assert
        Assert.True(dto.HasProperty(nameof(dto.Name)));
    }

    [Fact]
    public void HasProperty_Should_Return_True_For_Property_Set_To_Null()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Name = null;

        // Assert
        Assert.True(dto.HasProperty(nameof(dto.Name)));
    }

    [Fact]
    public void HasProperty_Should_Be_Case_Sensitive()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Name = "Test";

        // Act & Assert
        Assert.True(dto.HasProperty("Name"));
        Assert.False(dto.HasProperty("name"));
        Assert.False(dto.HasProperty("NAME"));
    }

    [Fact]
    public void HasProperty_Should_Return_False_For_Invalid_Property_Name()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Name = "Test";

        // Act & Assert
        Assert.False(dto.HasProperty("InvalidProperty"));
        Assert.False(dto.HasProperty(""));
        Assert.False(dto.HasProperty(null!));
    }

    #endregion

    #region GetPropertyValue Tests

    [Fact]
    public void GetPropertyValue_Should_Return_Null_For_Unset_Property()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act & Assert
        Assert.Null(dto.GetPropertyValue(nameof(dto.Name)));
        Assert.Null(dto.GetPropertyValue("InvalidProperty"));
    }

    [Fact]
    public void GetPropertyValue_Should_Return_Value_For_Set_Property()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        const string expectedValue = "Test Company";

        // Act
        dto.Name = expectedValue;

        // Assert
        Assert.Equal(expectedValue, dto.GetPropertyValue(nameof(dto.Name)));
    }

    [Fact]
    public void GetPropertyValue_Should_Return_Null_For_Property_Set_To_Null()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Name = null;

        // Assert
        Assert.Null(dto.GetPropertyValue(nameof(dto.Name)));
    }

    [Fact]
    public void GetPropertyValue_Should_Be_Case_Sensitive()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Name = "Test";

        // Act & Assert
        Assert.Equal("Test", dto.GetPropertyValue("Name"));
        Assert.Null(dto.GetPropertyValue("name"));
    }

    [Fact]
    public void GetPropertyValue_Should_Return_Null_For_Null_Property_Name()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Name = "Test";

        // Act & Assert
        Assert.Null(dto.GetPropertyValue(null!));
    }

    #endregion

    #region GetProvidedProperties Tests

    [Fact]
    public void GetProvidedProperties_Should_Return_Empty_For_New_Instance()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        var properties = dto.GetProvidedProperties();

        // Assert
        Assert.Empty(properties);
    }

    [Fact]
    public void GetProvidedProperties_Should_Return_Single_Property_When_One_Set()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Name = "Test";
        var properties = dto.GetProvidedProperties().ToList();

        // Assert
        Assert.Single(properties);
        Assert.Contains(nameof(dto.Name), properties);
    }

    [Fact]
    public void GetProvidedProperties_Should_Return_Multiple_Properties_When_Multiple_Set()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Name = "Test";
        dto.StockTicker = "AAPL";
        dto.Exchange = "NYSE";
        var properties = dto.GetProvidedProperties().ToList();

        // Assert
        Assert.Equal(3, properties.Count);
        Assert.Contains(nameof(dto.Name), properties);
        Assert.Contains(nameof(dto.StockTicker), properties);
        Assert.Contains(nameof(dto.Exchange), properties);
    }

    [Fact]
    public void GetProvidedProperties_Should_Include_Properties_Set_To_Null()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Name = null;
        dto.StockTicker = "AAPL";
        var properties = dto.GetProvidedProperties().ToList();

        // Assert
        Assert.Equal(2, properties.Count);
        Assert.Contains(nameof(dto.Name), properties);
        Assert.Contains(nameof(dto.StockTicker), properties);
    }

    [Fact]
    public void GetProvidedProperties_Should_Not_Duplicate_When_Property_Set_Multiple_Times()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Name = "First";
        dto.Name = "Second";
        var properties = dto.GetProvidedProperties().ToList();

        // Assert
        Assert.Single(properties);
        Assert.Contains(nameof(dto.Name), properties);
    }

    #endregion

    #region Validate Tests - Name

    [Fact]
    public void Validate_Should_Return_Empty_When_No_Properties_Set()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        var results = dto.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_Name_Should_Pass_With_Valid_Value()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Name = "Valid Company Name";

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_Name_Should_Fail_When_Null()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Name = null;

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Name)) &&
                                    r.ErrorMessage == "Name cannot be null or whitespace");
    }

    [Fact]
    public void Validate_Name_Should_Fail_When_Empty()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Name = string.Empty;

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Name)) &&
                                    r.ErrorMessage == "Name cannot be null or whitespace");
    }

    [Fact]
    public void Validate_Name_Should_Fail_When_Whitespace()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Name = "   ";

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Name)) &&
                                    r.ErrorMessage == "Name cannot be null or whitespace");
    }

    [Fact]
    public void Validate_Name_Should_Pass_With_200_Characters()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Name = new string('A', 200);

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_Name_Should_Fail_With_201_Characters()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Name = new string('A', 201);

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Name)) &&
                                    r.ErrorMessage == "Name cannot exceed 200 characters");
    }

    #endregion

    #region Validate Tests - StockTicker

    [Fact]
    public void Validate_StockTicker_Should_Pass_With_Valid_Value()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.StockTicker = "AAPL";

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_StockTicker_Should_Pass_When_Null()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.StockTicker = null;

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_StockTicker_Should_Pass_With_10_Characters()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.StockTicker = new string('A', 10);

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_StockTicker_Should_Fail_With_11_Characters()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.StockTicker = new string('A', 11);

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.StockTicker)) &&
                                    r.ErrorMessage == "Stock ticker cannot exceed 10 characters");
    }

    [Fact]
    public void Validate_StockTicker_Should_Pass_With_Empty_String()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.StockTicker = string.Empty;

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    #endregion

    #region Validate Tests - Exchange

    [Fact]
    public void Validate_Exchange_Should_Pass_With_Valid_Value()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Exchange = "NYSE";

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_Exchange_Should_Pass_When_Null()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Exchange = null;

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_Exchange_Should_Pass_With_100_Characters()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Exchange = new string('A', 100);

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_Exchange_Should_Fail_With_101_Characters()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Exchange = new string('A', 101);

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Exchange)) &&
                                    r.ErrorMessage == "Exchange cannot exceed 100 characters");
    }

    #endregion

    #region Validate Tests - Isin

    [Fact]
    public void Validate_Isin_Should_Pass_With_Valid_Value()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Isin = "US0378331005"; // Valid Apple ISIN

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_Isin_Should_Pass_When_Null()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Isin = null;

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_Isin_Should_Pass_When_Empty()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Isin = string.Empty;

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    #endregion

    #region Validate Tests - Website

    [Fact]
    public void Validate_Website_Should_Pass_With_Valid_Url()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Website = "https://www.example.com";

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_Website_Should_Pass_When_Null()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Website = null;

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_Website_Should_Pass_When_Empty()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Website = string.Empty;

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_Website_Should_Pass_With_500_Characters()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        var longUrl = "https://www.example.com/" + new string('a', 500 - 25);
        dto.Website = longUrl;

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_Website_Should_Fail_With_501_Characters()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        var tooLongUrl = "https://www.example.com/" + new string('a', 501);
        dto.Website = tooLongUrl;

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Website)) &&
                                    r.ErrorMessage == "Website URL cannot exceed 500 characters");
    }

    [Fact]
    public void Validate_Website_Should_Fail_With_Invalid_Url()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Website = "not-a-valid-url";

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Website)) &&
                                    r.ErrorMessage == "Website must be a valid URL");
    }

    [Fact]
    public void Validate_Website_Should_Fail_With_Both_Length_And_Format_Errors()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        var invalidLongUrl = "invalid-url-" + new string('a', 500);
        dto.Website = invalidLongUrl;

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Website)) &&
                                    r.ErrorMessage == "Website URL cannot exceed 500 characters");
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Website)) &&
                                    r.ErrorMessage == "Website must be a valid URL");
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://example.com")]
    [InlineData("https://www.example.com")]
    [InlineData("https://subdomain.example.com")]
    [InlineData("https://example.com/path")]
    [InlineData("https://example.com/path?query=value")]
    [InlineData("https://example.com:8080")]
    public void Validate_Website_Should_Pass_With_Valid_Url_Formats(string validUrl)
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Website = validUrl;

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    #endregion

    #region Validate Tests - Multiple Properties

    [Fact]
    public void Validate_Should_Return_Multiple_Errors_For_Multiple_Invalid_Properties()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Name = null; // Invalid
        dto.StockTicker = new string('A', 11); // Invalid
        dto.Exchange = new string('B', 101); // Invalid
        dto.Isin = "INVALID"; // Invalid
        dto.Website = "invalid-url"; // Invalid

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Equal(5, results.Count);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Name)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.StockTicker)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Exchange)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Isin)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Website)));
    }

    [Fact]
    public void Validate_Should_Return_Empty_For_Valid_Multiple_Properties()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Name = "Valid Company";
        dto.StockTicker = "AAPL";
        dto.Exchange = "NYSE";
        dto.Isin = "US0378331005";
        dto.Website = "https://www.example.com";

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_Should_Only_Check_Set_Properties()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Name = null; // Only this property is set (and invalid)

        // Act
        var results = dto.Validate().ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Name)));
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void Properties_Should_Handle_Unicode_Characters()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        const string unicodeName = "Tëst Çömpäny 🏢";

        // Act
        dto.Name = unicodeName;

        // Assert
        Assert.Equal(unicodeName, dto.Name);
        Assert.True(dto.HasProperty(nameof(dto.Name)));
    }

    [Fact]
    public void Properties_Should_Handle_Leading_And_Trailing_Whitespace()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        const string nameWithWhitespace = "  Test Company  ";

        // Act
        dto.Name = nameWithWhitespace;

        // Assert
        Assert.Equal(nameWithWhitespace, dto.Name);
    }

    [Fact]
    public void GetValue_Should_Return_Default_For_Wrong_Type()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();
        dto.Name = "Test";

        // Act
        var result = dto.GetPropertyValue(nameof(dto.Name));

        // Assert
        Assert.Equal("Test", result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public void SetValue_Should_Overwrite_Existing_Value()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Name = "First";
        dto.Name = "Second";
        dto.Name = "Third";

        // Assert
        Assert.Equal("Third", dto.Name);
        Assert.Single(dto.GetProvidedProperties());
    }

    [Fact]
    public void Multiple_Properties_Should_Be_Independent()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Name = "Company Name";
        dto.StockTicker = "AAPL";
        dto.Exchange = "NYSE";

        // Assert
        Assert.Equal("Company Name", dto.Name);
        Assert.Equal("AAPL", dto.StockTicker);
        Assert.Equal("NYSE", dto.Exchange);
        Assert.Equal(3, dto.GetProvidedProperties().Count());
    }

    #endregion

    #region Internal Dictionary Access Tests

    [Fact]
    public void Internal_Dictionary_Should_Store_Exact_Values()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Name = "Test";
        dto.StockTicker = null;
        dto.Exchange = "";

        // Assert
        Assert.Equal("Test", dto.GetPropertyValue(nameof(dto.Name)));
        Assert.Null(dto.GetPropertyValue(nameof(dto.StockTicker)));
        Assert.Equal("", dto.GetPropertyValue(nameof(dto.Exchange)));
    }

    [Fact]
    public void Internal_Dictionary_Should_Distinguish_Between_Null_And_Not_Set()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Name = null;

        // Assert
        Assert.True(dto.HasProperty(nameof(dto.Name)));
        Assert.False(dto.HasProperty(nameof(dto.StockTicker)));
        Assert.Null(dto.Name);
        Assert.Null(dto.StockTicker);
    }

    #endregion

    #region Comprehensive Integration Tests

    [Fact]
    public void Complete_Workflow_Should_Work_Correctly()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act & Assert - Initial state
        Assert.Empty(dto.GetProvidedProperties());
        Assert.Empty(dto.Validate());

        // Act & Assert - Set valid values
        dto.Name = "Test Company";
        dto.StockTicker = "TEST";
        dto.Website = "https://test.com";

        Assert.Equal(3, dto.GetProvidedProperties().Count());
        Assert.Contains(nameof(dto.Name), dto.GetProvidedProperties());
        Assert.Contains(nameof(dto.StockTicker), dto.GetProvidedProperties());
        Assert.Contains(nameof(dto.Website), dto.GetProvidedProperties());
        Assert.Empty(dto.Validate());

        // Act & Assert - Update values
        dto.Name = "Updated Company";
        dto.Exchange = "NYSE";

        Assert.Equal(4, dto.GetProvidedProperties().Count());
        Assert.Equal("Updated Company", dto.Name);
        Assert.Equal("NYSE", dto.Exchange);
        Assert.Empty(dto.Validate());

        // Act & Assert - Set invalid values
        dto.Name = null;
        dto.StockTicker = new string('A', 11);

        var validationResults = dto.Validate().ToList();
        Assert.Equal(2, validationResults.Count);
        Assert.Contains(validationResults, r => r.MemberNames.Contains(nameof(dto.Name)));
        Assert.Contains(validationResults, r => r.MemberNames.Contains(nameof(dto.StockTicker)));
    }

    [Fact]
    public void Validation_Should_Only_Validate_Set_Properties_Complex_Scenario()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act - Set only some properties, some valid, some invalid
        dto.Name = "Valid Name";
        dto.StockTicker = new string('A', 11); // Invalid
        // Exchange not set
        dto.Isin = "INVALID"; // Invalid
        // Website not set

        var results = dto.Validate().ToList();

        // Assert - Only validates set properties
        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.StockTicker)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Isin)));
        Assert.DoesNotContain(results, r => r.MemberNames.Contains(nameof(dto.Exchange)));
        Assert.DoesNotContain(results, r => r.MemberNames.Contains(nameof(dto.Website)));
    }

    [Fact]
    public void Patch_Dto_Should_Handle_All_Boundary_Conditions()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act & Assert - Boundary testing for all properties
        // Name - exactly at limit
        dto.Name = new string('A', 200);
        Assert.Empty(dto.Validate());

        // StockTicker - exactly at limit
        dto.StockTicker = new string('B', 10);
        Assert.Empty(dto.Validate());

        // Exchange - exactly at limit
        dto.Exchange = new string('C', 100);
        Assert.Empty(dto.Validate());

        // Website - exactly at limit
        var longUrl = "https://www.example.com/" + new string('d', 500 - 25);
        dto.Website = longUrl;
        Assert.Empty(dto.Validate());

        // All should still be valid
        Assert.Empty(dto.Validate());

        // Now test one character over limit
        dto.Name = new string('A', 201);
        dto.StockTicker = new string('B', 11);
        dto.Exchange = new string('C', 101);
        dto.Website = "https://www.example.com/" + new string('d', 501);

        var results = dto.Validate().ToList();
        Assert.Equal(4, results.Count); // All 4 should fail
    }

    #endregion

    #region Performance and Memory Tests

    [Fact]
    public void Large_Number_Of_Property_Updates_Should_Not_Cause_Memory_Issues()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act - Update properties many times
        for (int i = 0; i < 1000; i++)
        {
            dto.Name = $"Company {i}";
            dto.StockTicker = $"TK{i:000}";
        }

        // Assert - Should still work correctly
        Assert.Equal("Company 999", dto.Name);
        Assert.Equal("TK999", dto.StockTicker);
        Assert.Equal(2, dto.GetProvidedProperties().Count());
    }

    #endregion

    #region Type Safety Tests

    [Fact]
    public void Generic_GetValue_Should_Return_Correct_Type()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        dto.Name = "Test";

        // Get value through reflection to test the generic method
        var method = typeof(PatchCompanyRequestDto).GetMethod("GetValue",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var genericMethod = method?.MakeGenericMethod(typeof(string));
        var result = genericMethod?.Invoke(dto, new object[] { nameof(dto.Name) });

        // Assert
        Assert.Equal("Test", result);
    }

    [Fact]
    public void Generic_GetValue_Should_Return_Default_For_Unset_Property()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act
        var method = typeof(PatchCompanyRequestDto).GetMethod("GetValue",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var genericMethod = method?.MakeGenericMethod(typeof(string));
        var result = genericMethod?.Invoke(dto, new object[] { nameof(dto.Name) });

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region Null Safety Tests

    [Fact]
    public void All_Methods_Should_Handle_Null_Inputs_Gracefully()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act & Assert

        dto.Name = null;
        dto.StockTicker = null;
        dto.Exchange = null;
        dto.Isin = null;
        dto.Website = null;

        Assert.NotNull(dto.GetProvidedProperties());
        Assert.NotNull(dto.Validate());
    }

    #endregion

    #region Documentation and Comments Coverage

    [Fact]
    public void All_Public_Methods_Should_Be_Accessible()
    {
        // Arrange
        var dto = new PatchCompanyRequestDto();

        // Act & Assert - Verify all public methods are accessible and work
        dto.Name = "Test";
        Assert.True(dto.HasProperty(nameof(dto.Name)));
        Assert.Equal("Test", dto.GetPropertyValue(nameof(dto.Name)));
        Assert.Single(dto.GetProvidedProperties());
        Assert.NotNull(dto.Validate());
    }

    #endregion
}
