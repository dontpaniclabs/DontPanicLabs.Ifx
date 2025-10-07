using Microsoft.Extensions.Configuration;

namespace DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Configuration;

public static class ConfigurationExtensions
{
    private const string ConfigSection = "ifx:telemetry:logging:serilog";

    public static ISerilogConfiguration GetSerilogConfiguration(this IConfiguration config)
    {
        return config.GetSection(ConfigSection).Get<SerilogConfiguration>() ?? new SerilogConfiguration();
    }
}