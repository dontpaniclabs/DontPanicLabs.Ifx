using System.Diagnostics;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using DontPanicLabs.Ifx.Proxy.Contracts.Exceptions;
using DontPanicLabs.Ifx.Proxy.Autofac.Registration;

namespace DontPanicLabs.Ifx.Proxy.Autofac;

/// <summary>
/// Manages the global service container used by <see cref="IProxyGenerator"/> implementations.
/// Call <see cref="RegisterServices"/> once during application startup to configure the container
/// with the required service registrations.
/// </summary>
public static class ServiceRegistrar
{
    private static ContainerBuilder _ContainerBuilder = null!;

    private static IContainer? _Container;

    /// <summary>
    /// Builds the proxy container from the supplied <paramref name="builder"/> and makes it globally
    /// available for proxy generation. Must be called exactly once during application startup.
    /// </summary>
    /// <param name="builder">
    /// A fully-configured <see cref="RegistrationBuilder"/> containing all <see cref="TypeRegistration"/>
    /// and <see cref="InstanceRegistration"/> entries that define the application's service graph.
    /// </param>
    /// <exception cref="ProxyException">
    /// Thrown if <see cref="RegisterServices"/> is called more than once. without first calling <see cref="Reset"/>.
    /// </exception>
    public static void RegisterServices(RegistrationBuilder builder)
    {
        Debug.Assert(
            _Container is null,
            $"'{nameof(RegisterServices)}' should only be called once and before any services are resolved. " +
            $"If you need different registrations between tests, then call '{nameof(Reset)}'."
        );

        ProxyException.ThrowIfTrue(
            _Container is not null,
            $"'{nameof(RegisterServices)}' should only be called once and before any services are resolved."
        );

        _ContainerBuilder = new ContainerBuilder();

        RegisterTypes(builder.Registrations.OfType<TypeRegistration>().ToArray());
        RegisterInstances(builder.Registrations.OfType<InstanceRegistration>().ToArray());

        _Container = _ContainerBuilder.Build();
    }

    private static void RegisterTypes(TypeRegistration[] registrations)
    {
        foreach (var registration in registrations)
        {
            var builder = _ContainerBuilder
                .RegisterType(registration.Implementation)
                .As(registration.Type);

            builder = registration.LifetimeScope switch
            {
                LifetimeScope.Scoped => builder.InstancePerLifetimeScope(),
                LifetimeScope.Singleton => builder.SingleInstance(),
                LifetimeScope.Transient => builder.InstancePerDependency(),
                _ => throw new NotImplementedException(
                    $"LifetimeScope '{registration.LifetimeScope}' is not implemented."
                )
            };

            if (!registration.Interceptors.Any())
            {
                continue;
            }

            _ = builder.EnableInterfaceInterceptors();

            foreach (var interceptor in registration.Interceptors)
            {
                var interceptorKey = $"{registration.Type.Name}:{interceptor.GetType().Name}";

                _ = builder.InterceptedBy(interceptorKey);
                _ = _ContainerBuilder.Register(_ => interceptor).Named<IInterceptor>(interceptorKey);
            }
        }
    }

    private static void RegisterInstances(InstanceRegistration[] registrations)
    {
        foreach (var registration in registrations)
        {
            _ = _ContainerBuilder.RegisterInstance(registration.Instance).As(registration.Type);
        }
    }

    internal static ILifetimeScope CreateScopedContainer()
    {
        if (_Container is null)
        {
            throw new ProxyException(
                "No services have been registered. " +
                $"Did you forget to call '{nameof(ServiceRegistrar)}.{nameof(RegisterServices)}'?"
            );
        }

        return _Container.BeginLifetimeScope();
    }

    /// <summary>
    /// This resets the static container and builder used by the registrar. This is meant to be used
    /// between tests that need different service registrations.
    /// </summary>
    public static void Reset()
    {
        _Container?.Dispose();
        _Container = null;
        _ContainerBuilder = null!;
    }
}
