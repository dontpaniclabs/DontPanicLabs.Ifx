using Microsoft.Extensions.Logging;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.OpenTelemetry.Tests;

public record LogEntry(LogLevel LogLevel, string Message, Exception? Exception);