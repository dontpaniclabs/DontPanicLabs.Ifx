using DontPanicLabs.Ifx.Configuration.Local;
using DontPanicLabs.Ifx.Telemetry.Logger.Contracts;
using DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Configuration;
using DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Exceptions;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using ILogger = DontPanicLabs.Ifx.Telemetry.Logger.Contracts.ILogger;
using SerilogLogger = Serilog.Core.Logger;
using ISerilogLogger = Serilog.ILogger;

namespace DontPanicLabs.Ifx.Telemetry.Logging.Serilog;

public sealed class Logger : ILogger, IDisposable
{
    private readonly SerilogLogger _serilogLogger;
    private bool _disposed;

    public Logger()
    {
        ISerilogConfiguration serilogConfig = new Config().GetSerilogConfiguration();
        ConfigureLogger(serilogConfig);

        EmptyConnectionStringException.ThrowIfEmpty(serilogConfig.ConnectionString ?? "");

        var sinkOptions = new MSSqlServerSinkOptions
        {
            TableName = "Logs",
            SchemaName = "dbo",
            AutoCreateSqlTable = true,
            BatchPostingLimit = 50,
            BatchPeriod = TimeSpan.FromSeconds(5)
        };

        _serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.MSSqlServer(
                connectionString: serilogConfig.ConnectionString!,
                sinkOptions: sinkOptions)
            .CreateLogger();
    }

    // For testing purposes only
    internal Logger(SerilogLogger serilogLogger)
    {
        _serilogLogger = serilogLogger;
    }

    void ILogger.Log(string message, SeverityLevel severityLevel, IDictionary<string, string> properties)
    {
        var logLevel = MapSeverityToLogLevel(severityLevel);

        // Create a log context with properties
        ISerilogLogger logContext = _serilogLogger;
        if (properties != null && properties.Any())
        {
            foreach (var prop in properties)
            {
                logContext = logContext.ForContext(prop.Key, prop.Value);
            }
        }

        logContext.Write(logLevel, message);
    }

    void ILogger.Exception(Exception exception, IDictionary<string, string> properties)
    {
        // Create a log context with properties
        ISerilogLogger logContext = _serilogLogger;
        if (properties != null && properties.Any())
        {
            foreach (var prop in properties)
            {
                logContext = logContext.ForContext(prop.Key, prop.Value);
            }
        }

        logContext.Error(exception, exception.Message);
    }

    void ILogger.Event(string eventName, IDictionary<string, string> properties, IDictionary<string, double> metrics, DateTimeOffset timeStamp)
    {
        // Serilog doesn't have a separate Event concept, so we log it as Information with properties
        ISerilogLogger logContext = _serilogLogger
            .ForContext("EventName", eventName)
            .ForContext("Timestamp", timeStamp);

        if (properties != null && properties.Any())
        {
            foreach (var prop in properties)
            {
                logContext = logContext.ForContext(prop.Key, prop.Value);
            }
        }

        if (metrics != null && metrics.Any())
        {
            foreach (var metric in metrics)
            {
                logContext = logContext.ForContext(metric.Key, metric.Value);
            }
        }

        logContext.Information("Event: {EventName}", eventName);
    }

    void ILogger.Flush()
    {
        _serilogLogger.Dispose();
    }

    private static LogEventLevel MapSeverityToLogLevel(SeverityLevel severityLevel)
    {
        return severityLevel switch
        {
            SeverityLevel.Verbose => LogEventLevel.Verbose,
            SeverityLevel.Information => LogEventLevel.Information,
            SeverityLevel.Warning => LogEventLevel.Warning,
            SeverityLevel.Error => LogEventLevel.Error,
            SeverityLevel.Critical => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }

    private static void ConfigureLogger(ISerilogConfiguration serilogConfig)
    {
        EmptyConnectionStringException.ThrowIfEmpty(serilogConfig.ConnectionString);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _serilogLogger?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}