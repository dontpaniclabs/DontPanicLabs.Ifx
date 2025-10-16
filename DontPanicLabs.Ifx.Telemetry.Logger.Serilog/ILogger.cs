namespace DontPanicLabs.Ifx.Telemetry.Logger.Serilog;

/// <summary>
/// A Serilog-specific version of our core <see cref="Contracts.ILogger"/> interface.
/// </summary>
public interface ILogger : Contracts.ILogger
{
    [Obsolete("Serilog does not implement a Flush method, dispose of instances instead.", error: true)]
    public new void Flush();
}