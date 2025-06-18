using System.Text;
using GlassLewis.Api.SystemTests.Fixtures;

namespace GlassLewis.Api.SystemTests.Controller.v1.Company;

/// <summary>
/// Contains tests for validating API behavior when handling malformed requests.
/// </summary>
/// <remarks>This test class focuses on scenarios where the API receives invalid or improperly formatted input,
/// ensuring that appropriate HTTP status codes are returned. Examples include malformed JSON payloads, empty request
/// bodies, excessively long values, and unsupported content types.</remarks>
/// <param name="fixture"></param>
[Trait("Category", "SystemTests")]
[Collection("Configuration")]
public class MalformedRequestTests(ConfigurationFixture fixture)
{
    [Fact]
    public async Task POST_Company_With_Malformed_JSON_Should_Return_400()
    {
        var payload = "{ invalid json }";

        var response = await fixture.SendHttpRequestAsync(HttpMethod.Post, fixture.CompanyEndpoint, payload: payload);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_Company_With_Empty_Payload_Should_Return_415()
    {
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Post, fixture.CompanyEndpoint);

        Assert.Equal(System.Net.HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }

    [Fact]
    public async Task POST_Company_With_Extremely_Long_Values_Should_Return_400()
    {
        var company = new
        {
            Name = new string('A', 10000),
            Exchange = "NYSE",
            Ticker = "TEST"
        };

        var response = await fixture.SendHttpRequestAsync(HttpMethod.Post, fixture.CompanyEndpoint, payload: company);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Request_With_Invalid_Content_Type_Should_Return_415()
    {
        var content = new StringContent("some data", Encoding.UTF8, "text/plain");

        var response = await fixture.SendHttpRequestAsync(HttpMethod.Post, fixture.CompanyEndpoint, content: content);

        Assert.Equal(System.Net.HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }
}
