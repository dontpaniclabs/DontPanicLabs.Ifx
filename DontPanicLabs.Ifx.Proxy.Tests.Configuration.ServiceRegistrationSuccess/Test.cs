using DontPanicLabs.Ifx.IoC.Contracts;
using DontPanicLabs.Ifx.Manager.Proxy.Tests;
using DontPanicLabs.Ifx.Services.Contracts;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using DontPanicLabs.Ifx.Proxy.Contracts;

namespace DontPanicLabs.Ifx.Proxy.Tests.Configuration.ServiceRegistrationSuccess
{
    [TestClass]
    [TestCategoryCI]
    public class ProxyConfigurationTests
    {
        [TestMethod]
        public void ServiceRegistrationSuccess_FromConfig()
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
    // This child class is needed so that we can override the RegisterFromConfiguration method to use our test IoC container.
    public class ProxyFactory : ProxyFactoryBase, IProxyFactory
    {
        public ProxyFactory()
        {
            Container = new TestContainer(Configuration.ServiceRegistrations);
        }
    }


    public class TestContainer : IContainer
    {
        private readonly Dictionary<Type, Type[]> Services;

        public TestContainer(Dictionary<Type, Type[]> services)
        {
            Services = services;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public TService GetService<TService>() where TService : class
        {
            var generic = typeof(TService);

            var service = Services[generic].First();

            var instance = Activator.CreateInstance(service);


            if (instance is not TService casted)
            {
                throw new NullReferenceException($"Could not create instance of {generic.Name}");
            }

            return casted;
        }
    }

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