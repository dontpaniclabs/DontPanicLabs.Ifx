namespace DontPanicLabs.Ifx.Proxy.Autofac.Registration;

/// <summary>
/// Collects service registrations that will be fed into a service registrar.
/// The builder can be passed around and modified freely until registration time.
/// </summary>
public class RegistrationBuilder
{
    /// <summary>
    /// Gets the current set of registrations to be applied to the proxy container.
    /// </summary>
    public ICollection<RegistrationBase> Registrations { get; private set; }

    /// <summary>
    /// Creates a new <see cref="RegistrationBuilder"/> seeded with the supplied registrations.
    /// </summary>
    /// <param name="registrations">Optional initial registrations.</param>
    public RegistrationBuilder(params RegistrationBase[] registrations)
    {
        Registrations = registrations;
    }

    /// <summary>
    /// Appends one or more registrations to the existing set.
    /// </summary>
    /// <param name="registrations">The registrations to add.</param>
    public void AddRegistrations(params RegistrationBase[] registrations)
    {
        Registrations = [.. Registrations, .. registrations];
    }

    /// <summary>
    /// Replaces any existing registration for the same service <c>Type</c> with the supplied
    /// <see cref="InstanceRegistration"/>—useful in tests where a real service is swapped for a mock.
    /// </summary>
    /// <param name="registration">The replacement instance registration.</param>
    public void ReplaceRegistration(InstanceRegistration registration)
    {
        var newRegistrations = Registrations
            .Where(reg =>
                (reg is TypeRegistration t && t.Type != registration.Type) ||
                (reg is InstanceRegistration i && i.Type != registration.Type)
            )
            .Concat([registration])
            .ToArray();

        Registrations = newRegistrations;
    }
}