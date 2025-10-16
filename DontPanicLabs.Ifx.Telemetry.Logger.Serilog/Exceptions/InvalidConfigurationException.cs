namespace DontPanicLabs.Ifx.Telemetry.Logger.Serilog.Exceptions;

public class InvalidConfigurationException : ArgumentException
{
    private const string ConfigNullOrEmptyMessage = "Serilong configuration JSON must not be null or empty.";

    private InvalidConfigurationException(string message) : base(message)
    {
    }

    public static void ThrowIfConfigNullOrEmpty(string? serilogConfigJson)
    {
        if (string.IsNullOrEmpty(serilogConfigJson))
        {
            throw new InvalidConfigurationException(ConfigNullOrEmptyMessage);
        }
    }
}