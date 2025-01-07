using DontPanicLabs.Ifx.Manager.Proxy.Tests;
using DontPanicLabs.Ifx.Proxy.Contracts;
using DontPanicLabs.Ifx.Services.Contracts;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DontPanicLabs.Ifx.Proxy.Tests.AutoDiscovery.ServiceRegistrationSuccess
{
    [TestClass]
    [TestCategoryCI]
    public class AutoDiscoveryTests
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
        public void Test() { }
    }

}
namespace DontPanicLabs.Ifx.Proxy.Tests
{
    public static class Proxy
    {
        private static readonly IProxy factory;

        static Proxy()
        {
            factory = new Autofac.ProxyFactory();
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