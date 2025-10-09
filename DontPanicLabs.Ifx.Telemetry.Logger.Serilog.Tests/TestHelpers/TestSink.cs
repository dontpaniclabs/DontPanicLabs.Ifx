using Serilog.Core;
using Serilog.Events;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Serilog.Tests.TestHelpers;

public class TestSink : ILogEventSink
{
    public List<LogEvent> LogEvents { get; } = [];

    public void Emit(LogEvent logEvent)
    {
        LogEvents.Add(logEvent);
    }
}