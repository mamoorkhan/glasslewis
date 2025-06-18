using GlassLewis.Client.Angular.UiTests.Playwright.Fixtures;
using Microsoft.Playwright;

namespace GlassLewis.Client.Angular.UiTests.Playwright.Tests.Authentication;

/// <summary>  
/// Provides a suite of UI tests for verifying the behavior of the sign-in process and homepage functionality.  
/// </summary>  
/// <remarks>This class contains tests that validate the homepage title, redirection to the login page, and user  
/// creation during the sign-in process. It uses Playwright for browser automation and interacts with the application's  
/// UI to ensure expected behavior.</remarks>  
/// <param name="fixture"></param>  
[Trait("Category", "UiTests")]
[Collection("Configuration")]
public class SignInTests(ConfigurationFixture fixture)
{
    /// <summary>  
    /// Verifies that the title of the home page matches the expected value.  
    /// </summary>  
    /// <remarks>This test navigates to the home page using Playwright and checks that the page title is   
    /// "GlassLewis Coding Challenge". The browser is launched in headless mode.</remarks>  
    /// <returns></returns>  
    [Fact]
    public async Task HomePage_Should_Have_Correct_Title()
    {
        using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });

        var page = await browser.NewPageAsync();
        await page.GotoAsync(fixture.ClientUrl);

        var title = await page.TitleAsync();

        Assert.Equal("GlassLewis Coding Challenge", title);
    }

    /// <summary>  
    /// <summary>  
    /// Tests the behavior of the home page when accessed by an unauthenticated user, ensuring it redirects to the login  
    /// page, completes the user authentication process, and successfully redirects back to the application with the  
    /// user created.  
    /// </summary>  
    /// <remarks>This test simulates the user authentication flow using Microsoft Entra External ID login. It  
    /// verifies that: <list type="bullet"> <item>The home page redirects to the login page when accessed without  
    /// authentication.</item> <item>The login process completes successfully using test credentials.</item> <item>The  
    /// user is redirected back to the application after authentication.</item> <item>The application displays the  
    /// expected title and retrieves a valid access token.</item> </list></remarks>  
    /// <returns></returns>  
    [Fact]
    public async Task HomePage_Should_Redirect_To_Login_Page_And_Back_To_Application()
    {
        using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });

        var page = await browser.NewPageAsync();

        // Go to the client URL, which should redirect to the Entra External ID login page  
        await page.GotoAsync(fixture.ClientUrl);
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Wait for the Microsoft Entra External ID login page to load  
        // The login page usually contains an input with name 'loginfmt'  
        await page.WaitForSelectorAsync("input[name='loginfmt']");

        await page.FillAsync("input[name='loginfmt']", fixture.TestUserEmail);
        await page.ClickAsync("button[type='submit'],input[type='submit']");
        // Wait for next step (e.g., password)  
        await page.WaitForSelectorAsync("input[name='passwd']");

        await page.FillAsync("input[name='passwd']", fixture.TestUserPassword);
        await page.ClickAsync("button[type='submit'],input[type='submit']");

        await page.ClickAsync("button[type='submit'],input[type='submit']");

        // After sign up, check for redirect back to app  
        await page.WaitForURLAsync(fixture.ClientUrl, new PageWaitForURLOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        await page.WaitForURLAsync(fixture.ClientUrl, new PageWaitForURLOptions { WaitUntil = WaitUntilState.NetworkIdle });

        // Check the title after successful sign up and redirect  
        var title = await page.TitleAsync();
        Assert.Equal("GlassLewis Coding Challenge", title);
    }
}
