using DontPanicLabs.Ifx.IoC.Contracts;

namespace DontPanicLabs.Ifx.IoC.Autofac;

public class ContainerBuilder : ContainerBuilderBase<AutofacContainerBuilder>
{
    public override IContainer Build()
    {
        var builder = CombineBuilderOptions();

        return new Container(builder.Build());
    }
}