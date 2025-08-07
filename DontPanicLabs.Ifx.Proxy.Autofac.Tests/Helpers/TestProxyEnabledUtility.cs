using DontPanicLabs.Ifx.Proxy.Contracts.Service;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Tests.Helpers;

public class TestProxyEnabledUtility : ProxyEnabledServiceBase, ITestProxyEnabledUtility
{
    public void TestMe()
    {
        Console.WriteLine($"'{nameof(TestProxyEnabledUtility)}.{nameof(TestMe)} called.'");
    }
}