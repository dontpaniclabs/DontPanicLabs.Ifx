namespace DontPanicLabs.Ifx.Telemetry.Logger.Contracts;

public interface ILogger
{
    public void Log(string message, SeverityLevel severityLevel, IDictionary<string, string> properties);

    public void Exception(Exception exception, IDictionary<string, string> properties);

    public void Event(string eventName, IDictionary<string, string> properties, IDictionary<string, double> metrics, DateTimeOffset timeStamp);

    public void Flush();
}