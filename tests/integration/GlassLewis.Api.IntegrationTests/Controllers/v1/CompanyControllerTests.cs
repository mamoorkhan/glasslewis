using System.Data;
using System.Net;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Security.Claims;
using GlassLewis.Application.Dtos.Requests.Company;
using GlassLewis.Application.Dtos.Responses.Company;
using GlassLewis.Domain.Entities;
using GlassLewis.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GlassLewis.Api.IntegrationTests.Controllers.v1;

/// <summary>
/// Provides integration tests for the <c>CompanyController</c> API, ensuring its endpoints function correctly in
/// various scenarios, including database interactions and authentication behaviors.
/// </summary>
/// <remarks>This test class uses an in-memory database and a custom authentication scheme to simulate the
/// application environment. It includes tests for retrieving, creating, updating, and deleting companies, as well as
/// handling edge cases such as empty databases and conflicting data.</remarks>
[Trait("Category", "IntegrationTests")]
public class CompanyControllerTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string _databaseName;

    public CompanyControllerTests()
    {
        _databaseName = $"TestDb_{Guid.NewGuid()}";

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    ConfigureTestServices(services);
                });
            });

        _client = _factory.CreateClient();
    }

    private void ConfigureTestServices(IServiceCollection services)
    {
        // Remove existing DbContext
        var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CompanyDbContext>));
        if (dbContextDescriptor != null)
        {
            services.Remove(dbContextDescriptor);
        }

        // Remove authentication services
        var authDescriptors = services.Where(d =>
            d.ServiceType.FullName?.Contains("Authentication") == true ||
            d.ServiceType.FullName?.Contains("Authorization") == true ||
            d.ServiceType.FullName?.Contains("Microsoft.Identity") == true).ToList();

        foreach (var descriptor in authDescriptors)
        {
            services.Remove(descriptor);
        }

        var dbServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDbContextOptionsConfiguration<CompanyDbContext>))!;
        services.Remove(dbServiceDescriptor);

        // Add test database
        services.AddDbContext<CompanyDbContext>(options =>
        {
            options.UseInMemoryDatabase(_databaseName);
        });

        // Add test auth
        services.AddAuthentication("Test")
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("Test")
                .RequireAssertion(_ => true)
                .Build();
        });
    }

    // Helper to get a database context that uses the same database as the application
    private CompanyDbContext GetTestContext()
    {
        var options = new DbContextOptionsBuilder<CompanyDbContext>()
            .UseInMemoryDatabase(_databaseName)
            .Options;

        return new CompanyDbContext(options);
    }

    // Helper to seed data directly into the database
    private async Task SeedAsync(params Company[] companies)
    {
        using var context = GetTestContext();
        context.Companies.AddRange(companies);
        await context.SaveChangesAsync();
    }

    // Helper to clear database
    private async Task ClearDatabaseAsync()
    {
        using var context = GetTestContext();
        context.Companies.RemoveRange(context.Companies);
        await context.SaveChangesAsync();
    }

    // Helper to verify database state
    private async Task<int> CountCompaniesAsync()
    {
        using var context = GetTestContext();
        return await context.Companies.CountAsync();
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsEmpty_WhenDatabaseIsEmpty()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/company");
        var companies = await response.Content.ReadFromJsonAsync<List<GetCompanyResponseDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(companies);
        Assert.Empty(companies);
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsCompanies_WhenDataExists()
    {
        // Arrange
        await ClearDatabaseAsync();

        var testCompanies = new[]
        {
            CreateTestCompany("Apple Inc.", "AAPL", "US0378331005"),
            CreateTestCompany("Microsoft Corp", "MSFT", "US5949181045"),
            CreateTestCompany("Amazon Inc", "AMZN", "US0231351067")
        };

        await SeedAsync(testCompanies);

        // Verify seeding worked
        var dbCount = await CountCompaniesAsync();
        if (dbCount != 3)
        {
            throw new DataException($"Expected 3 companies in DB, but found {dbCount}");
        }

        // Act
        var response = await _client.GetAsync("/api/v1/company");

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new DataException($"API call failed with {response.StatusCode}: {error}");
        }

        var companies = await response.Content.ReadFromJsonAsync<List<GetCompanyResponseDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(companies);
        Assert.Equal(3, companies.Count);

        // Verify ordering (should be by name)
        Assert.Equal("Amazon Inc", companies[0].Name);
        Assert.Equal("Apple Inc.", companies[1].Name);
        Assert.Equal("Microsoft Corp", companies[2].Name);
    }

    [Fact]
    public async Task GetCompanyById_ReturnsCompany_WhenExists()
    {
        // Arrange
        await ClearDatabaseAsync();

        var testCompany = CreateTestCompany("Test Company", "TEST", "US1234567890");
        await SeedAsync(testCompany);

        // Act
        var response = await _client.GetAsync($"/api/v1/company/{testCompany.Id}");
        var company = await response.Content.ReadFromJsonAsync<GetCompanyResponseDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(company);
        Assert.Equal(testCompany.Id, company.Id);
        Assert.Equal("Test Company", company.Name);
    }

    [Fact]
    public async Task GetCompanyById_ReturnsNotFound_WhenDoesNotExist()
    {
        // Arrange
        await ClearDatabaseAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/company/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateCompany_ReturnsCreated_WhenValid()
    {
        // Arrange
        await ClearDatabaseAsync();

        var request = new CreateCompanyRequestDto
        {
            Name = "New Company",
            StockTicker = "NEW",
            Exchange = "NASDAQ",
            Isin = "US1111111111",
            Website = "https://newcompany.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/company", request);
        var created = await response.Content.ReadFromJsonAsync<CreateCompanyResponseDto>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(created);
        Assert.Equal("New Company", created.Name);

        // Verify it's in the database
        var dbCount = await CountCompaniesAsync();
        Assert.Equal(1, dbCount);
    }

    [Fact]
    public async Task CreateCompany_ReturnsConflict_WhenIsinExists()
    {
        // Arrange
        await ClearDatabaseAsync();

        var existingCompany = CreateTestCompany("Existing", "EXIST", "US1111111111");
        await SeedAsync(existingCompany);

        var request = new CreateCompanyRequestDto
        {
            Name = "New Company",
            StockTicker = "NEW",
            Exchange = "NASDAQ",
            Isin = "US1111111111", // Same ISIN
            Website = "https://newcompany.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/company", request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCompany_ReturnsOk_WhenValid()
    {
        // Arrange
        await ClearDatabaseAsync();

        var existing = CreateTestCompany("Original", "ORIG", "US1111111111");
        await SeedAsync(existing);

        var request = new UpdateCompanyRequestDto
        {
            Name = "Updated Name",
            StockTicker = "UPD",
            Exchange = "NYSE",
            Isin = "US2222222222",
            Website = "https://updated.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/company/{existing.Id}", request);
        var updated = await response.Content.ReadFromJsonAsync<UpdateCompanyResponseDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updated);
        Assert.Equal("Updated Name", updated.Name);
        Assert.Equal("UPD", updated.StockTicker);
    }

    [Fact]
    public async Task UpdateCompany_ReturnsNotFound_WhenDoesNotExist()
    {
        // Arrange
        await ClearDatabaseAsync();

        var request = new UpdateCompanyRequestDto
        {
            Name = "Updated Name",
            StockTicker = "UPD",
            Exchange = "NYSE",
            Isin = "US2222222222"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/company/{Guid.NewGuid()}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCompany_ReturnsNoContent_WhenExists()
    {
        // Arrange
        await ClearDatabaseAsync();

        var testCompany = CreateTestCompany("To Delete", "DEL", "US1234567890");
        await SeedAsync(testCompany);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/company/{testCompany.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify it's gone
        var dbCount = await CountCompaniesAsync();
        Assert.Equal(0, dbCount);
    }

    [Fact]
    public async Task DeleteCompany_ReturnsNotFound_WhenDoesNotExist()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/company/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static Company CreateTestCompany(string name, string ticker, string isin, string? website = null)
    {
        return new Company
        {
            Id = Guid.NewGuid(),
            Name = name,
            StockTicker = ticker,
            Exchange = "NYSE",
            Isin = isin,
            Website = website,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, System.Text.Encodings.Web.UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.NameIdentifier, "test-123")
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
