using DontPanicLabs.Ifx.Proxy.Contracts.Service;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Tests.Helpers;

public interface ITestProxyEnabledSubsystem : IProxyEnabledSubsystem
{
    void TestMe();
}