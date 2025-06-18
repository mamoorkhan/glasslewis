namespace GlassLewis.Api.SystemTests;

/// <summary>
/// Provides functionality to set up Playwright by installing required browsers.
/// </summary>
/// <remarks>This class is designed to facilitate the installation of browsers required by Playwright. The <see
/// cref="InstallBrowsers"/> method installs the default set of browsers, including their dependencies. If you need to
/// install a specific subset of browsers, you can modify the installation command accordingly.</remarks>
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
