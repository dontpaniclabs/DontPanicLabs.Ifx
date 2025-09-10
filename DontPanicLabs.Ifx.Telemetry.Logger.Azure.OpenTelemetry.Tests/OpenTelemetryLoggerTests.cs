using DontPanicLabs.Ifx.Telemetry.Logger.Contracts;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.OpenTelemetry.Tests;

[TestClass]
public sealed class OpenTelemetryLoggerTests
{
    private ILoggerFactory _loggerFactory = null!;
    private ILogger _openTelemetryLogger = null!;
    private TestLoggerProvider _testLoggerProvider = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _testLoggerProvider = new TestLoggerProvider();
        _loggerFactory = LoggerFactory.Create(builder => { builder.AddProvider(_testLoggerProvider).SetMinimumLevel(LogLevel.Trace); });

        _openTelemetryLogger = new Logger(_loggerFactory);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _loggerFactory.Dispose();
    }

    [TestMethod]
    [DataRow(SeverityLevel.Verbose, LogLevel.Trace)]
    [DataRow(SeverityLevel.Information, LogLevel.Information)]
    [DataRow(SeverityLevel.Warning, LogLevel.Warning)]
    [DataRow(SeverityLevel.Error, LogLevel.Error)]
    [DataRow(SeverityLevel.Critical, LogLevel.Critical)]
    public void Log_SeverityLevel_ShouldLogWithCorrectLogLevel(SeverityLevel severityLevel, LogLevel expectedLogLevel)
    {
        // Act
        var message = "Severity level test message";
        _openTelemetryLogger.Log(message, severityLevel);

        // Assert
        _testLoggerProvider.LogEntries.Count.ShouldBe(1);
        var logEntry = _testLoggerProvider.LogEntries[0];
        logEntry.LogLevel.ShouldBe(expectedLogLevel);
        logEntry.Message.ShouldBe(message);
    }

    [TestMethod]
    public void Event_WithEventNameOnly_ShouldLogInformationWithCustomEventName()
    {
        // Arrange
        const string eventName = "TestEvent";
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        _openTelemetryLogger.Event(eventName, null, null, timestamp);

        // Assert
        _testLoggerProvider.LogEntries.Count.ShouldBe(1);
        var logEntry = _testLoggerProvider.LogEntries[0];
        logEntry.LogLevel.ShouldBe(LogLevel.Information);
        logEntry.Message.ShouldContain(eventName);
    }

    [TestMethod]
    public void Event_WithPropertiesAndMetrics_ShouldLogStructuredEvent()
    {
        // Arrange
        const string eventName = "TestEvent";
        var properties = new Dictionary<string, string>
        {
            {
                "prop1", "value1"
            },
            {
                "prop2", "value2"
            }
        };
        var metrics = new Dictionary<string, double>
        {
            {
                "metric1", 123.45
            },
            {
                "metric2", 67.89
            }
        };
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        _openTelemetryLogger.Event(eventName, properties, metrics, timestamp);

        // Assert
        _testLoggerProvider.LogEntries.Count.ShouldBe(1);
        var logEntry = _testLoggerProvider.LogEntries[0];
        logEntry.LogLevel.ShouldBe(LogLevel.Information);
        logEntry.Message.ShouldContain(eventName);
        logEntry.Message.ShouldContain("value1");
        logEntry.Message.ShouldContain("value2");
        logEntry.Message.ShouldContain("123.45");
        logEntry.Message.ShouldContain("67.89");
    }

    [TestMethod]
    public void Event_WithPropertiesOnly_ShouldLogEventWithProperties()
    {
        // Arrange
        const string eventName = "TestEventWithProps";
        var properties = new Dictionary<string, string>
        {
            {
                "userId", "12345"
            },
            {
                "action", "login"
            }
        };
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        _openTelemetryLogger.Event(eventName, properties, null, timestamp);

        // Assert
        _testLoggerProvider.LogEntries.Count.ShouldBe(1);
        var logEntry = _testLoggerProvider.LogEntries[0];
        logEntry.LogLevel.ShouldBe(LogLevel.Information);
        logEntry.Message.ShouldContain(eventName);
        logEntry.Message.ShouldContain("12345");
        logEntry.Message.ShouldContain("login");
    }

    [TestMethod]
    public void Event_WithMetricsOnly_ShouldLogEventWithMetrics()
    {
        // Arrange
        const string eventName = "TestEventWithMetrics";
        var metrics = new Dictionary<string, double>
        {
            {
                "duration", 1234.56
            },
            {
                "count", 42
            }
        };
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        _openTelemetryLogger.Event(eventName, null, metrics, timestamp);

        // Assert
        _testLoggerProvider.LogEntries.Count.ShouldBe(1);
        var logEntry = _testLoggerProvider.LogEntries[0];
        logEntry.LogLevel.ShouldBe(LogLevel.Information);
        logEntry.Message.ShouldContain(eventName);
        logEntry.Message.ShouldContain("1234.56");
        logEntry.Message.ShouldContain("42");
    }

    [TestMethod]
    public void Logger_MissingConfiguration_ShouldThrow()
    {
        // Act + Assert
        var ex = Should.Throw<AggregateException>(() =>
                new Logger() // No ILoggerFactory provided
        );
        ex.InnerExceptions.ShouldContain(e => e.Message.Contains("'ifx' is missing from configuration"));
    }
}