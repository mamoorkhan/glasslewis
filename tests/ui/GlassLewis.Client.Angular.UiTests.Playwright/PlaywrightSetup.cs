namespace GlassLewis.Client.Angular.UiTests.Playwright;

/// <summary>
/// Provides functionality to set up and install browsers required for Playwright testing.
/// </summary>
/// <remarks>This class includes methods to install the default browsers supported by Playwright. It ensures that
/// the browsers are installed only once during the application's lifecycle.</remarks>
public static class PlaywrightSetup
{
    private static bool _isInstalled;

    public static bool InstallBrowsers()
    {
        if (!_isInstalled)
        {
            Console.WriteLine("Installing browsers");

            // The following line installs the default browsers. If you only need a subset of browsers,
            // you can specify the list of browsers you want to install among: chromium, chrome,
            // chrome-beta, msedge, msedge-beta, msedge-dev, firefox, and webkit.
            // var exitCode = Microsoft.Playwright.Program.Main(new[] { "install", "webkit", "chrome" });
            // If you need to install dependencies, you can add "--with-deps"
            var exitCode = Microsoft.Playwright.Program.Main(["install", "--with-deps"]);
            if (exitCode != 0)
            {
                Console.WriteLine("Failed to install browsers");
                Environment.Exit(exitCode);
            }

            _isInstalled = true;
            Console.WriteLine("Browsers installed");
        }

        return _isInstalled;
    }
}
