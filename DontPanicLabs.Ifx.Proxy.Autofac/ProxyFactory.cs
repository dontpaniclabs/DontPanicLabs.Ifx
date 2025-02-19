using Autofac;
using DontPanicLabs.Ifx.Proxy.Contracts;
using ContainerBuilder = DontPanicLabs.Ifx.IoC.Autofac.ContainerBuilder;

namespace DontPanicLabs.Ifx.Proxy.Autofac
{
    public class ProxyFactory : ProxyFactoryBase, IProxyFactory
    {
        private ContainerBuilder? ContainerBuilder;

        /// <summary>
        /// Create a new instance of the ProxyFactory. 
        /// Service registrations will be based on configuration.
        /// Interceptors will be auto-discoverd based on configuration.
        /// </summary>
        /// <returns>New instance of ProxyFactory.</returns>
        public ProxyFactory()
        {
            Initialize(Configuration.AutoDiscoverServices, [], []);            
        }

        /// <summary>
        /// Create a new instance of the ProxyFactory.
        /// </summary>
        /// <param name="serviceTypes">The collection of services that will be registered.  The key Type is the interface.  The values in Type[] are possible implementations.  An empty list will result in no registrations.</param>
        /// <param name="interceptors">The collection of interceptors to register with the IoC container.  An empty list will result in no interceptors.  Interception can be enabled/disabled based on configuration.</param>
        /// <returns>New instance of ProxyFactory.</returns>
        public ProxyFactory(Dictionary<Type, Type[]> serviceTypes, List<IInterceptor> interceptors) 
        {
            Initialize(false, serviceTypes, interceptors);
        }

        public ProxyFactory(ContainerBuilder builder)
        {
            Container = builder.Build();
        }

        protected void Initialize(bool autoDiscoverServices, Dictionary<Type, Type[]> serviceTypes, List<IInterceptor> interceptors)
        {
            ContainerBuilder = new ContainerBuilder();

            var isInterceptionEnabled = Configuration.IsInterceptionEnabled;

            if (autoDiscoverServices)
            {
                ContainerBuilder = ContainerBuilder.AutoDiscoverServices(isInterceptionEnabled);
            }
            else
            {
                if (serviceTypes.Count > 0)
                {
                    ContainerBuilder = ContainerBuilder.RegisterServices(serviceTypes, isInterceptionEnabled);
                }

                if (serviceTypes.Count == 0)
                {
                    ContainerBuilder = ContainerBuilder.RegisterServices(Configuration.ServiceRegistrations, isInterceptionEnabled);
                }

                ContainerBuilder.RegisterServices(builder =>
                {
                    foreach (var interceptor in interceptors)
                    {
                        builder.RegisterInstance(interceptor).AsSelf();
                    }
                });
            }

            Container = ContainerBuilder.Build();
        }
    }
}