namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.ApplicationInsights.Configuration
{
    internal class AppInsightsConfiguration : IAppInsightsConfiguration
    {
        public string? ConnectionString { get; set; }

        public string? TelemetryChannel { get; set; }
    }
}