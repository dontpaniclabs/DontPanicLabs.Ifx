namespace DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Exceptions;

public sealed class EmptyConnectionStringException(string message) : ArgumentException(message)
{
    private const string ConnectionStringEmptyMessage = "The entry for the AppInsights connection string was found " +
                                                 "in configuration, but the value was blank or empty.";

    public static void ThrowIfEmpty(string connectionString)
    {
        if (connectionString.Length == 0)
        {
            throw new EmptyConnectionStringException(ConnectionStringEmptyMessage);
        }
    }
}