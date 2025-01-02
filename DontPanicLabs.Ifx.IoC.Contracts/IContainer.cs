using DontPanicLabs.Ifx.IoC.Contracts.Exceptions;

namespace DontPanicLabs.Ifx.IoC.Contracts
{
    /// <summary>
    /// Defines a contract for a dependency injection container.
    /// </summary>
    /// <remarks>
    /// The container manages the lifecycle and resolution of services. It includes methods for retrieving services and 
    /// managing the container's lifecycle. Implementations should release resources when no longer needed.
    /// </remarks>
    public interface IContainer : IDisposable
    {
        /// <summary>
        /// Retrieve a service from the container.
        /// </summary>
        /// <typeparam name="TService">The service to retrieve.</typeparam>
        /// <returns>The instance that provides the service.</returns>
        /// <exception cref="IoCServiceNotFoundException" />
        /// <exception cref="IoCServiceResolutionException" />
        TService GetService<TService>() where TService : class;
    }
}