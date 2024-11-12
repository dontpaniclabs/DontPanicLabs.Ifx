namespace DontPanicLabs.Ifx.Configuration.Contracts
{
    public interface IConfig
    {
        string? this[string key] { get; set; }
    }
}