using Azure.Monitor.OpenTelemetry.Exporter;
using DontPanicLabs.Ifx.Configuration.Local;
using DontPanicLabs.Ifx.Telemetry.Logger.Azure.OpenTelemetry.Configuration;
using DontPanicLabs.Ifx.Telemetry.Logger.Azure.OpenTelemetry.Exceptions;
using DontPanicLabs.Ifx.Telemetry.Logger.Contracts;
using Microsoft.Extensions.Logging;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.OpenTelemetry;

public class Logger : ILogger
{
    private static ILoggerFactory? _LoggerFactory;
    private static readonly object _LockObject = new();
    private static bool _IsInitialized;

    private readonly Microsoft.Extensions.Logging.ILogger _logger;

    public Logger()
    {
        var openTelemetryConfig = new Config().GetOpenTelemetryConfiguration();

        ConfigureTelemetry(openTelemetryConfig);

        _logger = _LoggerFactory!.CreateLogger<Logger>();
    }

    // For testing purposes only
    internal Logger(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Logger>();
    }

    private static void ConfigureTelemetry(IOpenTelemetryConfiguration openTelemetryConfig)
    {
        if (_IsInitialized)
        {
            return;
        }

        lock (_LockObject)
        {
            if (_IsInitialized)
            {
                return;
            }

            EmptyConnectionStringException.ThrowIfEmpty(openTelemetryConfig.ConnectionString ?? "");

            _LoggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddOpenTelemetry(options =>
                {
                    options.AddAzureMonitorLogExporter(exporterOptions =>
                    {
                        exporterOptions.ConnectionString = openTelemetryConfig.ConnectionString;
                    });
                    options.IncludeFormattedMessage = true;
                    options.IncludeScopes = true;
                });
            });

            _IsInitialized = true;
        }
    }

    public void Log(string message, SeverityLevel severityLevel)
    {
        var logLevel = ConvertSeverityLevel(severityLevel);
        _logger.Log(logLevel, message);
    }

    public void Log(string message, SeverityLevel severityLevel, IDictionary<string, string> properties)
    {
        throw new PlatformNotSupportedException("Custom properties are not supported.");
    }

    public void Exception(Exception exception)
    {
        _logger.LogError(exception, "Exception occurred");
    }

    public void Exception(Exception exception, IDictionary<string, string> properties)
    {
        throw new PlatformNotSupportedException("Custom properties are not supported.");
    }

    public void Event(
        string eventName,
        IDictionary<string, string>? properties,
        IDictionary<string, double>? metrics,
        DateTimeOffset timeStamp
    )
    {
        // https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/monitor/Azure.Monitor.OpenTelemetry.Exporter#customevents
        var templateParts = new List<string>
        {
            "{microsoft.custom_event.name}"
        };
        var values = new List<object>
        {
            eventName
        };

        if (properties != null)
        {
            foreach (var property in properties)
            {
                templateParts.Add("{" + property.Key + "}");
                values.Add(property.Value);
            }
        }

        if (metrics != null)
        {
            foreach (var metric in metrics)
            {
                templateParts.Add("{" + metric.Key + "}");
                values.Add(metric.Value);
            }
        }

        // Create the final template string
        var template = string.Join(" ", templateParts);

        _logger.LogInformation(template, values.ToArray());
    }

    public void Flush()
    {
        // OpenTelemetry providers in newer versions handle flushing automatically
        // The Azure Monitor exporter manages batching and flushing internally
        // Force a brief delay to allow any pending telemetry to be sent
        Thread.Sleep(1000);
    }

    private static LogLevel ConvertSeverityLevel(SeverityLevel severityLevel)
    {
        return severityLevel switch
        {
            SeverityLevel.Verbose => LogLevel.Trace,
            SeverityLevel.Information => LogLevel.Information,
            SeverityLevel.Warning => LogLevel.Warning,
            SeverityLevel.Error => LogLevel.Error,
            SeverityLevel.Critical => LogLevel.Critical,
            _ => LogLevel.Information
        };
    }
}