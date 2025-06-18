namespace GlassLewis.Client.Angular.UiTests.Playwright.Fixtures;

/// <summary>
/// Represents a collection of configuration fixtures used for shared test setup.
/// </summary>
/// <remarks>This class is used to define a collection of test fixtures that can be shared across multiple test
/// classes. The collection is identified by the name "Configuration" and is associated with the <see
/// cref="ConfigurationFixture"/>.</remarks>
[CollectionDefinition("Configuration")]
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public class ConfigurationCollection : ICollectionFixture<ConfigurationFixture>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
}
