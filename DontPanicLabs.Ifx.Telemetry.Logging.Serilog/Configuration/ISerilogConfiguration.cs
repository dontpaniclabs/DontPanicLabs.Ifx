namespace DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Configuration;

/// <summary>
/// Configuration settings for Serilog logging.
/// </summary>
public interface ISerilogConfiguration
{
    /// <summary>
    /// SQL Server connection string for logging database.
    /// </summary>
    public string? ConnectionString { get; init; }

    /// <summary>
    /// The name of the table to write logs to.
    /// </summary>
    public string? TableName { get; init; }
}