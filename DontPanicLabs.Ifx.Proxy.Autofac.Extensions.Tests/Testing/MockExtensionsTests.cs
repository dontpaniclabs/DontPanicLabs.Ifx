using DontPanicLabs.Ifx.Services.Contracts;
using DontPanicLabs.Ifx.Proxy.Contracts.Service;
using DontPanicLabs.Ifx.Proxy.Autofac.Extensions.Testing;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using Moq;
using Shouldly;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Extensions.Tests.Testing;

[TestClass]
[TestCategoryCI]
public class MockExtensionsTests
{
    [TestMethod]
    public void MockDependency_ProxyEnabledServiceBase_NewService_ProxyGeneratorIsMocked()
    {
        var service = new TestProxyEnabledService();

        ((IProxyEnabledService)service).ServiceProxyGenerator.ShouldBeNull();

        _ = service.MockDependency<ITestUtility>();

        _ = ((IProxyEnabledService)service).ServiceProxyGenerator.ShouldBeOfType<MockProxyGenerator>();
    }

    [TestMethod]
    public void MockDependency_ProxyEnabledServiceBase_NewService_DependencyIsMocked()
    {
        var service = new TestProxyEnabledService();
        var mockedUtility = service.MockDependency<ITestUtility>().Object;
        var proxiedUtility = ((IProxyEnabledService)service).ServiceProxyGenerator.ProxyForService<ITestUtility>();

        mockedUtility.ShouldBeSameAs(proxiedUtility);
    }

    [TestMethod]
    public void MockDependency_ProxyEnabledServiceBase_NewService_MockingCanBeChained()
    {
        var service = new TestProxyEnabledService();

        _ = service
            .MockDependency<ITestService>(out var mockedService)
            .MockDependency<ITestUtility>(out var mockedUtility);

        _ = mockedService.ShouldBeOfType<Mock<ITestService>>();
        _ = mockedUtility.ShouldBeOfType<Mock<ITestUtility>>();
    }

    [TestMethod]
    public void MockDependency_ProxyEnabledServiceBase_ServiceHasIncompatibleProxyGenerator_ShouldThrow()
    {
        var service = new TestProxyEnabledService();
        var incompatibleProxyGenerator = new Mock<DontPanicLabs.Ifx.Proxy.Contracts.Proxy.IProxyGenerator>();
        ((IProxyEnabledService)service).ServiceProxyGenerator = incompatibleProxyGenerator.Object;

        var exception =
            Should.Throw<InvalidOperationException>(() => service.MockDependency<ITestUtility>());

        exception.Message.ShouldContain("'ServiceProxyGenerator' already set to a");
        exception.Message.ShouldContain("'MockDependency' can only be called when the proxy generator is unset or set to a 'MockProxyGenerator'");
    }

    public interface ITestService : IService
    {
        void DoSomething();
    }

    public interface ITestUtility : IUtility
    {
        void DoSomething();
    }

    public class TestProxyEnabledService : ProxyEnabledServiceBase, IService
    {
        public void DoWork()
        {
            // Test implementation
        }
    }
}