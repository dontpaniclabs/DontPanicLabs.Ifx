using System.Diagnostics;
using System.Reflection;
using Castle.DynamicProxy;
using DontPanicLabs.Ifx.Proxy.Autofac.Interceptor;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using Moq;
using Shouldly;
using IInvocation = Castle.DynamicProxy.IInvocation;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Tests.Interceptor;

[TestClass]
[TestCategoryCI]
public class AsyncMethodInterceptorBaseTests
{
    private Mock<Action> _callbackMock = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _callbackMock = new Mock<Action>();
        _callbackMock.Setup(callback => callback()).Verifiable();
    }

    [TestMethod]
    [DataRow(typeof(AsyncMethodInvocationInterceptor), 1)]
    [DataRow(typeof(RegularInterceptor), 1)]
    public async Task InterceptMethodInvocation_MethodInvocation_InvokesInterceptor(Type interceptorType,
        int expectedCallCount)
    {
        var interceptor = (IInterceptor) Activator.CreateInstance(interceptorType, _callbackMock.Object)!;
        var proxy = CreateProxy(interceptor);

        _callbackMock.Verify(callback => callback(), Times.Never);

        _ = await proxy.Method();

        _callbackMock.Verify(callback => callback(), Times.Exactly(expectedCallCount));
    }

    [DataTestMethod]
    [DataRow(typeof(AsyncMethodInvocationInterceptor), 0)]
    [DataRow(typeof(RegularInterceptor), 1)]
    public void InterceptMethodInvocation_PropertyGet(Type interceptorType, int expectedCallCount)
    {
        var interceptor = (IInterceptor) Activator.CreateInstance(interceptorType, _callbackMock.Object)!;
        var proxy = CreateProxy(interceptor);

        _callbackMock.Verify(callback => callback(), Times.Never);

        proxy.Property.ShouldBe(0);

        _callbackMock.Verify(callback => callback(), Times.Exactly(expectedCallCount));
    }

    [DataTestMethod]
    [DataRow(typeof(AsyncMethodInvocationInterceptor), 0)]
    [DataRow(typeof(RegularInterceptor), 1)]
    public void InterceptMethodInvocation_PropertySet(Type interceptorType, int expectedCallCount)
    {
        var interceptor = (IInterceptor) Activator.CreateInstance(interceptorType, _callbackMock.Object)!;
        var proxy = CreateProxy(interceptor);

        _callbackMock.Verify(callback => callback(), Times.Never);

        proxy.Property = 42;

        _callbackMock.Verify(callback => callback(), Times.Exactly(expectedCallCount));

        proxy.Property.ShouldBe(42);
    }

    [TestMethod]
    public async Task InterceptMethodInvocation_AwaitBeforeInvokeDoesNotCauseLoop()
    {
        // This test spawns a bunch of tasks that will call the interceptor.
        // If it's working properly, it will not cause a loop. The count of
        // invocations should match the number of tasks spawned. This test
        // will take a few seconds.
        var interceptor = new AsyncMethodInvocationInterceptor(_callbackMock.Object);
        var proxy = CreateProxy(interceptor);

        // Keep this test from consuming all the threads in the thread pool.
        _ = ThreadPool.SetMinThreads(1, 1);

        _callbackMock.Verify(callback => callback(), Times.Never);

        const int invocationCount = 100;

        var tasks = Enumerable
            .Range(0, invocationCount)
            .Select(_ => proxy.Method())
            .ToArray();

        _ = await Task.WhenAll(tasks);

        _callbackMock.Verify(callback => callback(), Times.Exactly(invocationCount));

        interceptor.InterceptionCount.ShouldBe(invocationCount);
    }

    [TestMethod]
    public async Task InterceptMethodInvocation_AwaitBeforeProceedCausesLoop()
    {
        // This test demonstrates that calling `Proceed` after an 'await'
        // without capturing the `ProceedInfo` will result in an infinite loop.
        // It works by spawning a bunch of tasks and cancelling them after a timeout.
        // Hopefully 10s is long enough for any machine's speed.
        var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;
        var interceptor = new BrokenAsyncMethodInvocationInterceptor(_callbackMock.Object, cancellationToken);
        var proxy = CreateProxy(interceptor);

        // Keep this test from consuming all the threads in the thread pool.
        _ = ThreadPool.SetMinThreads(1, 1);

        _callbackMock.Verify(callback => callback(), Times.Never);

        const int invocationCount = 100;

        var tasks = Enumerable
            .Range(0, invocationCount)
            .Select(_ => proxy.Method())
            .ToArray();

        _ = await Should.ThrowAsync<TaskCanceledException>(() => Task.WhenAll(tasks));

        _callbackMock.Verify(callback => callback(), Times.AtLeast(invocationCount + 1));

        interceptor.InterceptionCount.ShouldBeGreaterThan(invocationCount);

        // ~400 invocations on m1 max
        Debug.WriteLine($"Invocation count: {interceptor.InterceptionCount}");
    }

    private ITestInterface CreateProxy(IInterceptor interceptor)
    {
        var proxyGenerator = new ProxyGenerator();

        return proxyGenerator.CreateInterfaceProxyWithTarget<ITestInterface>(
            new TestService(),
            interceptor
        );
    }

    private class AsyncMethodInvocationInterceptor : AsyncMethodInterceptorBase
    {
        private readonly Action _callback;

        public AsyncMethodInvocationInterceptor(Action callback)
        {
            _callback = callback;
        }

        protected override void InterceptMethodInvocation(
            IInvocationProceedInfo invocationProceedInfo,
            IInvocation invocation
        )
        {
            invocation.ReturnValue = GetType()
                .GetMethod(nameof(InvokeAsync), BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(this, [invocationProceedInfo, invocation]);
        }

        private async Task<int> InvokeAsync(
            IInvocationProceedInfo invocationProceedInfo,
            IInvocation invocation
        )
        {
            await Task.Run(() =>
            {
                Thread.Sleep(500);

                _callback();
            });


            // Note that this MUST BE `Invoke` on the IInvocationProceedInfo, not Proceed on the IInvocation.
            invocationProceedInfo.Invoke();

            return await (Task<int>) invocation.ReturnValue;
        }
    }

    /// <summary>
    /// This interceptor is meant to demonstrate that calling `Proceed` after an 'await'
    /// will result in a loop.
    /// </summary>
    private class BrokenAsyncMethodInvocationInterceptor : AsyncMethodInterceptorBase
    {
        private readonly Action _callback;

        private readonly CancellationToken _cancellationToken;

        public BrokenAsyncMethodInvocationInterceptor(Action callback, CancellationToken cancellationToken)
        {
            _callback = callback;
            _cancellationToken = cancellationToken;
        }

        protected override void InterceptMethodInvocation(
            IInvocationProceedInfo invocationProceedInfo,
            IInvocation invocation
        )
        {
            invocation.ReturnValue = GetType()
                .GetMethod(nameof(InvokeAsync), BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(this, [invocation]);
        }

        private async Task<int> InvokeAsync(
            IInvocation invocation
        )
        {
            await Task.Run(() =>
            {
                Thread.Sleep(500);

                _callback();
            }, _cancellationToken);


            // Purposefully call `.Proceed()` after an await to demonstrate this results in a loop. The correct call is `IInvocationProceedInfo.Invoke()`
            invocation.Proceed();

            return await (Task<int>) invocation.ReturnValue;
        }
    }

    private class RegularInterceptor : IInterceptor
    {
        private readonly Action _callback;

        public RegularInterceptor(Action callback)
        {
            _callback = callback;
        }

        public void Intercept(IInvocation invocation)
        {
            _callback();

            invocation.Proceed();
        }
    }

    // ReSharper disable MemberCanBePrivate.Global
    public interface ITestInterface
    {
        int Property { get; set; }

        Task<int> Method();
    }

    public class TestService : ITestInterface
    {
        public int Property { get; set; }

        public Task<int> Method()
        {
            return Task.FromResult(9001);
        }
    }
    // ReSharper enable MemberCanBePrivate.Global
}
