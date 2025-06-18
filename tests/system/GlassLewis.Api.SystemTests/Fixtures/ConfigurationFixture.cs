using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;

namespace GlassLewis.Api.SystemTests.Fixtures;

/// <summary>
/// Provides configuration settings and utility methods for integration tests.
/// </summary>
/// <remarks>This class initializes configuration settings from environment variables and JSON files based on the
/// current environment. It also provides access to specific configuration values and utility methods for test setup,
/// such as retrieving authentication tokens.</remarks>
public class ConfigurationFixture : IAsyncDisposable
{
    public IConfiguration Configuration { get; private set; }

    public string ApiUrl => Configuration[nameof(ApiUrl)] ?? "dev";

    public string ApiVersion => Configuration[nameof(ApiVersion)] ?? "v1";


    public string ClientUrl => Configuration[nameof(ClientUrl)] ?? throw new InvalidOperationException($"Configuration value for '{nameof(ClientUrl)}' is null.");

    public string TestUserEmail => Configuration[nameof(TestUserEmail)] ?? throw new InvalidOperationException($"Configuration value for '{nameof(TestUserEmail)}' is null.");

    public string TestUserPassword => Configuration[nameof(TestUserPassword)] ?? throw new InvalidOperationException($"Configuration value for '{nameof(TestUserPassword)}' is null.");

    public string ApiApplicationId => Configuration[nameof(ApiApplicationId)] ?? throw new InvalidOperationException($"Configuration value for '{nameof(ApiApplicationId)}' is null.");

    public string CompanyEndpoint => $"/api/{ApiVersion}/company";

    private DateTime _accesTokenExpiry = DateTime.MinValue;

    private string _accessToken = string.Empty;

    public Guid CreatedCompanyId { get; set; }

    public string? CreatedCompanyIsin { get; set; }

    public ConfigurationFixture()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{GetEnvironment()}.json", optional: true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();

        PlaywrightSetup.InstallBrowsers();
    }

    public async Task<HttpResponseMessage> SendHttpRequestAsync(HttpMethod method, string endpoint, bool withToken = true, object? payload = null, HttpContent? content = null)
    {
        using var httpClient = new HttpClient
        {
            BaseAddress = new Uri(ApiUrl)
        };

        if (withToken)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await GetToken());
        }

        var request = new HttpRequestMessage(method, endpoint);

        if(content != null)
        {
            request.Content = content;
        }
        else if (payload != null)
        {
            var json = JsonSerializer.Serialize(payload);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return await httpClient.SendAsync(request);
    }

    public async Task<string> GetToken()
    {
        if (!string.IsNullOrEmpty(_accessToken) && _accesTokenExpiry > DateTime.UtcNow)
        {
            return _accessToken;
        }

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });

        var page = await browser.NewPageAsync();

        var accessToken = await RetrieveToken(page);
        if (string.IsNullOrEmpty(accessToken))
        {
            await LoginUser(page);
            accessToken = await RetrieveToken(page);
        }

        _accesTokenExpiry = DateTime.UtcNow.AddMinutes(10);
        _accessToken = accessToken;
        return accessToken;
    }

    private async Task<string> RetrieveToken(IPage page)
    {
        return await page.EvaluateAsync<string>(
            @"async () => {
                let accessToken;
                let counter = 0;
                while(!accessToken && counter <= 5) {
                    try {
                        console.log('Token Debug: accessToken at loop start found:', accessToken);
                        console.log('Token Debug: counter at loop start found:', counter);

                        counter++;

                        // Get Angular elements and find MSAL service
                        const appRoot = document.querySelector('app-root');
                        if (!appRoot) return null;
                        console.log('Token Debug: App root found:', appRoot);
                            
                        // Get Angular component context
                        const ngComponent = window.ng?.getComponent?.(appRoot);
                        if (!ngComponent) return null;
                        console.log('Token Debug: ngComponent found:', ngComponent);

                        const msalService = ngComponent.authService;

                        if (msalService && msalService.instance) {
                            const accounts = await msalService.instance.getAllAccounts();
                            console.log('Token Debug: accounts found:', accounts);

                            if (accounts.length > 0) {
                                const tokenRequest = {
                                    scopes: ['api://" + ApiApplicationId + @"/company.read'],
                                    account: accounts[0]
                                };

                                console.log('Token Debug: tokenRequest found:', tokenRequest);

                                const response = await msalService.instance.acquireTokenSilent(tokenRequest);
                                console.log('Token Debug: response found:', response);

                                accessToken = response.accessToken;

                                console.log('Token Debug: accessToken found:', accessToken);
                            }
                        }
                    } catch (error) {
                        console.log('Error accessing MSAL through Angular:', error);
                        return error;
                    }

                    await new Promise(resolve => setTimeout(resolve, 1000));
                }

                return accessToken;
            }");

    }

    private async Task LoginUser(IPage page)
    {
        // Go to the client URL, which should redirect to the Entra External ID login page  
        await page.GotoAsync(ClientUrl);
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Wait for the Microsoft Entra External ID login page to load  
        // The login page usually contains an input with name 'loginfmt'  
        await page.WaitForSelectorAsync("input[name='loginfmt']");

        await page.FillAsync("input[name='loginfmt']", TestUserEmail);
        await page.ClickAsync("button[type='submit'],input[type='submit']");
        // Wait for next step (e.g., password)  
        await page.WaitForSelectorAsync("input[name='passwd']");

        await page.FillAsync("input[name='passwd']", TestUserPassword);
        await page.ClickAsync("button[type='submit'],input[type='submit']");

        await page.ClickAsync("button[type='submit'],input[type='submit']");

        // After sign up, check for redirect back to app  
        await page.WaitForURLAsync(ClientUrl, new PageWaitForURLOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        await page.WaitForURLAsync(ClientUrl, new PageWaitForURLOptions { WaitUntil = WaitUntilState.NetworkIdle });

        // Check the title after successful sign up and redirect  
        var title = await page.TitleAsync();
        Assert.Equal("GlassLewis Coding Challenge", title);
    }

    private static string GetEnvironment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "dev";
    }

    public async ValueTask DisposeAsync()
    {
        if (CreatedCompanyId != Guid.Empty)
        {
            await SendHttpRequestAsync(HttpMethod.Delete, $"{CompanyEndpoint}/{CreatedCompanyId}");
        }
        GC.SuppressFinalize(this);
    }
}
