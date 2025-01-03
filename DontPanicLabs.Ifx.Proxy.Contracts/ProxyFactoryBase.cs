using DontPanicLabs.Ifx.Configuration.Local;
using DontPanicLabs.Ifx.IoC.Contracts;
using DontPanicLabs.Ifx.Proxy.Contracts.Configuration;
using DontPanicLabs.Ifx.Proxy.Contracts.Exceptions;
using DontPanicLabs.Ifx.Services.Contracts;
using Microsoft.Extensions.Configuration;

namespace DontPanicLabs.Ifx.Proxy.Contracts
{
    public abstract class ProxyFactoryBase : IProxy, IProxyFactory
    {
        protected IContainer container;
        
        protected ProxyFactoryBase()
        {
            container = RegisterServices();
        }

        public I ForSubsystem<I>() where I : class, ISubsystem
        {
            NamespaceException.ThrowIfNotSubsystem(typeof(I));

            I subsystem = container.GetService<I>();

            return subsystem;
        }
        
        public I ForComponent<I>(object caller) where I : class, IComponent
        {
            if (caller == null)
            { 
                throw new ArgumentNullException(nameof(caller), "Invalid component call. Must supply a caller.");
            }

            NamespaceException.ThrowIfNotComponent(typeof(I));

            I component = container.GetService<I>();

            return component;
        }

        protected virtual IContainer RegisterServices()
        {
            IConfiguration configuration = new Config();

            IProxyConfiguration proxyConfiguration = configuration.GetProxyConfiguration();

            IContainer container;

            if (!proxyConfiguration.AutoDiscoverServices) 
            {
                
                container = RegisterFromConfiguration(proxyConfiguration.ServiceRegistrations);
            }
            else
            {
                container = RegisterFromAutoDiscover();
            }

            return container;
        }

        protected abstract IContainer RegisterFromAutoDiscover();

        protected abstract IContainer RegisterFromConfiguration(Dictionary<Type, Type[]> serviceTypes);
    }
}