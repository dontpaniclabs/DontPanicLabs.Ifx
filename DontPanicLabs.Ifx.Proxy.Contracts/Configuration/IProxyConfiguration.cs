namespace DontPanicLabs.Ifx.Proxy.Contracts.Configuration
{
    public interface IProxyConfiguration
    {
        // This is false by default
        bool AutoDiscoverServices { get; }

        // This is false by default
        //bool IsInterceptionEnabled { get; }

        // For this dictionary, the key is the Interface, and the value is the list of implementations
        // that could resolve for the key (interface).
        Dictionary<Type, Type[]> ServiceRegistrations { get; }
    }

    enum CommunicationMode
    {
        // This might be the only one we use for the forseeable future
        InProc,

        // Future
        // Grpc,

        // Future
        // Http
    }
}