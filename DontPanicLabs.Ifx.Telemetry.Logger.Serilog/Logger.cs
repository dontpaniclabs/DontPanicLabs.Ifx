using System.Text;
using DontPanicLabs.Ifx.Configuration.Local;
using DontPanicLabs.Ifx.Telemetry.Logger.Contracts;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Settings.Configuration;
using SerilogLogger = Serilog.Core.Logger;
using ISerilogLogger = Serilog.ILogger;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Serilog;

/// <summary>
/// `DontPanicLabs.Ifx.Telemetry.Logger.Contracts.ILogger` implementation using Serilog.
/// </summary>
public sealed class Logger : ILogger, IDisposable
{
    private readonly SerilogLogger _logger;
    private bool _disposed;
    private const string SerilogConfigSectionName = "ifx:telemetry:logging:serilog";

    /// <summary>
    /// A constructor that initializes the logger using configuration from appsettings.json or environment variables.
    /// The configuration section used is "ifx:telemetry:logging:serilog".
    /// </summary>
    public Logger()
    {
        _logger = new LoggerConfiguration()
            .ReadFrom.Configuration(new Config(), new ConfigurationReaderOptions
            {
                SectionName = SerilogConfigSectionName
            })
            .CreateLogger();
    }

    /// <summary>
    /// Initializes the logger using a provided Serilog configuration in JSON format.
    /// </summary>
    /// <param name="serilogConfigJson">
    /// The Serilog configuration as a JSON string.
    /// <example>
    /// {
    ///   "MinimumLevel": "Debug",
    ///   "WriteTo": [ "Console" ]
    /// }
    /// </example>
    /// </param>
    public Logger(string serilogConfigJson)
    {
        // Wrap the provided JSON in a parent object to create a valid configuration section; the name of the
        // section doesn't really matter as long as it matches what we specify below in config reader options.
        var wrappedJson = $"{{ \"{SerilogConfigSectionName}\": {serilogConfigJson} }}";
        using var configMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(wrappedJson));

        var config = new ConfigurationBuilder()
            .AddJsonStream(configMemoryStream)
            .Build();

        _logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config, new ConfigurationReaderOptions
            {
                SectionName = SerilogConfigSectionName
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

    public void Log(string message, SeverityLevel severityLevel, IDictionary<string, string>? properties)
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

    public void Exception(Exception exception, IDictionary<string, string>? properties)
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

    public void Event(string eventName, IDictionary<string, string>? properties, IDictionary<string, double>? metrics,
        DateTimeOffset timeStamp)
    {
        // Serilog doesn't have a separate Event concept, so we log it as Information with properties encoding that
        // this is an Event.
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

        if (metrics is { Count: > 0 })
        {
            foreach (var metric in metrics)
            {
                logger = logger.ForContext(metric.Key, metric.Value);
            }
        }

        logger.Information("Event: {EventName}", eventName);
    }

    public void Flush()
    {
        throw new PlatformNotSupportedException(
            "Serilog does not implement a Flush method, dispose of instances instead.");
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _logger.Dispose();
        _disposed = true;
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
}