using Microsoft.Extensions.Configuration;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.ApplicationInsights.Configuration
{
    public static class ConfigurationExtensions
    {
        private static readonly string _ConfigSection = "ifx:telemetry:logging:applicationInsights";

        public static IAppInsightsConfiguration GetAppInsightsConfiguration(this IConfiguration config)
        {
            return config.GetSection(_ConfigSection).Get<AppInsightsConfiguration>();
        }
    }
}
