namespace DontPanicLabs.Ifx.Mapping.DtoMapper;

using DontPanicLabs.Ifx.Mapping.Contracts;
using IObjectMappingOperationOptions = DontPanicLabs.Ifx.Mapping.Contracts.IMappingOperationOptions<object, object>;
using Factory = Func<Type, object>;
internal interface IInternalRuntimeMapper : IRuntimeMapper
{
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination, ResolutionContext context, Type sourceType = null, Type destinationType = null, MemberMap memberMap = null);
    ResolutionContext DefaultContext { get; }
    Factory ServiceCtor { get; }
}
public sealed class Mapper : IMapper, IInternalRuntimeMapper
{
    private readonly IGlobalConfiguration _configuration;
    private readonly Factory _serviceCtor;
    private readonly ResolutionContext _defaultContext;
    public Mapper(IConfigurationProvider configuration) : this(configuration, configuration.Internal().ServiceCtor) { }
    public Mapper(IConfigurationProvider configuration, Factory serviceCtor)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(serviceCtor);
        _configuration = (IGlobalConfiguration)configuration;
        _serviceCtor = serviceCtor;
        _defaultContext = new(this);
    }
    ResolutionContext IInternalRuntimeMapper.DefaultContext => _defaultContext;
    Factory IInternalRuntimeMapper.ServiceCtor => _serviceCtor;
    public IConfigurationProvider ConfigurationProvider => _configuration;
    public TDestination Map<TDestination>(object source) => Map(source, default(TDestination));
    public TDestination Map<TDestination>(object source, Action<IMappingOperationOptions<object, TDestination>> opts) => Map(source, default, opts);
    public TDestination Map<TSource, TDestination>(TSource source) => Map(source, default(TDestination));
    public TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> opts) =>
        Map(source, default, opts);
    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination) =>
        MapCore(source, destination, _defaultContext);
    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> opts) =>
        MapWithOptions(source, destination, opts);
    public object Map(object source, Type sourceType, Type destinationType) => Map(source, null, sourceType, destinationType);
    public object Map(object source, Type sourceType, Type destinationType, Action<IObjectMappingOperationOptions> opts) =>
        Map(source, null, sourceType, destinationType, opts);
    public object Map(object source, object destination, Type sourceType, Type destinationType) =>
        MapCore(source, destination, _defaultContext, sourceType, destinationType);
    public object Map(object source, object destination, Type sourceType, Type destinationType, Action<IObjectMappingOperationOptions> opts) =>
        MapWithOptions(source, destination, opts, sourceType, destinationType);
    public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source, object parameters, params Expression<Func<TDestination, object>>[] membersToExpand)
        => source.ProjectTo(ConfigurationProvider, parameters, membersToExpand);
    public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source, IDictionary<string, object> parameters, params string[] membersToExpand)
        => source.ProjectTo<TDestination>(ConfigurationProvider, parameters, membersToExpand);
    public IQueryable ProjectTo(IQueryable source, Type destinationType, IDictionary<string, object> parameters, params string[] membersToExpand)
        => source.ProjectTo(destinationType, ConfigurationProvider, parameters, membersToExpand);
    TDestination IInternalRuntimeMapper.Map<TSource, TDestination>(TSource source, TDestination destination,
        ResolutionContext context, Type sourceType, Type destinationType, MemberMap memberMap) =>
        MapCore(source, destination, context, sourceType, destinationType, memberMap);
    private TDestination MapWithOptions<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> opts,
        Type sourceType = null, Type destinationType = null)
    {
        MappingOperationOptions<TSource, TDestination> typedOptions = new(_serviceCtor);
        opts(typedOptions);
        typedOptions.BeforeMapAction?.Invoke(source, destination);
        destination = MapCore(source, destination, new(this, typedOptions), sourceType, destinationType);
        typedOptions.AfterMapAction?.Invoke(source, destination);
        return destination;
    }
    private TDestination MapCore<TSource, TDestination>(
        TSource source, TDestination destination, ResolutionContext context, Type sourceType = null, Type destinationType = null, MemberMap memberMap = null)
    {
        TypePair requestedTypes = new(typeof(TSource), typeof(TDestination));
        TypePair runtimeTypes = new(source?.GetType() ?? sourceType ?? typeof(TSource), destination?.GetType() ?? destinationType ?? typeof(TDestination));
        MapRequest mapRequest = new(requestedTypes, runtimeTypes, memberMap);
        return _configuration.GetExecutionPlan<TSource, TDestination>(mapRequest)(source, destination, context);
    }
}