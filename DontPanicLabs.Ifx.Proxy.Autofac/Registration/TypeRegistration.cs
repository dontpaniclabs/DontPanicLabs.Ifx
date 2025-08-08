using Castle.DynamicProxy;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Registration;

/// <summary>
/// Represents a registration that maps a service interface to an implementing type,
/// with optional interceptors and lifetime scope.
/// Use <see cref="New{TType, TImplementation}"/> to create an instance.
/// </summary>
public class TypeRegistration : RegistrationBase
{
    /// <summary>The service interface type.</summary>
    public Type Type { get; }

    /// <summary>The concrete type that implements the service.</summary>
    public Type Implementation { get; }

    /// <summary>The desired lifetime scope for the service.</summary>
    public LifetimeScope LifetimeScope { get; }

    /// <summary>
    /// Zero or more interceptors to be applied when the service is resolved through a proxy.
    /// </summary>
    public IInterceptor[] Interceptors { get; }

    private TypeRegistration(
        Type type,
        Type implementation,
        LifetimeScope lifetimeScope = LifetimeScope.Transient,
        params IInterceptor[] interceptors
    )
    {
        Type = type;
        Implementation = implementation;
        LifetimeScope = lifetimeScope;
        Interceptors = interceptors;
    }

    /// <summary>
    /// Creates a new <see cref="TypeRegistration"/> mapping <typeparamref name="TType"/> to
    /// <typeparamref name="TImplementation"/>.
    /// </summary>
    /// <typeparam name="TType">The service interface or base type.</typeparam>
    /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
    /// <param name="lifetimeScope">
    /// The lifetime scope of the registration (Transient, Scoped, or Singleton). Defaults to
    /// <see cref="LifetimeScope.Transient"/>.
    /// </param>
    /// <param name="interceptors">
    /// Optional interceptors that will wrap the service when resolved through a proxy.
    /// </param>
    public static TypeRegistration New<TType, TImplementation>(
        LifetimeScope lifetimeScope = LifetimeScope.Transient,
        params IInterceptor[] interceptors
    )
    {
        return new TypeRegistration(typeof(TType), typeof(TImplementation), lifetimeScope, interceptors);
    }
}