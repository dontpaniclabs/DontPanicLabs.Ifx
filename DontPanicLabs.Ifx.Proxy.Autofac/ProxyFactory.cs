using Autofac;
using DontPanicLabs.Ifx.Proxy.Contracts;
using ContainerBuilder = DontPanicLabs.Ifx.IoC.Autofac.ContainerBuilder;

namespace DontPanicLabs.Ifx.Proxy.Autofac
{
    public class ProxyFactory : ProxyFactoryBase, IProxy
    {
        private ContainerBuilder? ContainerBuilder;

        // autodiscover will be pulled from config. if true, auto discover.  if false, use explicit configuration
        public ProxyFactory()
        {
            Initialize(Configuration.AutoDiscoverServices, [], []);            
        }

        // register this set of services.  No interceptors.
        public ProxyFactory(Dictionary<Type, Type[]> serviceTypes)
        {
            Initialize(false, serviceTypes, []);
        }

        // autodiscover will be pulled from config. if true, auto discover.  if false, use configuration.  always register this set of interceptors.
        public ProxyFactory(List<IInterceptor> interceptors)
        {
            Initialize(Configuration.AutoDiscoverServices, [], interceptors);
        }

        // register this set of services.  register this set of interceptors
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

        //private static void RegisterFromAutoDiscover()
        //{
        //    bool interceptionEnabled = false;

        //    ContainerBuilder.RegisterServices(builder =>
        //    {
        //        var dynamic = builder
        //            .RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
        //            .Where(t =>
        //                typeof(IService).IsAssignableFrom(t)
        //             ).AsImplementedInterfaces()
        //             .EnableInterfaceInterceptors();
                
        //    });


        //}

        //private void RegisterFromConfiguration(Dictionary<Type, Type[]> serviceTypes)
        //{
        //    ContainerBuilder.RegisterServices(options =>
        //    {
        //        foreach (var contract in serviceTypes)
        //        {
        //            foreach (var implementation in contract.Value)
        //            {
        //                options.RegisterType(implementation).As(contract.Key);
        //            }
        //        }
        //    });
        //}
    }
}