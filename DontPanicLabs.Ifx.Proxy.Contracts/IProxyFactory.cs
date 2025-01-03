using System.ComponentModel;

namespace DontPanicLabs.Ifx.Proxy.Contracts
{
    public interface IProxyFactory
    {
        IContainer RegisterFromAutoDiscover() { throw new NotImplementedException(); }

        IContainer RegisterFromConfiguration(Dictionary<Type, Type[]> serviceTypes) { throw new NotImplementedException(); }
    }
}