using Autofac;
using DontPanicLabs.Ifx.Proxy.Contracts;
using DontPanicLabs.Ifx.Services.Contracts;
using ContainerBuilder = DontPanicLabs.Ifx.IoC.Autofac.ContainerBuilder;
using IContainer = DontPanicLabs.Ifx.IoC.Contracts.IContainer;

namespace DontPanicLabs.Ifx.Proxy.Autofac
{
    public class ProxyFactory : ProxyFactoryBase
    {
        protected override IContainer RegisterFromAutoDiscover()
        {
            var builder = new ContainerBuilder();

            builder.RegisterServices(options =>
            {
                options.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                    .Where(t =>
                        typeof(IService).IsAssignableFrom(t)
                    )
                    .AsImplementedInterfaces();
            });

            return builder.Build();
        }

        protected override IContainer RegisterFromConfiguration(Dictionary<Type, Type[]> serviceTypes)
        {
            var builder = new ContainerBuilder();

            builder.RegisterServices(options =>
            {
                foreach (var contract in serviceTypes)
                {
                    foreach (var implementation in contract.Value)
                    {
                        options.RegisterType(implementation).As(contract.Key);
                    }
                }
            });

            return builder.Build();
        }
    }
}