using DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Exceptions;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Configuration;

/// <summary>
/// Configuration for SQL Server sink.
/// </summary>
public class SqlSinkConfiguration : ISerilogConfiguration
{
    /// <summary>
    /// SQL Server connection string for logging database.
    /// </summary>
    public string? ConnectionString { get; init; }

    /// <summary>
    /// The name of the table to write logs to.
    /// </summary>
    public string? TableName { get; init; } = "Logs";

    /// <summary>
    /// The database schema name.
    /// </summary>
    public string? SchemaName { get; init; } = "dbo";

    /// <summary>
    /// Whether to automatically create the SQL table if it doesn't exist.
    /// </summary>
    public bool AutoCreateSqlTable { get; init; } = true;

    /// <summary>
    /// Maximum number of log events to batch before writing.
    /// </summary>
    public int BatchPostingLimit { get; init; } = 50;

    /// <summary>
    /// Time period (in seconds) to wait between batch writes.
    /// </summary>
    public int BatchPeriodSeconds { get; init; } = 5;

    public void ConfigureSink(LoggerConfiguration loggerConfig)
    {
        EmptyConnectionStringException.ThrowIfEmpty(ConnectionString ?? "");

        var sinkOptions = new MSSqlServerSinkOptions
        {
            TableName = TableName ?? "Logs",
            SchemaName = SchemaName ?? "dbo",
            AutoCreateSqlTable = AutoCreateSqlTable,
            BatchPostingLimit = BatchPostingLimit,
            BatchPeriod = TimeSpan.FromSeconds(BatchPeriodSeconds)
        };

        loggerConfig.WriteTo.MSSqlServer(
            connectionString: ConnectionString!,
            sinkOptions: sinkOptions);
    }
}
