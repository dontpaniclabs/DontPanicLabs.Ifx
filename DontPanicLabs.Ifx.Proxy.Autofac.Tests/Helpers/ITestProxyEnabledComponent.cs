using DontPanicLabs.Ifx.Proxy.Contracts.Service;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Tests.Helpers;

public interface ITestProxyEnabledComponent : IProxyEnabledComponent
{
    void SetDisposeCallback(Action disposeCallback);

    void TestMe();
}