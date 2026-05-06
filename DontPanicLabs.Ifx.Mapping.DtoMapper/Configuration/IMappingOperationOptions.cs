namespace DontPanicLabs.Ifx.Mapping.DtoMapper;
using StringDictionary = Dictionary<string, object>;
using DontPanicLabs.Ifx.Mapping.Contracts;
public sealed class MappingOperationOptions<TSource, TDestination>(Func<Type, object> serviceCtor) : IMappingOperationOptions<TSource, TDestination>
{
    public Func<Type, object> ServiceCtor { get; private set; } = serviceCtor;
    public StringDictionary Items => (StringDictionary) (State ??= new StringDictionary());
    public object State { get; set; }
    public Action<TSource, TDestination> BeforeMapAction { get; private set; }
    public Action<TSource, TDestination> AfterMapAction { get; private set; }
    public void BeforeMap(Action<TSource, TDestination> beforeFunction) => BeforeMapAction = beforeFunction;
    public void AfterMap(Action<TSource, TDestination> afterFunction) => AfterMapAction = afterFunction;
    public void ConstructServicesUsing(Func<Type, object> constructor)
    {
        var ctor = ServiceCtor;
        ServiceCtor = t => constructor(t) ?? ctor(t);
    }
    void IMappingOperationOptions.BeforeMap(Action<object, object> beforeFunction) => BeforeMapAction = (s, d) => beforeFunction(s, d);
    void IMappingOperationOptions.AfterMap(Action<object, object> afterFunction) => AfterMapAction = (s, d) => afterFunction(s, d);
}