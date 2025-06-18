namespace GlassLewis.Api.SystemTests.Fixtures;

/// <summary>
/// Represents a collection of configuration-related tests.
/// </summary>
/// <remarks>This class is used to define a collection fixture for configuration tests, enabling shared setup and
/// teardown logic across multiple test classes. It is associated with the <see cref="ConfigurationFixture"/>.</remarks>
[CollectionDefinition("Configuration")]
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public class ConfigurationCollection : ICollectionFixture<ConfigurationFixture>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
}
