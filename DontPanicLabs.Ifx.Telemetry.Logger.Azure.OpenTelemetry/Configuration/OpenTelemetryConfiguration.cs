namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.OpenTelemetry.Configuration;

internal class OpenTelemetryConfiguration : IOpenTelemetryConfiguration
{
    public string? ConnectionString { get; set; }
}