using DontPanicLabs.Ifx.Engine.ProxyTest;
using DontPanicLabs.Ifx.Manager.ProxyTest;
using Autofac;
using IContainer = DontPanicLabs.Ifx.IoC.Contracts.IContainer;
using ContainerBuilder = DontPanicLabs.Ifx.IoC.Autofac.ContainerBuilder;
using System.Diagnostics;
using DontPanicLabs.Ifx.Services.Contracts;
using DontPanicLabs.Ifx.Proxy.Contracts;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DPL.Ifx.Proxy.Tests.ManualDebugging
{
    [TestClass]
    public class ProxyTest
    {
        [TestMethod]
        [TestCategoryLocal]
        public void Debug_Customized()
        {
            var proxySubsystem = Company.Product.Customized.Proxy.ForSubsystem<ITestSubsystem>();

            _ = proxySubsystem;

            var proxyComponent = Company.Product.Customized.Proxy.ForComponent<ITestComponent>(this);

            _ = proxyComponent;
        }

        [TestMethod]
        [TestCategoryLocal]
        public void Debug_Default()
        {
            var proxySubsystem = Company.Product.Default.Proxy.ForSubsystem<ITestSubsystem>();

            _ = proxySubsystem;

            var proxyComponent = Company.Product.Default.Proxy.ForComponent<ITestComponent>(this);

            _ = proxyComponent;
        }

        [TestMethod]
        [TestCategoryLocal]
        public void Debug_Perf()
        {
            int loops = 0;

            var sw = new Stopwatch();
            sw.Start();
            do
            {
                var proxySubsystem = Company.Product.Customized.Proxy.ForSubsystem<ITestSubsystem>();

                _ = proxySubsystem;

                loops++;
            } while (loops < 10000);

            sw.Stop();

            var dur = sw.ElapsedMilliseconds / 1000d;

            _ = dur;
        }
    }
}

namespace Company.Product.Customized
{
    public static class Proxy
    {
        private static readonly IProxy factory;

        static Proxy()
        {
            var registrations = new Dictionary<Type, Type[]>()
            {
                { typeof(ITestSubsystem), [typeof(TestSubsystem)] }
            };

            factory = new DontPanicLabs.Ifx.Proxy.Autofac.ProxyFactory(registrations);
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

namespace Company.Product.Default
{
    public static class Proxy
    {
        private static readonly IProxy factory;

        static Proxy()
        {
            factory = new DontPanicLabs.Ifx.Proxy.Autofac.ProxyFactory();
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

namespace DontPanicLabs.Ifx.Manager.ProxyTest
{
    public interface ITestSubsystem : ISubsystem
    {
    }

    public class TestSubsystem : ITestSubsystem
    {
    }
}

namespace DontPanicLabs.Ifx.Engine.ProxyTest
{
    public interface ITestComponent : IComponent
    {
    }

    public class TestComponent : ITestComponent
    {
    }
}