using DontPanicLabs.Ifx.Proxy.Contracts.Exceptions;
using DontPanicLabs.Ifx.Services.Contracts;

namespace DontPanicLabs.Ifx.Proxy.Contracts
{
    /// <summary>
    /// Provides methods to retrieve registered instances of subsystems and components.
    /// </summary>
    public interface IProxy
    {
        /// <summary>
        /// Request a subsystem from the proxy.
        /// </summary>
        /// <typeparam name="I">The subsystem (manager) to retrieve.</typeparam>
        /// <returns>The registered instance of the requested subsystem.</returns>
        /// <exception cref="DPL.Ifx.IoC.Contracts.Exceptions.IoCServiceNotFoundException" />
        /// <exception cref="DPL.Ifx.IoC.Contracts.Exceptions.IoCServiceResolutionException" />
        /// <exception cref="NamespaceException" />
        public I ForSubsystem<I>() where I : class, ISubsystem;

        /// <summary>
        /// Request a component from the proxy.
        /// </summary>
        /// <typeparam name="I">The component (engine or accessor) to retrieve.</typeparam>
        /// <returns>The registered instance of the requested service.</returns>
        /// <exception cref="DPL.Ifx.IoC.Contracts.Exceptions.IoCServiceNotFoundException" />
        /// <exception cref="DPL.Ifx.IoC.Contracts.Exceptions.IoCServiceResolutionException" />
        /// <exception cref="NamespaceException" />
        public I ForComponent<I>(object caller) where I : class, IComponent;
    }
}