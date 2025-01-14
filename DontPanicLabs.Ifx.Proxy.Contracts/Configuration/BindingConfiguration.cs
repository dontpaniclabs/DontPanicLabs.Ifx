namespace DontPanicLabs.Ifx.Proxy.Contracts.Configuration
{
    internal class BindingConfiguration
    {
        public bool? AutoDiscoverServices { get; set; }

        public bool? IsInterceptionEnabled { get; set; }

        public Dictionary<string, string[]>? ServiceRegistrations { get; set; }
    }
}