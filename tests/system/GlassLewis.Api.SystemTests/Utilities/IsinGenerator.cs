namespace GlassLewis.Api.SystemTests.Utilities;

/// <summary>
/// Generates unique ISINs for testing purposes
/// ISIN Format: [2 Letters][9 Alphanumeric][1 Check Digit]
/// Example: US0378331005, GB0002634946
/// </summary>
public static class IsinGenerator
{
    private static readonly Random _random = new Random();

    // Character sets
    private const string CountryCodes = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Alphanumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const string Digits = "0123456789";

    public static string GenerateIsin()
    {
        string isin;

        // Generate country code (first 2 letters)
        var country = GenerateRandomCountryCode();

        // Generate 9 alphanumeric characters
        var securityCode = GenerateSecurityCode();

        // Calculate check digit using Luhn algorithm
        var checkDigit = GenerateCheckDigit();

        isin = country + securityCode + checkDigit;

        return isin;
    }

    #region Helper Methods

    private static string GenerateRandomCountryCode(Random? random = null)
    {
        random ??= _random;
        return new string(
        [
            CountryCodes[random.Next(CountryCodes.Length - 1)],
            CountryCodes[random.Next(CountryCodes.Length - 1)]
        ]);
    }

    private static string GenerateSecurityCode(Random? random = null)
    {
        random ??= _random;
        return new string([.. Enumerable.Range(0, 8).Select(_ => Alphanumeric[random.Next(Alphanumeric.Length - 1)])]);
    }

    private static int GenerateCheckDigit(Random? random = null)
    {
        random ??= _random;
        return Digits[random.Next(Digits.Length - 1)];
    }

    #endregion
}
