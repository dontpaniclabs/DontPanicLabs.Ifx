using Microsoft.Extensions.Configuration;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.OpenTelemetry.Configuration;

public static class ConfigurationExtensions
{
    private static readonly string _ConfigSection = "ifx:openTelemetry";

    public static IOpenTelemetryConfiguration GetOpenTelemetryConfiguration(this IConfiguration config)
    {
        return config.GetSection(_ConfigSection).Get<OpenTelemetryConfiguration>();
    }
}