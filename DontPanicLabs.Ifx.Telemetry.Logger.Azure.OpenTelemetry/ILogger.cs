using DontPanicLabs.Ifx.Telemetry.Logger.Contracts;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.OpenTelemetry;

public interface ILogger : Contracts.ILogger
{
    [Obsolete("Custom properties are not supported in OpenTelemetry.", error: true)]
    public new void Log(string message, SeverityLevel severityLevel, IDictionary<string, string> properties);
    
    public void Log(string message, SeverityLevel severityLevel);
    
    [Obsolete("Custom properties are not supported in OpenTelemetry.", error: true)]
    public new void Exception(Exception exception, IDictionary<string, string> properties);
    
    public void Exception(Exception exception);
}