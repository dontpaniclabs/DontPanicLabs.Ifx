using DontPanicLabs.Ifx.Configuration.Contracts.Exceptions;
using DontPanicLabs.Ifx.Configuration.Local;
using DontPanicLabs.Ifx.Telemetry.Logger.Azure.ApplicationInsights.Configuration;
using DontPanicLabs.Ifx.Telemetry.Logger.Azure.ApplicationInsights.Exceptions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.ApplicationInsights
{
    public sealed class Logger : Contracts.ILogger
    {
        private readonly TelemetryClient _TelemetryClient;

        private static ITelemetryChannel? _telemetryChannel;

        /// <remarks>
        /// `TelemetryConfiguration` is static so that it can be reused across instances of the logger; failure to
        /// re-use a single object or otherwise `Dispose` each instance results in a memory leak.
        /// </remarks>
        private static readonly Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration TelemetryConfig =
            Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.CreateDefault();

        public Logger()
        {
            IAppInsightsConfiguration appInsightsConfig = new Config().GetAppInsightsConfiguration();

            ConfigureTelemetry(appInsightsConfig);

            _TelemetryClient = new TelemetryClient(TelemetryConfig);
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

        private static void ConfigureTelemetry(IAppInsightsConfiguration appInsightsConfig)
        {
            EmptyConnectionStringException.ThrowIfEmpty(appInsightsConfig.ConnectionString!);
            TelemetryConfig.ConnectionString ??= appInsightsConfig.ConnectionString;

            InvalidTelemetryChannelException.ThrowIfChannelInvalid(appInsightsConfig.TelemetryChannel);
            if (_telemetryChannel == null)
            {
                _telemetryChannel = appInsightsConfig.TelemetryChannel switch
                {
                    "ServerTelemetryChannel" =>
                        new Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel.ServerTelemetryChannel(),
                    var channel when string.IsNullOrEmpty(channel) || channel == "InMemoryChannel" =>
                        new InMemoryChannel(),
                    _ => throw InvalidTelemetryChannelException.Create(appInsightsConfig.TelemetryChannel)
                };
                TelemetryConfig.TelemetryChannel = _telemetryChannel;
            }
        }
    }
}