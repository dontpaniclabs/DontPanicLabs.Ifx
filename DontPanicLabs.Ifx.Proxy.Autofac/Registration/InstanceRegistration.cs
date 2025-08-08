using DontPanicLabs.Ifx.Services.Contracts;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Registration;

/// <summary>
/// Represents a registration of a concrete service instance (e.g., a mock) to be injected wherever
/// the specified service <see cref="Type"/> is requested.
/// Use <see cref="New{TType}(IService)"/> to create an instance.
/// </summary>
public class InstanceRegistration : RegistrationBase
{
    /// <summary>The service interface type.</summary>
    public Type Type { get; }

    /// <summary>The concrete instance that will be supplied by the container.</summary>
    public IService Instance { get; }

    private InstanceRegistration(Type type, IService instance)
    {
        Type = type;
        Instance = instance;
    }

    /// <summary>
    /// Creates a new <see cref="InstanceRegistration"/> for the specified service type.
    /// </summary>
    /// <typeparam name="TType">The service interface.</typeparam>
    /// <param name="instance">The concrete instance to be returned by the container.</param>
    public static InstanceRegistration New<TType>(IService instance)
    {
        return new InstanceRegistration(typeof(TType), instance);
    }
}