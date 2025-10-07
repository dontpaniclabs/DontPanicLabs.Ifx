using Serilog;

namespace DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Configuration;

/// <summary>
/// Configuration for console sink.
/// Useful for development and debugging scenarios.
/// </summary>
public class ConsoleSinkConfiguration : ISerilogConfiguration
{
    public void ConfigureSink(LoggerConfiguration loggerConfig)
    {
        loggerConfig.WriteTo.Console();
    }
}
