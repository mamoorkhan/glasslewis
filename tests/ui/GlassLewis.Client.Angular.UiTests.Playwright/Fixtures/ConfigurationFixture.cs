using Microsoft.Extensions.Configuration;

namespace GlassLewis.Client.Angular.UiTests.Playwright.Fixtures;

public class ConfigurationFixture : IDisposable
{
    public IConfiguration Configuration { get; private set; }

    public string ClientUrl => Configuration[nameof(ClientUrl)] ?? throw new InvalidOperationException($"Configuration value for '{nameof(ClientUrl)}' is null.");

    public string TestUserEmail => Configuration[nameof(TestUserEmail)] ?? throw new InvalidOperationException($"Configuration value for '{nameof(TestUserEmail)}' is null.");

    public string TestUserPassword => Configuration[nameof(TestUserPassword)] ?? throw new InvalidOperationException($"Configuration value for '{nameof(TestUserPassword)}' is null.");

    public string ApiApplicationId => Configuration[nameof(ApiApplicationId)] ?? throw new InvalidOperationException($"Configuration value for '{nameof(ApiApplicationId)}' is null.");

    public ConfigurationFixture()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Ensure the namespace for SetBasePath is included  
            .AddJsonFile($"appsettings.{GetEnvironment()}.json", optional: true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();

        PlaywrightSetup.InstallBrowsers();
    }

    private static string GetEnvironment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "dev";
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
