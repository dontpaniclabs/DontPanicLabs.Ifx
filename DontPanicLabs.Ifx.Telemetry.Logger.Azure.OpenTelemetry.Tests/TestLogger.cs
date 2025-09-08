using Microsoft.Extensions.Logging;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.OpenTelemetry.Tests;

public class TestLogger : Microsoft.Extensions.Logging.ILogger
{
    private readonly List<LogEntry> _logEntries;

    public TestLogger(List<LogEntry> logEntries)
    {
        _logEntries = logEntries;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        _logEntries.Add(new LogEntry(logLevel, message, exception));
    }
}