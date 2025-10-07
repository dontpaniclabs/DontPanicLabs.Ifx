using Microsoft.Extensions.Configuration;

namespace DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Configuration;

public static class ConfigurationExtensions
{
    private const string ConfigSection = "ifx:telemetry:logging:serilog";
    private const string SinksSection = "sinks";

    /// <summary>
    /// Gets all configured Serilog sink configurations from the configuration.
    /// </summary>
    /// <param name="config">The IConfiguration instance.</param>
    /// <returns>An enumerable of ISerilogConfiguration instances representing the configured sinks.</returns>
    public static IEnumerable<ISerilogConfiguration> GetSerilogConfigurations(this IConfiguration config)
    {
        var serilogSection = config.GetSection(ConfigSection);
        var sinksSection = serilogSection.GetSection(SinksSection);

        var sinkConfigs = new List<ISerilogConfiguration>();

        // Get all sink configurations from the array
        var sinkSections = sinksSection.GetChildren();

        foreach (var sinkSection in sinkSections)
        {
            var sinkType = sinkSection["type"]?.ToLower();

            ISerilogConfiguration? sinkConfig = sinkType switch
            {
                "sql" => sinkSection.Get<SqlSinkConfiguration>(),
                "file" => sinkSection.Get<FileSinkConfiguration>(),
                "console" => sinkSection.Get<ConsoleSinkConfiguration>(),
                _ => null // Unknown sink type, skip
            };

            if (sinkConfig != null)
            {
                sinkConfigs.Add(sinkConfig);
            }
        }

        // If no sinks configured, return a default SQL sink for backward compatibility
        if (sinkConfigs.Count == 0)
        {
            // Try to load legacy single-sink configuration format
            var legacyConfig = serilogSection.Get<SqlSinkConfiguration>();
            if (legacyConfig != null && !string.IsNullOrEmpty(legacyConfig.ConnectionString))
            {
                sinkConfigs.Add(legacyConfig);
            }
        }

        return sinkConfigs;
    }
}