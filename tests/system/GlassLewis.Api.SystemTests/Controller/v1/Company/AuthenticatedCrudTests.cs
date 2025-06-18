using System.Net;
using System.Net.Http.Json;
using GlassLewis.Api.SystemTests.Fixtures;
using GlassLewis.Api.SystemTests.Models.Responses.Company;
using GlassLewis.Api.SystemTests.TestCaseOrdering;
using GlassLewis.Api.SystemTests.Utilities;

namespace GlassLewis.Api.SystemTests.Controller.v1.Company;

/// <summary>
/// Provides a suite of tests for CRUD operations on the API while authenticated.
/// </summary>
/// <remarks>This test class is designed to validate the behavior of API endpoints for creating, reading,
/// updating,  and deleting resources while using a pre-authenticated HTTP client. It ensures that the API adheres to 
/// expected behavior for valid and invalid inputs, as well as edge cases.  The tests are executed in a specific order
/// to maintain state consistency, such as creating a resource  before attempting to read, update, or delete it. The
/// class uses xUnit attributes for test organization  and prioritization.</remarks>
[Trait("Category", "SystemTests")]
[Collection("Configuration")]
[TestCaseOrderer("GlassLewis.Api.SystemTests.TestCaseOrdering.TestCaseOrderer", "GlassLewis.Api.SystemTests")]
public class AuthenticatedCrudTests(ConfigurationFixture fixture) : IDisposable
{
    [Fact]
    [TestPriority(1)]
    public async Task CREATE_Company_With_Valid_Data_Should_Return_201()
    {
        // Arrange
        var company = TestDataBuilder.ValidCompany("CREATE");

        // Act
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Post, fixture.CompanyEndpoint, payload: company);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<CreateCompanyResponseModel>();

        fixture.CreatedCompanyId = created!.Id;
        Assert.NotEqual(Guid.Empty, fixture.CreatedCompanyId);

        Assert.Contains("CREATE", created.Name);
    }

    [Fact]
    [TestPriority(2)]
    public async Task READ_Company_By_Id_Should_Return_200_With_Correct_Data()
    {
        // Arrange - Ensure we have a company to read
        if (fixture.CreatedCompanyId == Guid.Empty)
        {
            await CREATE_Company_With_Valid_Data_Should_Return_201();
        }

        // Act
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Get, $"{fixture.CompanyEndpoint}/{fixture.CreatedCompanyId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<CreateCompanyResponseModel>();

        fixture.CreatedCompanyId = created!.Id;
        Assert.NotEqual(Guid.Empty, fixture.CreatedCompanyId);
        Assert.NotNull(created.Name);
    }

    [Fact]
    [TestPriority(3)]
    public async Task READ_All_Companies_Should_Return_200_With_Array()
    {
        // Act
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Get, fixture.CompanyEndpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var companies = await response.Content.ReadFromJsonAsync<List<GetCompanyResponseModel>>();

        Assert.NotNull(companies);
        Assert.NotEmpty(companies);
        Assert.True(companies.Count >= 0);
    }

    [Fact]
    [TestPriority(4)]
    public async Task UPDATE_Company_With_PUT_Should_Return_200()
    {
        // Arrange
        if (fixture.CreatedCompanyId == Guid.Empty)
        {
            await CREATE_Company_With_Valid_Data_Should_Return_201();
        }

        var updatedCompany = new
        {
            Name = "Updated Test Company PUT",
            Exchange = "NASDAQ",
            StockTicker = "UPDT",
            Isin = IsinGenerator.GenerateIsin(),
            Website = "https://updatedcompany.com"
        };

        // Act
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Put, $"{fixture.CompanyEndpoint}/{fixture.CreatedCompanyId}", payload: updatedCompany);

        // Assert
        Assert.True(response.IsSuccessStatusCode,
            $"PUT request failed with {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");

        // Verify the update by reading the company
        var getResponse = await fixture.SendHttpRequestAsync(HttpMethod.Get, $"{fixture.CompanyEndpoint}/{fixture.CreatedCompanyId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var company = await getResponse.Content.ReadFromJsonAsync<GetCompanyResponseModel>();

        Assert.NotNull(company);
        Assert.Equal("Updated Test Company PUT", company?.Name);
    }

    [Fact]
    [TestPriority(5)]
    public async Task UPDATE_Company_With_PATCH_Should_Return_200()
    {
        // Arrange
        if (fixture.CreatedCompanyId == Guid.Empty)
        {
            await CREATE_Company_With_Valid_Data_Should_Return_201();
        }

        var patchData = new { Name = "Patched Company Name PATCH" };

        // Act
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Patch, $"{fixture.CompanyEndpoint}/{fixture.CreatedCompanyId}", payload: patchData);

        // Assert
        Assert.True(response.IsSuccessStatusCode,
            $"PATCH request failed with {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");

        // Verify the patch by reading the company
        var getResponse = await fixture.SendHttpRequestAsync(HttpMethod.Get, $"{fixture.CompanyEndpoint}/{fixture.CreatedCompanyId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var company = await getResponse.Content.ReadFromJsonAsync<GetCompanyResponseModel>();

        Assert.NotNull(company);
        Assert.Equal("Patched Company Name PATCH", company?.Name);
    }

    [Fact]
    [TestPriority(6)]
    public async Task DELETE_Company_Should_Return_200_And_Remove_Resource()
    {
        // Arrange
        if (fixture.CreatedCompanyId == Guid.Empty)
        {
            await CREATE_Company_With_Valid_Data_Should_Return_201();
        }

        // Act
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Delete, $"{fixture.CompanyEndpoint}/{fixture.CreatedCompanyId}");

        // Assert
        Assert.True(response.IsSuccessStatusCode,
            $"DELETE request failed with {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");

        // Verify deletion by trying to get the company
        var getResponse = await fixture.SendHttpRequestAsync(HttpMethod.Get, $"{fixture.CompanyEndpoint}/{fixture.CreatedCompanyId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        // Reset for other tests
        fixture.CreatedCompanyId = Guid.Empty;
    }

    #region Validation Tests

    [Fact]
    public async Task UPDATE_Nonexistent_Company_Should_Return_404()
    {
        // Arrange
        var nonexistentId = Guid.NewGuid(); ;
        var company = new
        {
            Id = nonexistentId,
            Name = "Nonexistent Company",
            Exchange = "NYSE",
            StockTicker = "NONE",
            Isin = IsinGenerator.GenerateIsin()
        };

        // Act
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Put, $"{fixture.CompanyEndpoint}/{nonexistentId}", payload: company);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task CREATE_Company_With_Minimum_Required_Fields_Should_Succeed()
    {
        // Arrange
        var minimalCompany = new
        {
            Name = "Minimal Company",
            Exchange = "NYSE",
            StockTicker = "MIN",
            Isin = IsinGenerator.GenerateIsin()
        };

        // Act
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Post, fixture.CompanyEndpoint, payload: minimalCompany);

        // Assert
        Assert.True(response.IsSuccessStatusCode,
            $"Minimal company creation failed: {response.StatusCode}");

        var createdCompany = await response.Content.ReadFromJsonAsync<CreateCompanyResponseModel>();
        Assert.NotNull(createdCompany);
        Assert.NotEqual(Guid.Empty, createdCompany?.Id);
        Assert.Equal("Minimal Company", createdCompany?.Name);

        await fixture.SendHttpRequestAsync(HttpMethod.Delete, $"{fixture.CompanyEndpoint}/{createdCompany?.Id}");
    }

    [Fact]
    public async Task CREATE_Company_With_Maximum_Length_Fields_Should_Succeed()
    {
        // Arrange
        var maxLengthCompany = new
        {
            Name = new string('A', 200),
            Exchange = "NYSE",
            StockTicker = "MAX",
            Isin = IsinGenerator.GenerateIsin(),
            Website = "https://" + new string('a', 240) + ".com"
        };

        // Act
        var response = await fixture.SendHttpRequestAsync(HttpMethod.Post, fixture.CompanyEndpoint, payload: maxLengthCompany);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(response.IsSuccessStatusCode,
            $"Max length company creation failed: {response.StatusCode}");

        var createdCompany = await response.Content.ReadFromJsonAsync<CreateCompanyResponseModel>();
        Assert.NotNull(createdCompany);
        Assert.NotEqual(Guid.Empty, createdCompany?.Id);

        await fixture.SendHttpRequestAsync(HttpMethod.Delete, $"{fixture.CompanyEndpoint}/{createdCompany?.Id}");
    }

    [Fact]
    public async Task DELETE_Already_Deleted_Company_Should_Return_404()
    {
        // Arrange
        var company = TestDataBuilder.ValidCompany("DELTST");
        var createResponse = await fixture.SendHttpRequestAsync(HttpMethod.Post, fixture.CompanyEndpoint, payload: company);
        Assert.True(createResponse.IsSuccessStatusCode);

        var createdCompany = await createResponse.Content.ReadFromJsonAsync<CreateCompanyResponseModel>();

        // Act
        var firstDeleteResponse = await fixture.SendHttpRequestAsync(HttpMethod.Delete, $"{fixture.CompanyEndpoint}/{createdCompany?.Id}");
        var secondDeleteResponse = await fixture.SendHttpRequestAsync(HttpMethod.Delete, $"{fixture.CompanyEndpoint}/{createdCompany?.Id}");

        // Assert
        Assert.True(firstDeleteResponse.IsSuccessStatusCode, "First delete should succeed");
        Assert.Equal(HttpStatusCode.NotFound, secondDeleteResponse.StatusCode);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #endregion
}

