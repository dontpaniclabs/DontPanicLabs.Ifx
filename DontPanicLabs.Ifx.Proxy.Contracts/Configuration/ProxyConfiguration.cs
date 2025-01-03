using DontPanicLabs.Ifx.Configuration.Contracts.Exceptions;
using DontPanicLabs.Ifx.Proxy.Contracts.Exceptions;

namespace DontPanicLabs.Ifx.Proxy.Contracts.Configuration
{
    internal class ProxyConfiguration : IProxyConfiguration
    {
        private const string ServiceRegistrationPath = "ifx:proxy:ServiceRegistrations";
        private const string AutoDiscoverPath = "ifx:proxy:AutodiscoverServices";

        public ProxyConfiguration(BindingConfiguration bindingConfig) 
        {
            AutoDiscoverServices = GetAutoDiscoveryValue(bindingConfig);
            ServiceRegistrations = GetServiceRegistrations(bindingConfig);
        }

        public bool AutoDiscoverServices { get;}

        public Dictionary<Type, Type[]> ServiceRegistrations { get; }

        private Dictionary<Type, Type[]> GetServiceRegistrations(BindingConfiguration bindingConfig)
        {
            var serviceRegistrations = bindingConfig.ServiceRegistrations;

            NullConfigurationValueException.ThrowIfNull(serviceRegistrations, ServiceRegistrationPath);

            Dictionary<Type, Type[]> result = new Dictionary<Type, Type[]>();

            foreach (string key in serviceRegistrations.Keys)
            {
                Type? keyType = Type.GetType(key);

                ProxyTypeLoadException.ThrowIfNull(keyType, key);

                Type[] valueTypes = serviceRegistrations[key]
                    .Select(impl =>
                    {
                        var type = Type.GetType(impl);

                        ProxyTypeLoadException.ThrowIfNull(type, impl);

                        return type;
                    }).ToArray();

                result[keyType] = valueTypes;
            }

            return result;
        }

        private bool GetAutoDiscoveryValue(BindingConfiguration bindingConfig)
        {
            NullConfigurationValueException.ThrowIfNull(bindingConfig.AutoDiscoverServices, AutoDiscoverPath);

            bool autoDiscover = bindingConfig.AutoDiscoverServices.Value;

            return autoDiscover;
        }
    }
}
