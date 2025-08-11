using DontPanicLabs.Ifx.Services.Contracts;
using DontPanicLabs.Ifx.Proxy.Autofac.Extensions.Testing;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using Moq;
using Shouldly;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Extensions.Tests.Testing;

[TestClass]
[TestCategoryCI]
public class MockProxyGeneratorTests
{
    [TestMethod]
    public void MockProxyGenerator_MockService_DuplicateServicesGetOverwritten()
    {
        var proxyGenerator = new MockProxyGenerator();

        var mock1 = proxyGenerator.MockService<ITestService>();
        var mock2 = proxyGenerator.MockService<ITestService>();

        mock1.ShouldNotBeSameAs(mock2);

        var service = proxyGenerator.ProxyForService<ITestService>();

        service.ShouldBeSameAs(mock2.Object);
    }

    [TestMethod]
    public void MockProxyGenerator_ProxyFor_ReturnsMockedServices()
    {
        var proxyGenerator = new MockProxyGenerator();

        _ = proxyGenerator.MockService<ITestService>();
        _ = proxyGenerator.MockService<ITestUtility>();

        var service = proxyGenerator.ProxyForService<ITestService>();
        var utility = proxyGenerator.ProxyForService<ITestUtility>();

        _ = Mock.Get(service).ShouldBeOfType<Mock<ITestService>>();
        _ = Mock.Get(utility).ShouldBeOfType<Mock<ITestUtility>>();
    }

    [TestMethod]
    public void MockProxyGenerator_ProxyFor_ServiceNotMocked_ShouldThrow()
    {
        var proxyGenerator = new MockProxyGenerator();

        var exception = Should.Throw<InvalidOperationException>(() =>
            proxyGenerator.ProxyForService<ITestService>()
        );

        exception.Message.ShouldBe(
            "Service 'ITestService' not found in mock proxy generator. " +
            "Ensure that the service has been mocked before attempting to retrieve it."
        );
    }

    public interface ITestService : IService
    {
        void DoSomething();
    }

    public interface ITestUtility : IUtility
    {
        void DoSomething();
    }
}