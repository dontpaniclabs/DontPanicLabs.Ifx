using DontPanicLabs.Ifx.IoC.Contracts;
using DontPanicLabs.Ifx.IoC.Contracts.Exceptions;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DontPanicLabs.Ifx.IoC.Tests.Contracts;

[TestClass]
[TestCategoryCI]
public class ContainerBuilderBaseTests
{
    [TestMethod]
    public void CombineBuilderOptions_ConfigurationThrowsException_ShouldRethrowInIoCContainerBuildException()
    {
        var builder = new DerivedContainerBuilder();

        builder.RegisterServices(options => { options.AddScoped(); });

        var exception = Assert.ThrowsException<IoCContainerBuildException>(() => builder.Build());

        Assert.IsInstanceOfType<IoCContainerBuildException>(exception);
        Assert.IsInstanceOfType<InvalidOperationException>(exception.InnerException);

        Assert.AreEqual(
            "An error occurred while configuring your service container. " +
            "Check the inner exception for more details.",
            exception.Message
        );
        
        Assert.AreEqual("That's not how you register a service.", exception.InnerException.Message);
    }
}

public class DerivedContainerBuilder : ContainerBuilderBase<TestContainerBuilder>
{
    public override IContainer Build()
    {
        CombineBuilderOptions();

        // CombineBuilderOptions will throw in the test, so this code should be unreachable.
        throw new NotImplementedException();
    }
}

public class TestContainerBuilder
{
    public void AddScoped() => throw new InvalidOperationException("That's not how you register a service.");
}