namespace DontPanicLabs.Ifx.Telemetry.Logger.Serilog.Exceptions;

/// <summary>
/// Exception representing invalid Serilog configuration.
/// </summary>
public class InvalidConfigurationException : ArgumentException
{
    private const string ConfigNullOrEmptyMessage = "Serilog configuration JSON must not be null or empty.";
    private const string InvalidJsonMessage = "Serilog configuration JSON is not valid.";

    private InvalidConfigurationException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public static InvalidConfigurationException CreateForInvalidJson(Exception innerException)
    {
        return new InvalidConfigurationException(InvalidJsonMessage, innerException);
    }

    public static void ThrowIfConfigNullOrEmpty(string? serilogConfigJson)
    {
        if (string.IsNullOrEmpty(serilogConfigJson))
        {
            throw new InvalidConfigurationException(ConfigNullOrEmptyMessage, null);
        }
    }
}