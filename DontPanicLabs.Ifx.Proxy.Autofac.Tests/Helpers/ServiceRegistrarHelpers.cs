using DontPanicLabs.Ifx.Proxy.Autofac;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Tests.Helpers;

public static class ServiceRegistrarHelpers
{
    public static void ResetRegistrations()
    {
        ServiceRegistrar.Reset();
    }
}