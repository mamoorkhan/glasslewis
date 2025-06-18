namespace GlassLewis.Api.SystemTests;

public static class TestDataBuilder
{
    public static object ValidCompany(string suffix = "") => new
    {
        Name = $"Test Company {suffix}",
        Exchange = "NYSE",
        StockTicker = $"TEST{suffix}",
        Isin = $"US{DateTime.Now.Ticks % 1000000000:D10}",
        Website = $"https://testcompany{suffix}.com"
    };

    public static object InvalidCompany_MissingFields() => new
    {
        Name = "Incomplete Company"
        // Missing required fields
    };

    public static object InvalidCompany_WrongTypes() => new
    {
        Name = 123, // Should be string
        Exchange = "NYSE",
        StockTicker = "TEST"
    };

    public static object InvalidCompany_TooLong() => new
    {
        Name = new string('A', 10000),
        Exchange = "NYSE",
        StockTicker = "TEST"
    };

    public static object ValidPatch() => new
    {
        Name = "Patched Company Name"
    };

    public static object InvalidPatch() => new
    {
        InvalidField = "This field doesn't exist",
        Name = 12345 // Wrong data type
    };
}
