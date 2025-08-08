using DontPanicLabs.Ifx.Proxy.Contracts.Service;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Tests.Helpers;

public class TestProxyEnabledSubsystem : ProxyEnabledServiceBase, ITestProxyEnabledSubsystem
{
    public void TestMe()
    {
        Console.WriteLine($"'{nameof(TestProxyEnabledSubsystem)}.{nameof(TestMe)} called.'");
    }
}