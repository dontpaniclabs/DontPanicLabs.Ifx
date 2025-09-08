using Microsoft.Extensions.Logging;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.OpenTelemetry.Tests;

public class TestLoggerProvider : ILoggerProvider
{
    public List<LogEntry> LogEntries { get; } = [];

    public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
    {
        return new TestLogger(LogEntries);
    }

    public void Dispose() { }
}