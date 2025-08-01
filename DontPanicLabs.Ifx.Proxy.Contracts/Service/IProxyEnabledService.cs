using DontPanicLabs.Ifx.Proxy.Contracts.Proxy;

namespace DontPanicLabs.Ifx.Proxy.Contracts.Service;

public interface IProxyEnabledService
{
    IProxyGenerator ServiceProxyGenerator { get; set; }
}