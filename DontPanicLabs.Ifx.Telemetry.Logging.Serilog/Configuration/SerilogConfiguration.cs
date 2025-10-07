namespace DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Configuration;

public class SerilogConfiguration : ISerilogConfiguration
{
    public string? ConnectionString { get; init; }

    public string? TableName { get; init; } = "Logs";
}