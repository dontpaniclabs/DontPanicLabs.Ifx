using DontPanicLabs.Ifx.IoC.Contracts;
using DontPanicLabs.Ifx.IoC.Contracts.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace DontPanicLabs.Ifx.IoC.Dotnet.ServiceCollection
{
    internal class Container : IContainer
    {
        private readonly ServiceProvider _serviceProvider;

        internal Container(MicrosoftServiceCollection serviceCollection)
        {
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public TService GetService<TService>() where TService : class
        {
            var service = _serviceProvider.GetService<TService>();

            IoCServiceNotFoundException.ThrowIfNull(service, typeof(TService));

            return service;
        }

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }
    }
}