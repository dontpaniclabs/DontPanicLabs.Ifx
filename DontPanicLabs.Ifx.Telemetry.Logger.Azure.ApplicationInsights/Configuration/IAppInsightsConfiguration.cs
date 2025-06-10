namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.ApplicationInsights.Configuration
{
    public interface IAppInsightsConfiguration
    {
        string? ConnectionString { get; }

        /// <summary>
        /// The `TelemetryChannel` to use, either "InMemoryChannel" or "ServerTelemetryChannel".
        /// </summary>
        /// <remarks>
        /// Microsoft recommends using "ServerTelemetryChannel" in all production scenarios.
        /// </remarks>
        string? TelemetryChannel { get; }
    }
}