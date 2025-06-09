using DontPanicLabs.Ifx.Configuration.Local;
using DontPanicLabs.Ifx.Telemetry.Logger.Azure.ApplicationInsights.Configuration;
using DontPanicLabs.Ifx.Telemetry.Logger.Azure.ApplicationInsights.Exceptions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Configuration;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.ApplicationInsights
{
    public sealed class Logger : Contracts.ILogger, IDisposable
    {
        private readonly TelemetryClient _TelemetryClient;
        private readonly Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration _telemetryConfig;


        public Logger()
        {
            IConfiguration configuration = new Config();
            IAppInsightsConfiguration appInsightsConfig = configuration.GetAppInsightsConfiguration();

            EmptyConnectionStringException.ThrowIfEmpty(appInsightsConfig.ConnectionString!);

            _telemetryConfig = Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.CreateDefault();
            _telemetryConfig.ConnectionString = appInsightsConfig.ConnectionString;
            _TelemetryClient = new TelemetryClient(_telemetryConfig);
        }

        public void Dispose()
        {
            _TelemetryClient?.Flush();
            _telemetryConfig?.Dispose();
        }

        void Contracts.ILogger.Flush()
        {
            _TelemetryClient.Flush();
        }

        void Contracts.ILogger.Event(string eventName, IDictionary<string, string> properties, IDictionary<string, double> metrics, DateTimeOffset timeStamp)
        {
            var telEvent = new EventTelemetry(eventName)
            {
                Timestamp = timeStamp
            };

            properties
                .Select(p => KeyValuePair.Create(p.Key, p.Value))
                .ToList()
                .ForEach(pair => telEvent.Properties.Add(pair.Key, pair.Value));

            metrics
                .Select(p => KeyValuePair.Create(p.Key, p.Value))
                .ToList()
                .ForEach(pair => telEvent.Metrics.Add(pair.Key, pair.Value));

            _TelemetryClient.TrackEvent(telEvent);
        }

        void Contracts.ILogger.Log(string message, Contracts.SeverityLevel severityLevel, IDictionary<string, string> properties)
        {
            _TelemetryClient.TrackTrace(message, (SeverityLevel)severityLevel, properties);
        }

        void Contracts.ILogger.Exception(Exception ex, IDictionary<string, string> properties)
        {
            _TelemetryClient.TrackException(ex, properties);
        }

    }
}