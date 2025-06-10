namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.ApplicationInsights.Exceptions;

public class InvalidTelemetryChannelException : ArgumentException
{
    private const string InvalidTelemetryChannel = "A `TelemetryChannel` was provided, but its value '{0}' was invalid.";

    InvalidTelemetryChannelException(string message) : base(message)
    {
    }


    public static InvalidTelemetryChannelException Create(string? channel)
    {
        return new InvalidTelemetryChannelException(string.Format(InvalidTelemetryChannel, channel));
    }

    public static void ThrowIfChannelInvalid(string? channel)
    {
        if (!string.IsNullOrEmpty(channel) && channel != "InMemoryChannel" && channel != "ServerTelemetryChannel")
        {
            throw Create(channel);
        }
    }
}