using Serilog.Core;
using Serilog.Events;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Serilog.Tests.TestHelpers;

public class StaticTestSink : ILogEventSink
{
    public static List<LogEvent> LogEvents { get; set; } = [];

    public void Emit(LogEvent logEvent)
    {
        LogEvents.Add(logEvent);
    }
}