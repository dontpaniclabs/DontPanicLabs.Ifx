using DontPanicLabs.Ifx.Configuration.Local;
using DontPanicLabs.Ifx.Telemetry.Logger.Contracts;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Settings.Configuration;
using ILogger = DontPanicLabs.Ifx.Telemetry.Logger.Contracts.ILogger;
using SerilogLogger = Serilog.Core.Logger;
using ISerilogLogger = Serilog.ILogger;

namespace DontPanicLabs.Ifx.Telemetry.Logging.Serilog;

public sealed class Logger : ILogger, IDisposable
{
    private readonly SerilogLogger _logger;
    private bool _disposed;

    public Logger()
    {
        IConfiguration config = new Config();

        _logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config, new ConfigurationReaderOptions
            {
                SectionName = "ifx:telemetry:logging:serilog"
            })
            .CreateLogger();
    }

    /// <summary>
    /// Internal constructor for unit testing purposes only.
    /// </summary>
    internal Logger(SerilogLogger logger)
    {
        _logger = logger;
    }

    void ILogger.Log(string message, SeverityLevel severityLevel, IDictionary<string, string>? properties)
    {
        var logLevel = MapSeverityToLogLevel(severityLevel);
        ISerilogLogger logger = _logger;

        if (properties is { Count: > 0 })
        {
            foreach (var prop in properties)
            {
                logger = logger.ForContext(prop.Key, prop.Value);
            }
        }

        logger.Write(logLevel, message);
    }

    void ILogger.Exception(Exception exception, IDictionary<string, string>? properties)
    {
        ISerilogLogger logger = _logger;

        if (properties is { Count: > 0 })
        {
            foreach (var prop in properties)
            {
                logger = logger.ForContext(prop.Key, prop.Value);
            }
        }

        logger.Error(exception, exception.Message);
    }

    void ILogger.Event(string eventName, IDictionary<string, string>? properties, IDictionary<string, double>? metrics,
        DateTimeOffset timeStamp)
    {
        // Serilog doesn't have a separate Event concept, so we log it as Information with properties
        ISerilogLogger logger = _logger
            .ForContext("EventName", eventName)
            .ForContext("Timestamp", timeStamp);

        if (properties is { Count: > 0 })
        {
            foreach (var prop in properties)
            {
                logger = logger.ForContext(prop.Key, prop.Value);
            }
        }

        if (metrics != null && metrics.Any())
        {
            foreach (var metric in metrics)
            {
                logger = logger.ForContext(metric.Key, metric.Value);
            }
        }

        logger.Information("Event: {EventName}", eventName);
    }

    void ILogger.Flush()
    {
        _logger.Dispose();
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

    public void Dispose()
    {
        if (!_disposed)
        {
            _logger?.Dispose();
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}