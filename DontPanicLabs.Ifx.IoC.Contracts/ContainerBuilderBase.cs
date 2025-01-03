using DontPanicLabs.Ifx.IoC.Contracts.Exceptions;

namespace DontPanicLabs.Ifx.IoC.Contracts;

/// <summary>
/// Serves as a base class for creating various IoC container builders with a shared pattern for 
/// registering services and building the final container. This class provides the common logic 
/// for aggregating configuration options and constructing a container builder of type 
/// <typeparamref name="TContainerBuilder"/>. Derived classes must implement <see cref="Build"/> to 
/// finalize and return a concrete IoC container (e.g., Autofac, ServiceCollection).
/// </summary>
/// <typeparam name="TContainerBuilder">
/// The type of the builder used to configure the IoC container. Must have a parameterless constructor.
/// </typeparam>
/// <remarks>
/// <para>
/// The <see cref="RegisterServices"/> method allows callers to register additional configuration 
/// callbacks (e.g., adding services, configuring modules). These are accumulated in a list of 
/// actions that the base class applies when <see cref="CombineBuilderOptions"/> is called.
/// </para>
/// <para>
/// Implementers should override <see cref="Build"/> to instantiate and return the final container. 
/// The <see cref="CombineBuilderOptions"/> method should be used in the <c>Build</c> override to 
/// create and configure an instance of <typeparamref name="TContainerBuilder"/> before producing 
/// the concrete container.
/// </para>
/// <para>
/// By delegating the underlying container creation to derived classes, this base class enables
/// different container implementations (e.g., Autofac, Microsoft DI) to share a consistent
/// registration pattern while maintaining flexibility for specific container features.
/// </para>
/// </remarks>
public abstract class ContainerBuilderBase<TContainerBuilder> : IContainerBuilder<TContainerBuilder>
    where TContainerBuilder : class, new()
{
    public abstract IContainer Build();

    private readonly List<Action<TContainerBuilder>> _options = [];

    protected TContainerBuilder CombineBuilderOptions()
    {
        try
        {
            var builder = new TContainerBuilder();

            foreach (var options in _options)
            {
                options(builder);
            }

            return builder;
        }
        catch (Exception ex)
        {
            throw new IoCContainerBuildException(
                "An error occurred while configuring your service container. Check the inner exception for more details.",
                ex
            );
        }
    }

    public void RegisterServices(Action<TContainerBuilder> options)
    {
        _options.Add(options);
    }
}