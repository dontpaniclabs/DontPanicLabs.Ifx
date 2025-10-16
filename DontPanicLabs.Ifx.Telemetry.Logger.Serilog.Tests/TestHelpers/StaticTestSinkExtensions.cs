using Serilog;
using Serilog.Configuration;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Serilog.Tests.TestHelpers;

/// <summary>
/// Serilog magic to allows configuring our <see cref="StaticTestSink"/> via appsettings configuration. Without
/// this, registering the test log sink in appsettings would have no effect.
/// </summary>
public static class StaticTestSinkExtensions
{
    public static LoggerConfiguration StaticTestSink(
        this LoggerSinkConfiguration loggerConfiguration)
    {
        return loggerConfiguration.Sink(new StaticTestSink());
    }
}