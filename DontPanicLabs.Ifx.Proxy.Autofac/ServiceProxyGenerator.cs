using Autofac;
using DontPanicLabs.Ifx.Services.Contracts;
using DontPanicLabs.Ifx.Proxy.Contracts.Context;
using DontPanicLabs.Ifx.Proxy.Contracts.Exceptions;
using DontPanicLabs.Ifx.Proxy.Contracts.Service;
using DontPanicLabs.Ifx.Proxy.Contracts.Proxy;

namespace DontPanicLabs.Ifx.Proxy.Autofac;

/// <summary>
/// Generates context-aware proxies for service implementations, enabling interception and dependency resolution
/// within a request or operation scope.
/// </summary>
public sealed class ServiceProxyGenerator : IProxyGenerator
{
    public RequestContextBase RequestContext { get; }

    private readonly ILifetimeScope _containerScope;

    public ServiceProxyGenerator(RequestContextBase context)
    {
        RequestContext = context;
        _containerScope = ServiceRegistrar.CreateScopedContainer();
    }

    public TService ProxyForService<TService>() where TService : class, IService
    {
        var type = typeof(TService);

        var service = type switch
        {
            _ when typeof(IComponent).IsAssignableFrom(type) ||
                   typeof(IProxyEnabledComponent).IsAssignableFrom(type) => ProxyForComponent<TService>(),

            _ when typeof(ISubsystem).IsAssignableFrom(type) ||
                   typeof(IProxyEnabledSubsystem).IsAssignableFrom(type) => ProxyForSubsystem<TService>(),

            _ when typeof(IUtility).IsAssignableFrom(type) ||
                   typeof(IProxyEnabledUtility).IsAssignableFrom(type) => ProxyForUtility<TService>(),
            _ => throw new NotImplementedException()
        };

        return service;
    }

    private TComponent ProxyForComponent<TComponent>() where TComponent : class, IService
    {
        var component = Resolve<TComponent>();

        return component;
    }

    private TSubsystem ProxyForSubsystem<TSubsystem>() where TSubsystem : class, IService
    {
        var component = Resolve<TSubsystem>();

        return component;
    }

    private TUtility ProxyForUtility<TUtility>() where TUtility : class, IService
    {
        var utility = Resolve<TUtility>();

        return utility;
    }

    private TService Resolve<TService>() where TService : class, IService
    {
        if (!_containerScope.TryResolve(out TService? service))
        {
            throw new ProxyException(
                $"Service of type '{typeof(TService).Name}' was not found. " +
                "Did you forget to register it?"
            );
        }

        if (service is IProxyEnabledService proxiedService)
        {
            proxiedService.ServiceProxyGenerator = this;
        }

        return service;
    }

    public void Dispose()
    {
        _containerScope.Dispose();
    }
}