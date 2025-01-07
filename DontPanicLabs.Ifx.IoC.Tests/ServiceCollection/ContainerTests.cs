using DontPanicLabs.Ifx.IoC.Contracts.Exceptions;
using DontPanicLabs.Ifx.IoC.Tests.Helpers;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContainerBuilder = DontPanicLabs.Ifx.IoC.Dotnet.ServiceCollection.ContainerBuilder;

namespace DontPanicLabs.Ifx.IoC.Tests.ServiceCollection;

[TestClass]
[TestCategoryCI]
public class ContainerTests
{
    [TestMethod]
    public void GetService_ServiceIsInContainer_ShouldReturnService()
    {
        var builder = new ContainerBuilder();

        builder.RegisterServices(options => { options.AddScoped<ITestInterface, TestImplementation>(); });

        using var container = builder.Build();

        var service = container.GetService<ITestInterface>();

        Assert.IsInstanceOfType<TestImplementation>(service);
    }

    [TestMethod]
    public void GetService_ServiceIsNotInContainer_ShouldThrowIoCServiceNotFoundException()
    {
        var container = new ContainerBuilder().Build();

        var exception = Assert.ThrowsException<IoCServiceNotFoundException>(() =>
        {
            container.GetService<ITestInterface>();
        });

        Assert.AreEqual(
            "Service of type 'ITestInterface' was not registered. " +
            "Did you forget to add it to the container?",
            exception.Message
        );
    }

    [TestMethod]
    public void GetService_ServiceIsRegisteredMultipleTimes_ShouldReturnLastRegisteredService()
    {
        var builder = new ContainerBuilder();

        builder.RegisterServices(options => { options.AddScoped<ITestInterface, TestImplementation>(); });

        var expectedService = new TestImplementation();

        builder.RegisterServices(options => { options.AddScoped<ITestInterface>(_ => expectedService); });

        using var container = builder.Build();

        var actualService = container.GetService<ITestInterface>();

        Assert.AreSame(expectedService, actualService);
    }
}