using DontPanicLabs.Ifx.Proxy.Contracts.Service;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Tests.Helpers;

public interface ITestProxyEnabledUtility : IProxyEnabledUtility
{
    void TestMe();
}