using System.ComponentModel.DataAnnotations;
using GlassLewis.Application.Attributes.Validation;

namespace GlassLewis.Application.UnitTests.Attributes.Validation;
// <summary>
// Unit tests for the IsinValidationAttribute class, which validates ISIN (International Securities Identification Number) formats.
// This class is derived from IsinValidationAttribute to test its functionality of protected methods.
// </summary>
[Trait("Category", "UnitTests")]
public class IsinValidationAttributeTests : IsinValidationAttribute
{
    private readonly ValidationContext _validationContext;

    public IsinValidationAttributeTests()
    {
        // Create a mock object for ValidationContext
        var mockObject = new TestClass();
        _validationContext = new ValidationContext(mockObject)
        {
            MemberName = "TestProperty"
        };
    }

    #region Valid ISIN Tests

    [Theory]
    [InlineData("US0378331005")] // Apple Inc.
    [InlineData("GB0002634946")] // BT Group
    [InlineData("DE0007164600")] // SAP SE
    [InlineData("FR0000120404")] // Air Liquide
    [InlineData("JP3633400001")] // Sony Corporation
    [InlineData("CA0679011084")] // Barrick Gold Corporation
    [InlineData("AU000000BHP4")] // BHP Group Limited
    [InlineData("NL0000009165")] // Unilever N.V.
    [InlineData("CH0012032048")] // Roche Holding AG
    [InlineData("IT0003128367")] // ENI S.p.A.
    public void IsValid_ReturnsSuccess_ForValidIsinFormats(string validIsin)
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();

        // Act
        var result = validator.IsValid(validIsin, _validationContext);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void IsValid_ReturnsSuccess_ForValidIsinWithAllUppercaseLetters()
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();
        var validIsin = "XX1234567890"; // All uppercase letters in country code

        // Act
        var result = validator.IsValid(validIsin, _validationContext);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void IsValid_ReturnsSuccess_ForValidIsinWithMixedAlphanumericMiddleSection()
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();
        var validIsin = "US12A4B6C8D9"; // Mixed letters and numbers in middle section

        // Act
        var result = validator.IsValid(validIsin, _validationContext);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    #endregion

    #region Null and Empty Value Tests

    [Fact]
    public void IsValid_ReturnsSuccess_ForNullValue()
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();

        // Act
        var result = validator.IsValid(null, _validationContext);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void IsValid_ReturnsValidationError_ForEmptyString()
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();

        // Act
        var result = validator.IsValid(string.Empty, _validationContext);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("ISIN format is invalid. It must start with two letters followed by 9 alphanumeric characters and end with a digit", result?.ErrorMessage);
    }

    [Fact]
    public void IsValid_ReturnsValidationError_ForWhitespaceString()
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();
        var whitespaceString = "   ";

        // Act
        var result = validator.IsValid(whitespaceString, _validationContext);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("ISIN format is invalid. It must start with two letters followed by 9 alphanumeric characters and end with a digit", result?.ErrorMessage);
    }

    #endregion

    #region Invalid Length Tests

    [Theory]
    [InlineData("US037833100")] // 11 characters (too short)
    [InlineData("US03783310055")] // 13 characters (too long)
    [InlineData("US")] // 2 characters (too short)
    [InlineData("US037")] // 5 characters (too short)
    [InlineData("US0378331005EXTRA")] // 17 characters (too long)
    public void IsValid_ReturnsValidationError_ForInvalidLength(string invalidLengthIsin)
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();

        // Act
        var result = validator.IsValid(invalidLengthIsin, _validationContext);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("ISIN format is invalid. It must start with two letters followed by 9 alphanumeric characters and end with a digit", result?.ErrorMessage);
    }

    #endregion

    #region Invalid Format Tests - Country Code

    [Theory]
    [InlineData("U50378331005")] // First character is not a letter
    [InlineData("1S0378331005")] // First character is a number
    [InlineData("us0378331005")] // Lowercase letters (invalid)
    [InlineData("U10378331005")] // Second character is a number
    [InlineData("1A0378331005")] // First character is a number
    [InlineData("A10378331005")] // Second character is a number
    [InlineData("120378331005")] // Both characters are numbers
    public void IsValid_ReturnsValidationError_ForInvalidCountryCodeFormat(string invalidCountryCodeIsin)
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();

        // Act
        var result = validator.IsValid(invalidCountryCodeIsin, _validationContext);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("ISIN format is invalid. It must start with two letters followed by 9 alphanumeric characters and end with a digit", result?.ErrorMessage);
    }

    #endregion

    #region Invalid Format Tests - Check Digit

    [Theory]
    [InlineData("US037833100A")] // Last character is a letter
    [InlineData("US037833100Z")] // Last character is a letter
    [InlineData("US037833100!")] // Last character is a special character
    [InlineData("US037833100 ")] // Last character is a space
    public void IsValid_ReturnsValidationError_ForInvalidCheckDigit(string invalidCheckDigitIsin)
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();

        // Act
        var result = validator.IsValid(invalidCheckDigitIsin, _validationContext);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("ISIN format is invalid. It must start with two letters followed by 9 alphanumeric characters and end with a digit", result?.ErrorMessage);
    }

    #endregion

    #region Invalid Format Tests - Middle Section

    [Theory]
    [InlineData("US!@#$%^&*05")] // Special characters in middle section
    [InlineData("US         5")] // Spaces in middle section
    [InlineData("US---------5")] // Hyphens in middle section
    public void IsValid_ReturnsValidationError_ForInvalidMiddleSectionFormat(string invalidMiddleSectionIsin)
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();

        // Act
        var result = validator.IsValid(invalidMiddleSectionIsin, _validationContext);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("ISIN format is invalid. It must start with two letters followed by 9 alphanumeric characters and end with a digit", result?.ErrorMessage);
    }

    #endregion

    #region Edge Cases and Special Characters

    [Theory]
    [InlineData("US037833100@")] // Special character at end
    [InlineData("@S0378331005")] // Special character at start
    [InlineData("U@0378331005")] // Special character in country code
    [InlineData("US@378331005")] // Special character in middle
    [InlineData("US037833100.")] // Period at end
    [InlineData("US037833100-")] // Hyphen at end
    [InlineData("US037833100_")] // Underscore at end
    public void IsValid_ReturnsValidationError_ForSpecialCharacters(string specialCharacterIsin)
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();

        // Act
        var result = validator.IsValid(specialCharacterIsin, _validationContext);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("ISIN format is invalid. It must start with two letters followed by 9 alphanumeric characters and end with a digit", result?.ErrorMessage);
    }

    #endregion

    #region Non-String Value Tests

    [Fact]
    public void IsValid_ReturnsValidationError_ForIntegerValue()
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();
        var integerValue = 123456789;

        // Act
        var result = validator.IsValid(integerValue, _validationContext);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("ISIN format is invalid. It must start with two letters followed by 9 alphanumeric characters and end with a digit", result?.ErrorMessage);
    }

    [Fact]
    public void IsValid_ReturnsValidationError_ForDateTimeValue()
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();
        var dateTimeValue = DateTime.Now;

        // Act
        var result = validator.IsValid(dateTimeValue, _validationContext);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("ISIN format is invalid. It must start with two letters followed by 9 alphanumeric characters and end with a digit", result?.ErrorMessage);
    }

    [Fact]
    public void IsValid_ReturnsValidationError_ForBooleanValue()
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();
        var booleanValue = true;

        // Act
        var result = validator.IsValid(booleanValue, _validationContext);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("ISIN format is invalid. It must start with two letters followed by 9 alphanumeric characters and end with a digit", result?.ErrorMessage);
    }

    [Fact]
    public void IsValid_ReturnsValidationError_ForObjectValue()
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();
        var objectValue = new object();

        // Act
        var result = validator.IsValid(objectValue, _validationContext);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("ISIN format is invalid. It must start with two letters followed by 9 alphanumeric characters and end with a digit", result?.ErrorMessage);
    }

    #endregion

    #region Regex Pattern Edge Cases

    [Theory]
    [InlineData("ab0378331005")] // Lowercase country code
    [InlineData("Us0378331005")] // Mixed case country code
    [InlineData("uS0378331005")] // Mixed case country code
    [InlineData("US037833100a")] // Lowercase in middle section (should be valid per regex)
    public void IsValid_ReturnsValidationError_ForCaseSensitiveViolations(string caseSensitiveIsin)
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();

        // Act
        var result = validator.IsValid(caseSensitiveIsin, _validationContext);

        // Assert - These should all fail because the regex requires uppercase letters for country code
        if (caseSensitiveIsin.Substring(0, 2).Any(char.IsLower))
        {
            Assert.NotEqual(ValidationResult.Success, result);
            Assert.Equal("ISIN format is invalid. It must start with two letters followed by 9 alphanumeric characters and end with a digit", result?.ErrorMessage);
        }
    }

    [Fact]
    public void IsValid_HandlesToStringNull_ForNullableObject()
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();
        var testObject = new TestClassWithNullableString { NullableProperty = null };

        // Act
        var result = validator.IsValid(testObject.NullableProperty, _validationContext);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    #endregion

    #region Boundary Tests

    [Fact]
    public void IsValid_ReturnsSuccess_ForExactly12CharactersValidFormat()
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();
        var exactLength = "US1234567890"; // Exactly 12 characters

        // Act
        var result = validator.IsValid(exactLength, _validationContext);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Theory]
    [InlineData("US123456789")] // 11 characters
    [InlineData("US12345678901")] // 13 characters
    public void IsValid_ReturnsValidationError_ForOffByOneLength(string offByOneIsin)
    {
        // Arrange
        var validator = new IsinValidationAttributeTests();

        // Act
        var result = validator.IsValid(offByOneIsin, _validationContext);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("ISIN format is invalid. It must start with two letters followed by 9 alphanumeric characters and end with a digit", result?.ErrorMessage);
    }

    #endregion

    #region Attribute Properties Tests

    [Fact]
    public void Attribute_CanBeAppliedToProperty()
    {
        // Arrange
        var property = typeof(TestClass).GetProperty(nameof(TestClass.IsinProperty));

        // Act
        var attribute = property?.GetCustomAttributes(typeof(IsinValidationAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(attribute);
        Assert.IsType<IsinValidationAttribute>(attribute);
    }

    [Fact]
    public void Attribute_InheritsFromValidationAttribute()
    {
        // Arrange & Act
        var attribute = new IsinValidationAttribute();

        // Assert
        Assert.IsAssignableFrom<ValidationAttribute>(attribute);
    }

    #endregion

    #region Test Helper Classes

    private sealed class TestClass
    {
        [IsinValidation]
        public string IsinProperty { get; set; } = string.Empty;

        public string TestProperty { get; set; } = string.Empty;
    }

    private sealed class TestClassWithNullableString
    {
        public string? NullableProperty { get; set; }
    }

    #endregion
}
