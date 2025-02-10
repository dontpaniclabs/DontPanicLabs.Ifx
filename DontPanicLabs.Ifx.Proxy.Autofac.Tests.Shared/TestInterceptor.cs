using DontPanicLabs.Ifx.Proxy.Contracts;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Tests.Shared
{
    public class TestInterceptor : IInterceptor
    {
        public void Intercept(Castle.DynamicProxy.IInvocation invocation)
        {
            throw new NotImplementedException();
        }
    }
}
