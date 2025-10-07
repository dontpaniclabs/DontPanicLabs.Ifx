using Serilog;

namespace DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Configuration;

/// <summary>
/// Base interface for Serilog sink configuration.
/// Implementations define how to configure specific Serilog sinks (SQL, File, Console, etc.)
/// </summary>
public interface ISerilogConfiguration
{
    /// <summary>
    /// Configures a Serilog sink on the provided LoggerConfiguration.
    /// </summary>
    /// <param name="loggerConfig">The LoggerConfiguration to add the sink to.</param>
    void ConfigureSink(LoggerConfiguration loggerConfig);
}