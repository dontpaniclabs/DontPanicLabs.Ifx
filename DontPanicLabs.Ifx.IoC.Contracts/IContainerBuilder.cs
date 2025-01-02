namespace DontPanicLabs.Ifx.IoC.Contracts;

/// <summary>
/// Defines a contract for building a dependency injection container.
/// </summary>
/// <remarks>
/// The builder is responsible for configuring and constructing the container. It includes methods for registering services
/// and building the container. Implementations should ensure that the container is properly configured before building.
/// </remarks>
public interface IContainerBuilder<out TContainerBuilder> where TContainerBuilder : class, new()
{
    /// <summary>
    /// Builds the container of registered services.
    /// </summary>
    /// <returns>The container of registered services.</returns>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown if an error occurs during the build process.
    /// </exception>
    IContainer Build();

    /// <summary>
    /// Registers services with the container builder. This method is additive, so you can call
    /// it multiple times to register multiple services, however the last registration will take
    /// precedence.
    /// </summary>
    /// <param name="options">The container builder to register services with.</param>
    public void RegisterServices(Action<TContainerBuilder> options);
}