using DontPanicLabs.Ifx.Services.Contracts;
using DontPanicLabs.Ifx.Proxy.Contracts.Proxy;

namespace DontPanicLabs.Ifx.Proxy.Contracts.Service;

public abstract class ProxyEnabledServiceBase : ServiceBase, IProxyEnabledService
{
    /// <summary>
    /// Gets or sets the proxy generator used to create proxies for services.
    /// This property is typically set automatically when the service is resolved through a proxy generator,
    /// but it may also be set manually for testing purposes.
    /// </summary>
    /// <remarks>
    /// This property is explicitly implemented to make direct access more intentional.
    /// In most cases, you should use the protected helper method provided by <see cref="ProxyEnabledServiceBase"/>
    /// instead of accessing this property directly.
    /// </remarks>
    IProxyGenerator IProxyEnabledService.ServiceProxyGenerator { get; set; } = null!;

    /// <summary>
    /// Creates a proxy instance for the specified service type, enabling interception and contextual behavior.
    /// </summary>
    /// <typeparam name="TService">The service interface to proxy. Must be a class that implements <see cref="IService"/>.</typeparam>
    /// <returns>A proxied instance of the specified service type.</returns>
    protected TService ProxyForService<TService>() where TService : class, IService
    {
        var service = ((IProxyEnabledService) this).ServiceProxyGenerator.ProxyForService<TService>();

        return service;
    }
}