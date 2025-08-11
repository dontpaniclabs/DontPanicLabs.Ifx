using DontPanicLabs.Ifx.Services.Contracts;
using DontPanicLabs.Ifx.Proxy.Contracts.Context;
using DontPanicLabs.Ifx.Proxy.Contracts.Proxy;
using Moq;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Extensions.Testing;

/// <summary>
/// A test-only implementation of <see cref="IProxyGenerator"/> that returns explicitly registered mocks
/// instead of real or proxied service implementations.
/// </summary>
/// <remarks>
/// Unlike proxy-based generators, this class does not dynamically create service proxies. Instead, each
/// service must be mocked manually using <see cref="MockService{TService}"/> before being retrieved via
/// <see cref="ProxyForService{TService}"/>.
/// <para>
/// This class is intended as a drop-in replacement for proxy generators in unit tests,
/// allowing services under test to receive <see cref="Mock{T}"/> instances for their dependencies.
/// </para>
/// </remarks>
public class MockProxyGenerator : IProxyGenerator
{
    private readonly Dictionary<Type, IService> _services = [];

    public RequestContextBase RequestContext { get; } = new MockProxyContext();

    /// <summary>
    /// Creates and registers a new mock for the specified service type, which will be returned
    /// on subsequent calls to <see cref="ProxyForService{TService}"/>.
    /// </summary>
    /// <typeparam name="TService">The service interface to mock. Must implement <see cref="IService"/>.</typeparam>
    /// <returns>The created <see cref="Mock{T}"/> instance, which can be configured as needed in the test.</returns>
    public Mock<TService> MockService<TService>() where TService : class, IService
    {
        var mock = new Mock<TService>();

        _services[typeof(TService)] = mock.Object;

        return mock;
    }

    public TService ProxyForService<TService>() where TService : class, IService
    {
        if (_services.TryGetValue(typeof(TService), out var service))
        {
            return (TService) service;
        }

        throw new InvalidOperationException(
            $"Service '{typeof(TService).Name}' not found in mock proxy generator. " +
            "Ensure that the service has been mocked before attempting to retrieve it."
        );
    }

    public void Dispose()
    {
    }
}