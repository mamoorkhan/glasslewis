using System.Text.Json;
using GlassLewis.Api.SystemTests.Fixtures;
using GlassLewis.Api.SystemTests.TestCaseOrdering;
using GlassLewis.Api.SystemTests.Utilities;

namespace GlassLewis.Api.SystemTests.Controller.v1.Company;

/// <summary>
/// Provides a suite of tests for API endpoints related to companies, ensuring proper functionality when authenticated
/// and using valid payloads.
/// </summary>
/// <remarks>This test class is designed to validate the behavior of company-related API endpoints under
/// authenticated conditions. It includes tests for creating, retrieving, updating, and deleting companies using valid
/// payloads. Each test is executed in a specific order to ensure proper setup and teardown of test data.  Traits
/// applied to this class categorize it as API tests, specifically focusing on authenticated scenarios with valid
/// payloads.</remarks>
/// <param name="fixture"></param>
[Trait("Category", "SystemTests")]
[Collection("Configuration")]
[TestCaseOrderer("GlassLewis.Api.SystemTests.TestCaseOrdering.TestCaseOrderer", "GlassLewis.Api.SystemTests")]
public class AuthenticatedValidPayloadTests(ConfigurationFixture fixture)
{
    [Fact]
    [TestPriority(1)]
    public async Task Setup_Authentication()
    {
        var accessToken = await fixture.GetToken();
        Assert.NotNull(accessToken);
    }

    [Fact]
    [TestPriority(2)]
    public async Task GET_Companies_With_Valid_Auth_Should_Return_200()
    {
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Get, fixture.CompanyEndpoint);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }

    [Fact]
    [TestPriority(3)]
    public async Task POST_Company_With_Valid_Payload_Should_Create_Company()
    {
        var company = new
        {
            Name = "Test Company Inc.",
            Exchange = "NYSE",
            StockTicker = "TEST",
            Isin = IsinGenerator.GenerateIsin(),
            Website = "https://testcompany.com"
        };

        var response = await fixture.SendHttpRequestAsync(HttpMethod.Post, fixture.CompanyEndpoint, payload: company);

        Assert.True(response.IsSuccessStatusCode,
            $"Expected success but got {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");

        var responseContent = await response.Content.ReadAsStringAsync();
        var createdCompany = JsonSerializer.Deserialize<JsonElement>(responseContent);

        if (createdCompany.TryGetProperty("id", out var idProperty))
        {
            fixture.CreatedCompanyId = idProperty.GetGuid();
        }

        if (createdCompany.TryGetProperty("isin", out var isinProperty))
        {
            fixture.CreatedCompanyIsin = isinProperty.GetString();
        }

        Assert.True(fixture.CreatedCompanyId != Guid.Empty, "Created company should have a valid ID");
    }

    [Fact]
    [TestPriority(4)]
    public async Task GET_Company_By_Id_Should_Return_Created_Company()
    {
        if (fixture.CreatedCompanyId == Guid.Empty) await POST_Company_With_Valid_Payload_Should_Create_Company();

        var response = await fixture.SendHttpRequestAsync(HttpMethod.Get, $"{fixture.CompanyEndpoint}/{fixture.CreatedCompanyId}");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var company = JsonSerializer.Deserialize<JsonElement>(content);

        Assert.True(company.TryGetProperty("name", out var nameProperty));
        Assert.Equal("Test Company Inc.", nameProperty.GetString());
    }

    [Fact]
    [TestPriority(5)]
    public async Task GET_Company_By_Isin_Should_Return_Created_Company()
    {
        if (string.IsNullOrEmpty(fixture.CreatedCompanyIsin)) await POST_Company_With_Valid_Payload_Should_Create_Company();

        var response = await fixture.SendHttpRequestAsync(HttpMethod.Get, $"{fixture.CompanyEndpoint}/isin/{fixture.CreatedCompanyIsin}");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var company = JsonSerializer.Deserialize<JsonElement>(content);

        Assert.True(company.TryGetProperty("name", out var nameProperty));
        Assert.Equal("Test Company Inc.", nameProperty.GetString());
    }

    [Fact]
    [TestPriority(6)]
    public async Task PUT_Company_With_Valid_Payload_Should_Update_Company()
    {
        if (fixture.CreatedCompanyId == Guid.Empty) await POST_Company_With_Valid_Payload_Should_Create_Company();

        var updatedCompany = new
        {
            Id = fixture.CreatedCompanyId,
            Name = "Updated Test Company Inc.",
            Exchange = "NASDAQ",
            StockTicker = "UPDT",
            Isin = "US1234567890",
            Website = "https://updatedtestcompany.com"
        };

        var response = await fixture.SendHttpRequestAsync(HttpMethod.Put, $"{fixture.CompanyEndpoint}/{fixture.CreatedCompanyId}", payload: updatedCompany);
        Assert.True(response.IsSuccessStatusCode,
            $"Expected success but got {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    [TestPriority(7)]
    public async Task PATCH_Company_With_Valid_Payload_Should_Partially_Update_Company()
    {
        if (fixture.CreatedCompanyId == Guid.Empty) await POST_Company_With_Valid_Payload_Should_Create_Company();

        var patchData = new { Name = "Patched Company Name" };

        var response = await fixture.SendHttpRequestAsync(HttpMethod.Patch, $"{fixture.CompanyEndpoint}/{fixture.CreatedCompanyId}", payload: patchData);
        Assert.True(response.IsSuccessStatusCode,
            $"Expected success but got {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    [TestPriority(8)]
    public async Task DELETE_Company_Should_Remove_Company()
    {
        if (fixture.CreatedCompanyId == Guid.Empty) await POST_Company_With_Valid_Payload_Should_Create_Company();


        var response = await fixture.SendHttpRequestAsync(HttpMethod.Delete, $"{fixture.CompanyEndpoint}/{fixture.CreatedCompanyId}");
        Assert.True(response.IsSuccessStatusCode,
            $"Expected success but got {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");

        // Verify deletion by trying to get the company
        var getResponse = await fixture.SendHttpRequestAsync(HttpMethod.Get, $"{fixture.CompanyEndpoint}/{fixture.CreatedCompanyId}");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
