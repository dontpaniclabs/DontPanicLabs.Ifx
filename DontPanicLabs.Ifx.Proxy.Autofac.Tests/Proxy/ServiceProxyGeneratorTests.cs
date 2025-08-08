using Castle.DynamicProxy;
using DontPanicLabs.Ifx.Proxy.Contracts.Exceptions;
using DontPanicLabs.Ifx.Proxy.Autofac.Interceptor;
using DontPanicLabs.Ifx.Proxy.Autofac.Registration;
using DontPanicLabs.Ifx.Proxy.Autofac.Tests.Helpers;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using Moq;
using Shouldly;
using IInvocation = Castle.DynamicProxy.IInvocation;
using TestContext = DontPanicLabs.Ifx.Proxy.Autofac.Tests.Helpers.TestContext;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Tests.Proxy;

[TestClass]
[TestCategoryCI]
public class ServiceProxyGeneratorTests
{
    [TestCleanup]
    public void Cleanup()
    {
        ServiceRegistrarHelpers.ResetRegistrations();
    }

    [TestMethod]
    public void ProxyGenerator_CreatesDifferentlyScopedProxiesFromRegistrations()
    {
        RegistrationBase[] registrations =
        [
            TypeRegistration.New<ITestProxyEnabledComponent, TestProxyEnabledComponent>(LifetimeScope.Scoped),
            TypeRegistration.New<ITestProxyEnabledUtility, TestProxyEnabledUtility>(LifetimeScope.Singleton),
            TypeRegistration.New<ITestProxyEnabledSubsystem, TestProxyEnabledSubsystem>()
        ];

        var registrationBuilder = new RegistrationBuilder(registrations);
        ServiceRegistrar.RegisterServices(registrationBuilder);

        using var proxyGenerator1 = new ServiceProxyGenerator(new TestContext());
        using var proxyGenerator2 = new ServiceProxyGenerator(new TestContext());

        // Request-scoped registrations resolve to a single instance per proxy generator,
        // but will not be the same across proxies generated from different generators.
        proxyGenerator1
            .ProxyForService<ITestProxyEnabledComponent>()
            .ShouldBeSameAs(proxyGenerator1.ProxyForService<ITestProxyEnabledComponent>());

        proxyGenerator1
            .ProxyForService<ITestProxyEnabledComponent>()
            .ShouldNotBeSameAs(proxyGenerator2.ProxyForService<ITestProxyEnabledComponent>());

        // Transient registrations always resolve to new instances.
        proxyGenerator1
            .ProxyForService<ITestProxyEnabledSubsystem>()
            .ShouldNotBeSameAs(proxyGenerator1.ProxyForService<ITestProxyEnabledSubsystem>());

        // Singleton registrations resolve the same instance across proxy generators.
        proxyGenerator1
            .ProxyForService<ITestProxyEnabledUtility>()
            .ShouldBeSameAs(proxyGenerator1.ProxyForService<ITestProxyEnabledUtility>());
    }

    [TestMethod]
    public void ProxyGenerator_ServicesGetReferencesToTheSameProxyGenerator()
    {
        RegistrationBase[] registrations =
        [
            TypeRegistration.New<ITestProxyEnabledComponent, TestProxyEnabledComponent>(),
            TypeRegistration.New<ITestProxyEnabledSubsystem, TestProxyEnabledSubsystem>(),
            TypeRegistration.New<ITestProxyEnabledUtility, TestProxyEnabledUtility>()
        ];

        var registrationBuilder = new RegistrationBuilder(registrations);
        ServiceRegistrar.RegisterServices(registrationBuilder);

        using var proxyGenerator = new ServiceProxyGenerator(new TestContext());

        var subsystem = proxyGenerator.ProxyForService<ITestProxyEnabledSubsystem>();
        var component = subsystem.ServiceProxyGenerator.ProxyForService<ITestProxyEnabledComponent>();
        var utility = component.ServiceProxyGenerator.ProxyForService<ITestProxyEnabledUtility>();

        subsystem.ServiceProxyGenerator.ShouldBeSameAs(component.ServiceProxyGenerator);
        component.ServiceProxyGenerator.ShouldBeSameAs(utility.ServiceProxyGenerator);
    }

    [TestMethod]
    public void ProxyGenerator_ProxyEnabledServiceBaseIsNotRequired()
    {
        RegistrationBase[] registrations =
        [
            TypeRegistration.New<ITestComponent, TestComponent>(),
            TypeRegistration.New<ITestSubsystem, TestSubsystem>(),
            TypeRegistration.New<ITestUtility, TestUtility>()
        ];

        var registrationBuilder = new RegistrationBuilder(registrations);
        ServiceRegistrar.RegisterServices(registrationBuilder);

        using var proxyGenerator = new ServiceProxyGenerator(new TestContext());

        _ = proxyGenerator.ProxyForService<ITestComponent>().ShouldNotBeNull();
        _ = proxyGenerator.ProxyForService<ITestSubsystem>().ShouldNotBeNull();
        _ = proxyGenerator.ProxyForService<ITestUtility>().ShouldNotBeNull();
    }

    [TestMethod]
    public void ProxyGenerator_Constructor_ServicesNotRegistered_ShouldThrow()
    {
        var exception = Should.Throw<ProxyException>(() =>
        {
            using var proxyGenerator = new ServiceProxyGenerator(new TestContext());
        });

        const string expectedMessage =
            "No services have been registered. " +
            "Did you forget to call 'ServiceRegistrar.RegisterServices'?";

        exception.Message.ShouldBe(expectedMessage);
    }

    [TestMethod]
    public void ProxyGenerator_Resolve_ServiceNotFound_ShouldThrow()
    {
        RegistrationBase[] registrations =
        [
            TypeRegistration.New<ITestProxyEnabledComponent, TestProxyEnabledComponent>(),
        ];

        var registrationBuilder = new RegistrationBuilder(registrations);
        ServiceRegistrar.RegisterServices(registrationBuilder);

        using var proxyGenerator = new ServiceProxyGenerator(new TestContext());

        var exception = Should.Throw<ProxyException>(() =>
            proxyGenerator.ProxyForService<ITestProxyEnabledUtility>()
        );

        const string expectedMessage =
            "Service of type 'ITestProxyEnabledUtility' was not found. Did you forget to register it?";

        exception.Message.ShouldBe(expectedMessage);
    }

    [TestMethod]
    public void ProxyGenerator_ProxyGeneratorDisposed_DisposesComponents()
    {
        RegistrationBase[] registrations =
        [
            TypeRegistration.New<ITestProxyEnabledComponent, TestProxyEnabledComponent>(LifetimeScope.Scoped)
        ];

        var registrationBuilder = new RegistrationBuilder(registrations);
        ServiceRegistrar.RegisterServices(registrationBuilder);

        var callback = new Mock<Action>();
        callback.Setup(action => action()).Verifiable();

        using (var proxyGenerator = new ServiceProxyGenerator(new TestContext()))
        {
            var component = proxyGenerator.ProxyForService<ITestProxyEnabledComponent>();
            component.SetDisposeCallback(callback.Object);

            callback.Verify(action => action(), Times.Never);
        }

        callback.Verify(action => action(), Times.Once);
    }

    [TestMethod]
    public void ProxyGenerator_Resolve_ServiceWithInterceptor_MethodInvokesInterceptorOnInterceptedService()
    {
        var callback = new Mock<Action>();
        callback.Setup(action => action()).Verifiable();

        var registrationBuilder = new RegistrationBuilder(
            TypeRegistration.New<ITestProxyEnabledComponent, TestProxyEnabledComponent>(
                interceptors: new TestInterceptorA(callback.Object)),
            TypeRegistration.New<ITestProxyEnabledUtility, TestProxyEnabledUtility>()
        );

        ServiceRegistrar.RegisterServices(registrationBuilder);

        using var proxyGenerator = new ServiceProxyGenerator(new TestContext());
        var component = proxyGenerator.ProxyForService<ITestProxyEnabledComponent>();
        var utility = proxyGenerator.ProxyForService<ITestProxyEnabledUtility>();

        callback.Verify(action => action(), Times.Never);

        component.TestMe();

        callback.Verify(action => action(), Times.Once);

        utility.TestMe();

        callback.Verify(action => action(), Times.Once);
    }

    [TestMethod]
    public void ProxyGenerator_Resolve_ServicesWithSharedInterceptor_MethodsInvokeInterceptor()
    {
        var callback = new Mock<Action>();
        callback.Setup(action => action()).Verifiable();

        var registrationBuilder = new RegistrationBuilder(
            TypeRegistration.New<ITestProxyEnabledComponent, TestProxyEnabledComponent>(
                interceptors: new TestInterceptorA(callback.Object)),
            TypeRegistration.New<ITestProxyEnabledUtility, TestProxyEnabledUtility>(
                interceptors: new TestInterceptorA(callback.Object))
        );

        ServiceRegistrar.RegisterServices(registrationBuilder);

        using var proxyGenerator = new ServiceProxyGenerator(new TestContext());
        var component = proxyGenerator.ProxyForService<ITestProxyEnabledComponent>();
        var utility = proxyGenerator.ProxyForService<ITestProxyEnabledUtility>();

        callback.Verify(action => action(), Times.Never);

        component.TestMe();

        callback.Verify(action => action(), Times.Exactly(1));

        utility.TestMe();

        callback.Verify(action => action(), Times.Exactly(2));
    }

    [TestMethod]
    public void ProxyGenerator_Resolve_ServiceWithMultipleInterceptors_MethodInvokesAllInterceptors()
    {
        var callbackA = new Mock<Action>();
        callbackA.Setup(action => action()).Verifiable();

        var callbackB = new Mock<Action>();
        callbackB.Setup(action => action()).Verifiable();

        var registrationBuilder = new RegistrationBuilder(
            TypeRegistration.New<ITestProxyEnabledComponent, TestProxyEnabledComponent>(
                interceptors: [new TestInterceptorA(callbackA.Object), new TestInterceptorB(callbackB.Object)]
            ));

        ServiceRegistrar.RegisterServices(registrationBuilder);

        using var proxyGenerator = new ServiceProxyGenerator(new TestContext());
        var component = proxyGenerator.ProxyForService<ITestProxyEnabledComponent>();

        callbackA.Verify(action => action(), Times.Never);
        callbackB.Verify(action => action(), Times.Never);

        component.TestMe();

        callbackA.Verify(action => action(), Times.Once);
        callbackB.Verify(action => action(), Times.Once);
    }

    [TestMethod]
    public void ProxyGenerator_Resolve_ServiceWithMultipleInterceptors_UniqueInterceptorCanOnlyBeAddedOnce()
    {
        var callbackA = new Mock<Action>();
        callbackA.Setup(action => action()).Verifiable();

        var callbackB = new Mock<Action>();
        callbackB.Setup(action => action()).Verifiable();

        var registrationBuilder = new RegistrationBuilder(
            TypeRegistration.New<ITestProxyEnabledComponent, TestProxyEnabledComponent>(
                interceptors: [new TestInterceptorA(callbackA.Object), new TestInterceptorA(callbackB.Object)]
            ));

        ServiceRegistrar.RegisterServices(registrationBuilder);

        using var proxyGenerator = new ServiceProxyGenerator(new TestContext());
        var component = proxyGenerator.ProxyForService<ITestProxyEnabledComponent>();

        callbackB.Verify(action => action(), Times.Never);

        component.TestMe();

        // Note that the second interceptor overrides the first one.
        callbackA.Verify(action => action(), Times.Never);
        callbackB.Verify(action => action(), Times.Once);
    }

    private sealed class TestInterceptorA(Action testCallback) : AsyncMethodInterceptorBase
    {
        protected override void InterceptMethodInvocation(
            IInvocationProceedInfo invocationProceedInfo,
            IInvocation invocation
        )
        {
            testCallback();

            invocation.Proceed();
        }
    }

    private sealed class TestInterceptorB(Action testCallback) : AsyncMethodInterceptorBase
    {
        protected override void InterceptMethodInvocation(
            IInvocationProceedInfo invocationProceedInfo,
            IInvocation invocation
        )
        {
            testCallback();

            invocation.Proceed();
        }
    }
}
