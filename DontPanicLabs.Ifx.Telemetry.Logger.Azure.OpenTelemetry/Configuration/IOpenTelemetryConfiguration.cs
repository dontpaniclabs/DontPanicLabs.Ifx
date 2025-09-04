namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.OpenTelemetry.Configuration;

public interface IOpenTelemetryConfiguration
{
    string? ConnectionString { get; }
}