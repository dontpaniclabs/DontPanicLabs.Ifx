using DontPanicLabs.Ifx.Services.Contracts;
using DontPanicLabs.Ifx.Proxy.Contracts.Service;
using Moq;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Extensions.Testing;

public static class MockExtensions
{
    /// <summary>
    /// Mocks a dependency of the given <see cref="IProxyEnabledService"/> by registering a mock proxy for a service
    /// interface. This is intended for use in **unit tests**, where concrete service implementations are
    /// being tested in isolation with mocked dependencies.
    /// </summary>
    /// <remarks>
    /// This method sets the <see cref="IProxyEnabledService.ServiceProxyGenerator"/> to a <see cref="MockProxyGenerator"/> if it has
    /// not already been set. If it is already set to a different proxy generator, an exception is thrown to
    /// prevent conflicting test configurations.
    /// </remarks>
    public static Mock<TService> MockDependency<TService>(this IProxyEnabledService service)
        where TService : class, IService
    {
        var proxyGenerator = service.ServiceProxyGenerator;

        if (proxyGenerator is not null && proxyGenerator is not MockProxyGenerator)
        {
            throw new InvalidOperationException(
                $"'{nameof(IProxyEnabledService.ServiceProxyGenerator)}' already set to a '{proxyGenerator.GetType().Name}'. " +
                $"'{nameof(MockDependency)}' can only be called when the proxy generator is unset or set to a '{nameof(MockProxyGenerator)}'."
            );
        }

        MockProxyGenerator mockProxyGenerator;

        if (proxyGenerator is null)
        {
            mockProxyGenerator = new MockProxyGenerator();
            service.ServiceProxyGenerator = mockProxyGenerator;
        }
        else
        {
            mockProxyGenerator = (MockProxyGenerator) proxyGenerator;
        }

        return mockProxyGenerator.MockService<TService>();
    }

    /// <summary>
    /// Mocks a dependency of the given <see cref="ProxyEnabledServiceBase"/> by registering a mock proxy for a service
    /// interface. This overload enables method chaining by returning the original service instance while
    /// exposing the created mock via an <c>out</c> parameter.
    /// </summary>
    /// <remarks>
    /// This method sets the <see cref="IProxyEnabledService.ServiceProxyGenerator"/> to a <see cref="MockProxyGenerator"/> if it has
    /// not already been set. If it is already set to a different proxy generator, an exception is thrown to
    /// prevent conflicting test configurations.
    /// </remarks>
    public static ProxyEnabledServiceBase MockDependency<TService>(
        this ProxyEnabledServiceBase service,
        out Mock<TService> mock
    )
        where TService : class, IService
    {
        mock = service.MockDependency<TService>();

        return service;
    }
}