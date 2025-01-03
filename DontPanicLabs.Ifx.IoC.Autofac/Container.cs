using Autofac;
using Autofac.Core;
using DontPanicLabs.Ifx.IoC.Contracts.Exceptions;

namespace DontPanicLabs.Ifx.IoC.Autofac
{
    internal class Container : Contracts.IContainer
    {
        private readonly AutofacContainer _container;

        internal Container(AutofacContainer container)
        {
            _container = container;
        }

        public TService GetService<TService>() where TService : class
        {
            IoCServiceNotFoundException.ThrowIfFalse(_container.IsRegistered(typeof(TService)), typeof(TService));

            try
            {
                return _container.Resolve<TService>();
            }
            catch (DependencyResolutionException ex)
            {
                throw new IoCServiceResolutionException(
                    $"An error occurred while resolving service of type '{typeof(TService).Name}'.",
                    ex
                );
            }
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}