namespace DontPanicLabs.Ifx.Proxy.Autofac.Registration;

/// <summary>
/// Defines the lifetime scope of a service.
/// </summary>
public enum LifetimeScope
{
    /// <summary>
    /// The service is created once per request and shared within that request.
    /// </summary>
    Scoped,

    /// <summary>
    /// The service is created once and shared across the entire application.
    /// </summary>
    Singleton,

    /// <summary>
    /// The service is created each time it is requested.
    /// </summary>
    Transient
}