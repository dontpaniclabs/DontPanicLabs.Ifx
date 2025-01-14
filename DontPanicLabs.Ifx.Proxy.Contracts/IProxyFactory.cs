using DontPanicLabs.Ifx.IoC.Contracts;

namespace DontPanicLabs.Ifx.Proxy.Contracts
{
    public interface IProxyFactory
    {
        void RegisterServices();

        IContainer RegisterFromAutoDiscover() { throw new NotImplementedException(); }

        IContainer RegisterFromConfiguration(Dictionary<Type, Type[]> serviceTypes) { throw new NotImplementedException(); }
    }
}