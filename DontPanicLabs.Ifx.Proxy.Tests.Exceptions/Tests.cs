using DontPanicLabs.Ifx.Accessor.Proxy.Tests;
using DontPanicLabs.Ifx.Engine.Proxy.Tests;
using DontPanicLabs.Ifx.Manager.Proxy.Tests;
using DontPanicLabs.Ifx.Proxy.Autofac;
using DontPanicLabs.Ifx.Proxy.Contracts;
using DontPanicLabs.Ifx.Proxy.Contracts.Exceptions;
using DontPanicLabs.Ifx.Services.Contracts;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DontPanicLabs.Ifx.Proxy.Tests.Exceptions
{
    [TestClass]
    [TestCategoryCI]
    public class ProxyExceptionTests
    {
        [TestMethod]
        public void NullNamespace()
        {
            Assert.ThrowsException<NamespaceException>(() => Proxy.ForSubsystem<NoNamespace>());
        }

        [TestMethod]
        public void SubsystemCall_Tests()
        {
            Assert.ThrowsException<NamespaceException>(() => Proxy.ForSubsystem<IManagerInAccNs>());
            Assert.ThrowsException<NamespaceException>(() => Proxy.ForSubsystem<IManagerInEngNs>());

            Assert.IsInstanceOfType<TestManager>(Proxy.ForSubsystem<ITestManager>().GetProxyTarget());
        }

        [TestMethod]
        public void ComponentCall_Tests()
        {
            Assert.ThrowsException<NamespaceException>(() => Proxy.ForComponent<IEngineInMgrNs>(this));
            Assert.ThrowsException<NamespaceException>(() => Proxy.ForComponent<IAccessorInMgrNs>(this));

            Assert.IsInstanceOfType<TestAccessor>(Proxy.ForComponent<ITestAccessor>(this).GetProxyTarget());
            Assert.IsInstanceOfType<TestEngine>(Proxy.ForComponent<ITestEngine>(this).GetProxyTarget());
        }
    }
}

namespace DontPanicLabs.Ifx.Manager.Proxy.Tests
{ 
    public class TestManager : ITestManager;

    public interface ITestManager : ISubsystem;

    public interface IEngineInMgrNs: IComponent;
    
    public interface IAccessorInMgrNs: IComponent;

    public class EngineInMgrNs : IEngineInMgrNs;

    public class AccessorInMgrNs : IAccessorInMgrNs;
}

namespace DontPanicLabs.Ifx.Engine.Proxy.Tests
{
    public interface ITestEngine : IComponent;

    public interface IManagerInEngNs : ISubsystem;

    public class TestEngine : ITestEngine;

    public class ManagerInEngNs : IManagerInEngNs;
}

namespace DontPanicLabs.Ifx.Accessor.Proxy.Tests
{
    public interface ITestAccessor : IComponent;

    public interface IManagerInAccNs : ISubsystem;

    public class TestAccessor : ITestAccessor;

    public class ManagerInAccNs : IManagerInAccNs;
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

class NoNamespace : ISubsystem;