using GlassLewis.Api.SystemTests.Fixtures;

namespace GlassLewis.Api.SystemTests.Controller.v1.Company;

/// <summary>
/// Provides a suite of tests to verify the behavior of API endpoints when accessed without authentication.
/// </summary>
/// <remarks>This test class is designed to ensure that unauthenticated requests to protected API endpoints
/// consistently return HTTP 401 Unauthorized responses. It covers various HTTP methods, including GET, POST, PUT,
/// PATCH, and DELETE, for endpoints related to company resources.</remarks>
/// <param name="fixture"></param>
[Trait("Category", "SystemTests")]
[Collection("Configuration")]
public class UnauthenticatedApiTests(ConfigurationFixture fixture)
{
    [Fact]
    public async Task GET_Companies_List_Without_Auth_Should_Return_401()
    {
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Get, fixture.CompanyEndpoint, false);

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GET_Company_By_Id_List_Without_Auth_Should_Return_401()
    {
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Get, $"{fixture.CompanyEndpoint}/{Guid.NewGuid()}", false);

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GET_Company_By_Isin_List_Without_Auth_Should_Return_401()
    {
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Get, $"{fixture.CompanyEndpoint}/isin/AB0000000000", false);

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task POST_Company_Without_Auth_Should_Return_401()
    {
        var company = new { Name = "Test Company", Exchange = "NYSE", Ticker = "TEST" };
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Post, fixture.CompanyEndpoint, false, company);

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PUT_Company_Without_Auth_Should_Return_401()
    {
        var company = new { Id = 1, Name = "Updated Company", Exchange = "NYSE", Ticker = "UPD" };
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Put, $"{fixture.CompanyEndpoint}/{Guid.NewGuid()}", false, company);

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PATCH_Company_Without_Auth_Should_Return_401()
    {
        var patch = new { Name = "Patched Company" };
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Patch, $"{fixture.CompanyEndpoint}/{Guid.NewGuid()}", false, patch);

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Company_Without_Auth_Should_Return_401()
    {
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Get, $"{fixture.CompanyEndpoint}/{Guid.NewGuid()}", false);

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
