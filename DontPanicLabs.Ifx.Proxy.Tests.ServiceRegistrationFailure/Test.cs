using DontPanicLabs.Ifx.IoC.Contracts;
using DontPanicLabs.Ifx.Proxy.Contracts;
using DontPanicLabs.Ifx.Proxy.Contracts.Exceptions;
using DontPanicLabs.Ifx.Services.Contracts;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DontPanicLabs.Ifx.Proxy.Tests.ServiceRegistrationFailure
{
    [TestClass]
    [TestCategoryCI]
    public class ProxyServiceRegistrationFailureTests
    {
        [TestMethod]
        public void ServiceRegistrationFailure_Config_ServiceNotFound()
        {
            // This exception is caused by a bad namespace in the appsettings.json file of this project.
            var ex = Assert.ThrowsException<TypeInitializationException>(() => Proxy.ForSubsystem<ITestSubsystem>().Test());
            Assert.IsInstanceOfType<ProxyTypeLoadException>(ex.InnerException);
        }
    }
}

namespace DontPanicLabs.Ifx.Proxy.Tests
{
    public class TestSubsystem : ITestSubsystem;

    public interface ITestSubsystem : ISubsystem
    {
        public void Test() { }
    }

    public class ProxyFactory() : ProxyFactoryBase()
    {
        protected override IContainer RegisterFromAutoDiscover()
        {
            throw new NotImplementedException();
        }

        protected override IContainer RegisterFromConfiguration(Dictionary<Type, Type[]> serviceTypes)
        {
            // The test should never get this far.  We should get a proxy exception just parsing the types from config.
            throw new NotImplementedException();
        }
    }

    public static class Proxy
    {
        private static readonly IProxy factory;

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
