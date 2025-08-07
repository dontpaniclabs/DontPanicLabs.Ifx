using DontPanicLabs.Ifx.Proxy.Contracts.Service;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Tests.Helpers;

public class TestProxyEnabledComponent : ProxyEnabledServiceBase, ITestProxyEnabledComponent, IDisposable
{
    private Action? _disposeCallback;

    public void TestMe()
    {
        Console.WriteLine($"'{nameof(TestProxyEnabledComponent)}.{nameof(TestMe)} called.'");
    }

    public void SetDisposeCallback(Action disposeCallback) => _disposeCallback = disposeCallback;

    public void Dispose() => _disposeCallback?.Invoke();
}