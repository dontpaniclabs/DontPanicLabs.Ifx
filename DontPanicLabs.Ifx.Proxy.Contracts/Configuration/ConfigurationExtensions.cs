using DontPanicLabs.Ifx.Configuration.Contracts.Exceptions;
using Microsoft.Extensions.Configuration;

namespace DontPanicLabs.Ifx.Proxy.Contracts.Configuration
{
    internal static class ConfigurationExtensions
    {
        private static readonly string _ConfigSection = "ifx:proxy";

        public static IProxyConfiguration GetProxyConfiguration(this IConfiguration config)
        {
            // This will catch if the section doesn't exist in the config
            var section = config.GetRequiredSection(_ConfigSection);

            BindingConfiguration? bindableConfiguration = section.Get<BindingConfiguration>();

            NullConfigurationValueException.ThrowIfNull(bindableConfiguration, _ConfigSection);

            IProxyConfiguration proxyConfiguration = new ProxyConfiguration(bindableConfiguration);

            return proxyConfiguration;
        }

    }
}