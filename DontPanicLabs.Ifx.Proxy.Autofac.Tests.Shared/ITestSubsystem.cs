using Autofac.Extras.DynamicProxy;
using DontPanicLabs.Ifx.Services.Contracts;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Tests.Shared.Manager
{
    [Intercept(typeof(TestInterceptor))]
    public interface ITestSubsystem : ISubsystem
    {
        public void Test()
        {
            var debug = true;

            _ = debug;
        }
    }
}
