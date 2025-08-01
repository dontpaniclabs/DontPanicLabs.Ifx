using DontPanicLabs.Ifx.Proxy.Contracts.Context;
using DontPanicLabs.Ifx.Services.Contracts;

namespace DontPanicLabs.Ifx.Proxy.Contracts.Proxy;

/// <summary>
/// Provides methods for creating context-aware proxies for services within the request lifecycle,
/// and manages any disposable resources tied to that context.
/// </summary>
public interface IProxyGenerator : IDisposable
{
    /// <summary>
    /// Gets the context associated with the current request.
    /// </summary>
    RequestContextBase RequestContext { get; }

    /// <summary>
    /// Creates a proxy instance for the specified service type, enabling interception and contextual behavior.
    /// </summary>
    /// <typeparam name="TService">The service interface to proxy. Must implement <see cref="IService"/>.</typeparam>
    /// <returns>A proxied instance of the specified service type.</returns>
    TService ProxyForService<TService>() where TService : class, IService;
}