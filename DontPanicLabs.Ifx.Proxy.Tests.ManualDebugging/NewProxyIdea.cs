using Autofac;
using DontPanicLabs.Ifx.Engine.Tests.NewProxyIdea;
using DontPanicLabs.Ifx.Manager.Tests.NewProxyIdea;
using DontPanicLabs.Ifx.Proxy.Contracts.Exceptions;
using DontPanicLabs.Ifx.Services.Contracts;


namespace DontPanicLabs.Ifx.Proxy.Tests.ManualDebugging
{
    [TestClass]
    public class NewProxyIdea
    {
        [TestInitialize]
        public void Initialize()
        {
            IoC.Autofac.ContainerBuilder builder = new IoC.Autofac.ContainerBuilder();

            builder.RegisterServices(options =>
            {
                options.RegisterType<TestSubsystem>().As<ITestSubsystem>().SingleInstance();
                options.RegisterType<TestComponent>().As<ITestComponent>().SingleInstance();
            });

            Proxy.RegisterServices(builder);

            var proxyManager = Proxy.ForSubsystem<ITestSubsystem>();
        }

        [TestMethod]
        public void Run_One()
        {
            Console.WriteLine($"Subsystem {Proxy.ForSubsystem<ITestSubsystem>().Id}");
            Console.WriteLine($"Subsystem {Proxy.ForSubsystem<ITestSubsystem>().Id}");
            Console.WriteLine($"Subsystem {Proxy.ForSubsystem<ITestSubsystem>().Id}");

            Console.WriteLine($"Component {Proxy.ForComponent<ITestComponent>(this).Id}");
            Console.WriteLine($"Component {Proxy.ForComponent<ITestComponent>(this).Id}");
            Console.WriteLine($"Component {Proxy.ForComponent<ITestComponent>(this).Id}");

            Proxy.OverrideSingleton<ITestSubsystem>(typeof(TestSubsystem));

            Console.WriteLine($"Subsystem {Proxy.ForSubsystem<ITestSubsystem>().Id}");
            Console.WriteLine($"Subsystem {Proxy.ForSubsystem<ITestSubsystem>().Id}");
            Console.WriteLine($"Subsystem {Proxy.ForSubsystem<ITestSubsystem>().Id}");

            Console.WriteLine($"Component {Proxy.ForComponent<ITestComponent>(this).Id}");
            Console.WriteLine($"Component {Proxy.ForComponent<ITestComponent>(this).Id}");
            Console.WriteLine($"Component {Proxy.ForComponent<ITestComponent>(this).Id}");

        }

        //[TestMethod]
        public void Run_Two()
        {
            Console.WriteLine(Proxy.ForSubsystem<ITestSubsystem>().Id);
            Console.WriteLine(Proxy.ForSubsystem<ITestSubsystem>().Id);
            Console.WriteLine(Proxy.ForSubsystem<ITestSubsystem>().Id);

            Proxy.OverrideSingleton<ITestSubsystem>(typeof(TestSubsystem));

            Console.WriteLine(Proxy.ForSubsystem<ITestSubsystem>().Id);
            Console.WriteLine(Proxy.ForSubsystem<ITestSubsystem>().Id);
            Console.WriteLine(Proxy.ForSubsystem<ITestSubsystem>().Id);
        }
    }

    public interface IProxy
    {
        static abstract I ForSubsystem<I>() where I : class, ISubsystem;

        static abstract I ForComponent<I>(object caller) where I : class, IComponent;

        static abstract I ForUtility<I>() where I : class, IUtility;

        static abstract void RegisterServices(IoC.Autofac.ContainerBuilder builder);

        static abstract void RegisterServices();

        static abstract void Override<I>(Type implementation);

        static abstract void OverrideSingleton<I>(Type implementation);

        static abstract void OverrideTransient<I>(Type implementation);

        //static abstract void OverrideInstance<T>(T instance);
    }


    // Autofac Proxy Implementation
    public class Proxy : IProxy
    {
        private static IoC.Autofac.ContainerBuilder? Builder;

        private static IoC.Contracts.IContainer? Container;

        public static I ForSubsystem<I>() where I : class, ISubsystem
        {
            NamespaceException.ThrowIfNotSubsystem(typeof(I));

            if (Container is null) throw new ArgumentNullException(nameof(Container));

            I subsystem = Container.GetService<I>();

            return subsystem;
        }

        public static I ForComponent<I>(object caller) where I : class, IComponent
        {
            if (caller == null)
            {
                throw new ArgumentNullException(nameof(caller), "Invalid component call. Must supply a caller.");
            }

            NamespaceException.ThrowIfNotComponent(typeof(I));

            if (Container is null) throw new ArgumentNullException(nameof(Container));

            I component = Container.GetService<I>();

            return component;
        }

        public static I ForUtility<I>() where I : class, IUtility
        {
            NamespaceException.ThrowIfNotUtility(typeof(I));

            if (Container is null) throw new ArgumentNullException(nameof(Container));

            I component = Container.GetService<I>();

            return component;
        }

        public static void RegisterServices(IoC.Autofac.ContainerBuilder builder)
        {
            Builder = builder;
            Container = builder.Build();
        }

        public static void RegisterServices()
        {
            throw new NotImplementedException();
        }

        public static void OverrideSingleton<I>(Type implementation)
        {
            Builder!.RegisterServices(options => 
            {
                options.RegisterType(implementation).As(typeof(I)).SingleInstance();
            });

            Container = Builder.Build();
        }

        public static void OverrideTransient<I>(Type implementation)
        {
            Builder!.RegisterServices(options =>
            {
                options.RegisterType(implementation).As(typeof(I)).InstancePerDependency();
            });

            Container = Builder.Build();
        }

        public static void Override<I>(Type implementation)
        {
            throw new NotImplementedException();
        }

        //public static void OverrideInstance<T>(T instance)
        //{
        //    Builder!.RegisterServices(options =>
        //    {
        //        options.Register<T>(instance);
        //    });

        //    Container = Builder.Build();
        //}
    }

    
}

namespace DontPanicLabs.Ifx.Manager.Tests.NewProxyIdea
{
    public interface ITestSubsystem : ISubsystem
    {
        public Guid Id { get; }
    }

    public class TestSubsystem : ITestSubsystem
    {
        public Guid Id { get; set; }

        public TestSubsystem()
        {
            Id = Guid.NewGuid();
        }
    }
}

namespace DontPanicLabs.Ifx.Engine.Tests.NewProxyIdea
{
    public interface ITestComponent : IComponent
    {
        public Guid Id { get; }
    }

    public class TestComponent : ITestComponent
    {
        public Guid Id { get; set; }

        public TestComponent()
        {
            Id = Guid.NewGuid();
        }
    }
}
