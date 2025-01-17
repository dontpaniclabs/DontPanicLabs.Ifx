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
        /// No interceptors will be used.
        /// </summary>
        /// <returns>New instance of ProxyFactory.</returns>
        public ProxyFactory()
        {
            Initialize(Configuration.AutoDiscoverServices, [], []);            
        }

        /// <summary>
        /// Create a new instance of the ProxyFactory. No interceptors will be used.
        /// </summary>
        /// <param name="serviceTypes">The collection of services that will be registered.  The key Type is the interface.  The values in Type[] are possible implementations</param>
        /// <returns>New instance of ProxyFactory.</returns>
        public ProxyFactory(Dictionary<Type, Type[]> serviceTypes)
        {
            Initialize(false, serviceTypes, []);
        }

        /// <summary>
        /// Create a new instance of the ProxyFactory. Service registration will be based on configuration.
        /// </summary>
        /// <param name="interceptors">The collection of interceptors to register with the IoC container.</param>
        /// <returns>New instance of ProxyFactory.</returns>
        public ProxyFactory(List<IInterceptor> interceptors)
        {
            Initialize(Configuration.AutoDiscoverServices, [], interceptors);
        }

        /// <summary>
        /// Create a new instance of the ProxyFactory.
        /// </summary>
        /// <param name="serviceTypes">The collection of services that will be registered.  The key Type is the interface.  The values in Type[] are possible implementations</param>
        /// <param name="interceptors">The collection of interceptors to register with the IoC container.</param>
        /// <returns>New instance of ProxyFactory.</returns>
        public ProxyFactory(Dictionary<Type, Type[]> serviceTypes, List<IInterceptor> interceptors) 
        {
            Initialize(false, serviceTypes, interceptors);
        }

        protected void Initialize(bool autoDiscoverServices, Dictionary<Type, Type[]> serviceTypes, List<IInterceptor> interceptors)
        {
            ContainerBuilder = new ContainerBuilder();

            var isInterceptionEnabled = Configuration.IsInterceptionEnabled;

            if (autoDiscoverServices)
            {
                ContainerBuilder = ContainerBuilder.AutoDiscoverServices(isInterceptionEnabled);
            }

            if (serviceTypes.Count > 0)
            { 
                ContainerBuilder = ContainerBuilder.RegisterServices(serviceTypes, isInterceptionEnabled);
            }

            if (!autoDiscoverServices && serviceTypes.Count == 0)
            {
                ContainerBuilder = ContainerBuilder.RegisterServices(Configuration.ServiceRegistrations, isInterceptionEnabled);
            }

            if (interceptors.Count > 0)
            {
                foreach (var interceptor in interceptors)
                {
                    ContainerBuilder.RegisterServices(options =>
                    {
                        options.Register(context => interceptor);
                    });
                }
            }

            Container = ContainerBuilder.Build();
        }
    }
}