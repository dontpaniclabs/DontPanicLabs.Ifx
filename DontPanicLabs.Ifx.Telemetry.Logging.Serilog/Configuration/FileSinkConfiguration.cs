using Serilog;

namespace DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Configuration;

/// <summary>
/// Configuration for rolling file sink.
/// </summary>
public class FileSinkConfiguration : ISerilogConfiguration
{
    /// <summary>
    /// Path to the log file. Supports date-based tokens like "logs/log-.txt"
    /// </summary>
    public string? FilePath { get; init; } = "logs/log-.txt";

    /// <summary>
    /// Rolling interval for log files (Day, Hour, Minute, etc.)
    /// </summary>
    public global::Serilog.RollingInterval RollingInterval { get; init; } = global::Serilog.RollingInterval.Day;

    /// <summary>
    /// Number of log files to retain. Older files are deleted.
    /// </summary>
    public int? RetainedFileCountLimit { get; init; } = 7;

    public void ConfigureSink(LoggerConfiguration loggerConfig)
    {
        loggerConfig.WriteTo.File(
            path: FilePath!,
            rollingInterval: RollingInterval,
            retainedFileCountLimit: RetainedFileCountLimit);
    }
}
