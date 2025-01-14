using DontPanicLabs.Ifx.Configuration.Local;
using DontPanicLabs.Ifx.IoC.Contracts;
using DontPanicLabs.Ifx.Proxy.Contracts.Configuration;
using DontPanicLabs.Ifx.Proxy.Contracts.Exceptions;
using DontPanicLabs.Ifx.Services.Contracts;
using Microsoft.Extensions.Configuration;

namespace DontPanicLabs.Ifx.Proxy.Contracts
{
    public abstract class ProxyFactoryBase
    {
        protected IContainer? Container;

        protected IProxyConfiguration Configuration;
        
        protected ProxyFactoryBase()
        {
            IConfiguration configuration = new Config();

            Configuration = configuration.GetProxyConfiguration();
        }

        public I ForSubsystem<I>() where I : class, ISubsystem
        {
            NamespaceException.ThrowIfNotSubsystem(typeof(I));

            if(Container is null) throw new ArgumentNullException(nameof(Container));

            I subsystem = Container.GetService<I>();

            return subsystem;
        }
        
        public I ForComponent<I>(object caller) where I : class, IComponent
        {
            if (caller == null)
            { 
                throw new ArgumentNullException(nameof(caller), "Invalid component call. Must supply a caller.");
            }

            NamespaceException.ThrowIfNotComponent(typeof(I));

            if (Container is null) throw new ArgumentNullException(nameof(Container));

            I component = Container.GetService<I>();

            return component;
        }

        public I ForUtility<I>() where I : class, IUtility
        {
            NamespaceException.ThrowIfNotUtility(typeof(I));

            if (Container is null) throw new ArgumentNullException(nameof(Container));

            I component = Container.GetService<I>();

            return component;
        }
    }
}