using DontPanicLabs.Ifx.Manager.Proxy.Tests;
using DontPanicLabs.Ifx.Services.Contracts;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using DontPanicLabs.Ifx.Proxy.Contracts;
using DontPanicLabs.Ifx.Proxy.Autofac;

namespace DontPanicLabs.Ifx.Proxy.Tests.AutoDiscovery.ServiceRegistrationSuccess
{
    [TestClass]
    [TestCategoryCI]
    public class ProxyAutoDiscoveryTests
    {
        [TestMethod]
        public void ServiceRegistrationSuccess_FromAutoDiscovery()
        {
            var instance = Proxy.ForSubsystem<ITestSubsystem>();

            Assert.IsInstanceOfType<ITestSubsystem>(instance);

            Assert.IsInstanceOfType<TestSubsystem>(instance);
        }
    }
}

namespace DontPanicLabs.Ifx.Manager.Proxy.Tests
{
    public class TestSubsystem : ITestSubsystem;

    public interface ITestSubsystem : ISubsystem
    {
        public void Test() 
        {
            var debug = true;

            _ = debug;
        }
    }

}
namespace DontPanicLabs.Ifx.Proxy.Tests
{
    public static class Proxy
    {
        private static readonly IProxyFactory factory;

        static Proxy()
        {
            factory = new ProxyFactory();
        }

        public static I ForSubsystem<I>() where I : class, ISubsystem
        {
            return factory.ForSubsystem<I>();
        }

        public static I ForComponent<I>(object caller) where I : class, IComponent
        {
            return factory.ForComponent<I>(caller);
        }
    }
}
