using System.Dynamic;

namespace DontPanicLabs.Ifx.Mapping.DtoMapper.Tests.Bug;

public class MapExpandoObjectProperty : AutoMapperSpecBase
{

    class From
    {
        public ExpandoObject ExpandoObject { get; set; }
    }

    class To
    {
        public ExpandoObject ExpandoObject { get; set; }
    }

    protected override MapperConfiguration CreateConfiguration() => new(cfg =>
    {
        cfg.CreateMap<From, To>();
    });
    [Fact]
    public void Should_work()
    {
        dynamic baseSettings = new ExpandoObject();
        var settings = Mapper.Map<To>(new From { ExpandoObject = baseSettings});
    }
}