using DontPanicLabs.Ifx.Telemetry.Logger.Contracts;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using Shouldly;
using ILogger = DontPanicLabs.Ifx.Telemetry.Logger.Contracts.ILogger;

namespace DontPanicLabs.Ifx.Telemetry.Logging.Serilog.IntegrationTests;

[TestClass]
[TestCategoryLocal]
public class LoggerIntegrationTests
{
    /// <summary>
    /// A local integration test that uses a real logger instance configured to write to multiple sinks. To use this,
    /// you must have a local SQL Server instance running (see appsettings.json / replace that connection string).
    /// </summary>
    [TestMethod]
    public void IntegrationSmokeTest()
    {
        // Arrange
        var testRunId = Guid.NewGuid();
        string FormatLogMessage(string message) => $"{nameof(IntegrationSmokeTest)} [{testRunId}]: {message}";

        // Use a real logger instance configured to write to SQL, File, and Console
        // See appsettings.json for the multi-sink configuration
        ILogger logger = new Logger();

        // Log a simple informational message
        logger.Log(
            FormatLogMessage("Log - Verbose"),
            SeverityLevel.Verbose,
            new Dictionary<string, string>
            {
                { "VerboseProp", "VerboseVal" }
            });

        logger.Log(
            FormatLogMessage("Log - Critical"),
            SeverityLevel.Critical,
            new Dictionary<string, string>
            {
                { "Critical", "CriticalVal" }
            });

        logger.Log(
            FormatLogMessage("Log - empty props"),
            SeverityLevel.Information,
            new Dictionary<string, string>());

        logger.Log(
            FormatLogMessage("Log - null props"),
            SeverityLevel.Information,
            null!);

        // Log an exception with properties
        logger.Exception(
            new ArgumentException("Test exception"),
            new Dictionary<string, string>
            {
                { "ExceptionProperty", "ExceptionValue" }
            });

        logger.Exception(
            new ArgumentException("Test exception"),
            new Dictionary<string, string>
            {
                { "ExceptionProperty", "ExceptionValue" }
            });

        // Log an event with properties and metrics
        logger.Event(
            "IntegrationTestEvent",
            new Dictionary<string, string>
            {
                { "EventProperty", "EventValue" }
            },
            new Dictionary<string, double>
            {
                { "Metric1", 123.45 }
            },
            DateTimeOffset.UtcNow);

        // Flush the logger to ensure all logs are written
        logger.Flush();
    }

    [TestMethod]
    [TestCategoryLocal]
    public void Logger_Constructor_ShouldLoadConfigurationFromCustomSection()
    {
        // Arrange & Act
        ILogger logger = null!;

        // This tests that ReadFrom.Configuration properly finds ifx:telemetry:logging:serilog
        Should.NotThrow(() => { logger = new Logger(); });

        // Assert - If we got here without exception, config loaded successfully
        logger.ShouldNotBeNull();

        // Verify logger is functional by logging something
        Should.NotThrow(() =>
        {
            logger.Log("Configuration test", SeverityLevel.Information, null!);
            logger.Flush();
        });
    }

    [TestMethod]
    [TestCategoryLocal]
    public void Logger_Constructor_ShouldConfigureMultipleSinks()
    {
        // Arrange & Act - appsettings.json has SQL, File, and Console sinks configured
        ILogger logger = new Logger();

        // Assert - Verify logger accepts logs without throwing
        Should.NotThrow(() =>
        {
            logger.Log("Multi-sink test", SeverityLevel.Information, null!);
            logger.Exception(new InvalidOperationException("Test"), null!);
            logger.Event("TestEvent", null!, null!, DateTimeOffset.UtcNow);
            logger.Flush();
        });
    }
}