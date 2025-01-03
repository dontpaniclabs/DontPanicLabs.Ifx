using DontPanicLabs.Ifx.IoC.Contracts;

namespace DontPanicLabs.Ifx.IoC.Dotnet.ServiceCollection;

public class ContainerBuilder : ContainerBuilderBase<MicrosoftServiceCollection>
{
    public override IContainer Build()
    {
        var serviceCollection = CombineBuilderOptions();

        return new Container(serviceCollection);
    }
}