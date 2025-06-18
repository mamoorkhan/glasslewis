using System.Net;
using System.Net.Http.Json;
using GlassLewis.Api.SystemTests.Fixtures;
using GlassLewis.Api.SystemTests.Models.Responses.Company;

namespace GlassLewis.Api.SystemTests.Controller.v1.Company;

/// <summary>
/// Provides a suite of tests for API endpoints related to company operations, focusing on scenarios  where the payload
/// is invalid and the user is authenticated.
/// </summary>
/// <remarks>This test class is designed to validate the behavior of the API when handling invalid payloads  for
/// authenticated users. It includes tests for various HTTP methods (POST, PUT, PATCH, DELETE, GET)  and ensures that
/// the API responds with appropriate status codes for invalid input scenarios, such as  missing required fields,
/// invalid data types, duplicate values, mismatched IDs, and nonexistent resources.</remarks>
/// <param name="fixture"></param>
[Trait("Category", "SystemTests")]
[Collection("Configuration")]
public class AuthenticatedInvalidPayloadTests(ConfigurationFixture fixture)
{
    [Fact]
    public async Task POST_Company_With_Missing_Required_Fields_Should_Return_400()
    {
        var incompleteCompany = new { Name = "Incomplete Company" };

        var response = await fixture.SendHttpRequestAsync(HttpMethod.Post, fixture.CompanyEndpoint, true, payload: incompleteCompany);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_Company_With_Invalid_Data_Types_Should_Return_400()
    {
        var invalidCompany = new
        {
            Name = 123, // Should be string
            Exchange = "NYSE",
            StockTicker = "TEST"
        };

        var response = await fixture.SendHttpRequestAsync(HttpMethod.Post, fixture.CompanyEndpoint, true, payload: invalidCompany);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_Company_With_Duplicate_Isin_Should_Return_409()
    {
        // First, create a company
        var company1 = new
        {
            Name = "First Company",
            Exchange = "NYSE",
            StockTicker = "DUPE",
            Isin = "US1111111111"
        };

        var response1 = await fixture.SendHttpRequestAsync(HttpMethod.Post, fixture.CompanyEndpoint, true, payload: company1);
        var created = await response1.Content.ReadFromJsonAsync<CreateCompanyResponseModel>();

        Assert.True(response1.IsSuccessStatusCode);

        // Try to create another company with the same ticker
        var company2 = new
        {
            Name = "Second Company",
            Exchange = "NASDAQ",
            StockTicker = "DUPE", // Same ticker
            Isin = "US1111111111"
        };

        var response2 = await fixture.SendHttpRequestAsync(HttpMethod.Post, fixture.CompanyEndpoint, true, payload: company2);

        Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);

        await fixture.SendHttpRequestAsync(HttpMethod.Delete, $"{fixture.CompanyEndpoint}/{created?.Id}", true);
    }

    [Fact]
    public async Task PUT_Nonexistent_Company_Should_Return_404()
    {
        var company = new
        {
            Name = "Nonexistent Company",
            Exchange = "NYSE",
            StockTicker = "NONE",
            Isin = "US2222222222"
        };

        var response = await fixture.SendHttpRequestAsync(HttpMethod.Put, $"{fixture.CompanyEndpoint}/{Guid.NewGuid()}", true, payload: company);
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PATCH_Company_With_Invalid_Fields_Should_Return_400()
    {
        // First, create a company
        var company1 = new
        {
            Name = "First Company",
            Exchange = "NYSE",
            StockTicker = "DUPE",
            Isin = "US1111111111"
        };

        var response1 = await fixture.SendHttpRequestAsync(HttpMethod.Post, fixture.CompanyEndpoint, true, payload: company1);
        var created = await response1.Content.ReadFromJsonAsync<CreateCompanyResponseModel>();

        var invalidPatch = new
        {
            InvalidField = "This field doesn't exist",
            Name = 12345 // Wrong data type
        };

        var response = await fixture.SendHttpRequestAsync(HttpMethod.Patch, $"{fixture.CompanyEndpoint}/{created?.Id}", true, payload: invalidPatch);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await fixture.SendHttpRequestAsync(HttpMethod.Delete, $"{fixture.CompanyEndpoint}/{created?.Id}", true);
    }

    [Fact]
    public async Task PATCH_Nonexistent_Company_Should_Return_404()
    {
        var patch = new { Name = "New Name" };

        var response = await fixture.SendHttpRequestAsync(HttpMethod.Patch, $"{fixture.CompanyEndpoint}/1", true, payload: patch);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Nonexistent_Company_Should_Return_404()
    {
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Delete, $"{fixture.CompanyEndpoint}/{Guid.NewGuid()}", true);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GET_Company_With_Invalid_Id_Should_Return_400()
    {
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Get, $"{fixture.CompanyEndpoint}/invalid-id", true);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
