using Autofac;
using Autofac.Core;
using DontPanicLabs.Ifx.IoC.Contracts.Exceptions;
using DontPanicLabs.Ifx.IoC.Tests.Helpers;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContainerBuilder = DontPanicLabs.Ifx.IoC.Autofac.ContainerBuilder;

namespace DontPanicLabs.Ifx.IoC.Tests.Autofac;

[TestClass]
[TestCategoryCI]
public class ContainerTests
{
    [TestMethod]
    public void GetService_ServiceIsInContainer_ShouldReturnService()
    {
        var builder = new ContainerBuilder();

        builder.RegisterServices(options =>
        {
            options
                .RegisterType<TestImplementation>()
                .As<ITestInterface>();
        });

        using var container = builder.Build();

        var service = container.GetService<ITestInterface>();

        Assert.IsInstanceOfType<TestImplementation>(service);
    }

    [TestMethod]
    public void GetService_ServiceFailsToResolve_ShouldThrowIoCServiceResolutionException()
    {
        var builder = new ContainerBuilder();

        builder.RegisterServices(options =>
        {
            options
                .RegisterType<TestImplementation>()
                .OnActivating(_ => throw new StackOverflowException())
                .As<ITestInterface>();
        });


        var exception = Assert.ThrowsException<IoCServiceResolutionException>(() =>
        {
            using var container = builder.Build();
            container.GetService<ITestInterface>();
        });

        Assert.AreEqual("An error occurred while resolving service of type 'ITestInterface'.", exception.Message);
        Assert.IsInstanceOfType<DependencyResolutionException>(exception.InnerException);
    }

    [TestMethod]
    public void GetService_ServiceIsNotInContainer_ShouldThrowIoCServiceNotFoundException()
    {
        var exception = Assert.ThrowsException<IoCServiceNotFoundException>(() =>
        {
            using var container = new ContainerBuilder().Build();
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

        builder.RegisterServices(options =>
        {
            options
                .RegisterType<TestImplementation>()
                .As<ITestInterface>();
        });

        var expectedService = new TestImplementation();

        builder.RegisterServices(options => { options.RegisterInstance<ITestInterface>(expectedService); });

        using var container = builder.Build();

        var actualService = container.GetService<ITestInterface>();

        Assert.AreSame(expectedService, actualService);
    }
}