using DontPanicLabs.Ifx.Telemetry.Logger.Contracts;
using DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Exceptions;
using DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Tests.TestHelpers;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using Serilog;
using Serilog.Events;
using Shouldly;
using ILogger = DontPanicLabs.Ifx.Telemetry.Logger.Contracts.ILogger;

namespace DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Tests;

[TestClass]
[TestCategoryCI]
public class LoggerTests
{
    private TestSink _testSink = null!;
    private ILogger _logger = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _testSink = new TestSink();
        var serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(_testSink)
            .CreateLogger();

        _logger = new Logger(serilogLogger);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        (_logger as IDisposable)?.Dispose();
    }

    [TestMethod]
    [DataRow(SeverityLevel.Verbose, LogEventLevel.Verbose)]
    [DataRow(SeverityLevel.Information, LogEventLevel.Information)]
    [DataRow(SeverityLevel.Warning, LogEventLevel.Warning)]
    [DataRow(SeverityLevel.Error, LogEventLevel.Error)]
    [DataRow(SeverityLevel.Critical, LogEventLevel.Fatal)]
    public void Log_SeverityLevel_ShouldLogWithCorrectLogLevel(SeverityLevel severityLevel, LogEventLevel expectedLogLevel)
    {
        // Arrange
        var message = "Severity level test message";
        var properties = new Dictionary<string, string>();

        // Act
        _logger.Log(message, severityLevel, properties);

        // Assert
        _testSink.LogEvents.Count.ShouldBe(1);
        var logEvent = _testSink.LogEvents[0];
        logEvent.Level.ShouldBe(expectedLogLevel);
        logEvent.RenderMessage().ShouldBe(message);
    }

    [TestMethod]
    public void Log_WithProperties_ShouldIncludePropertiesInLogEvent()
    {
        // Arrange
        var message = "Test message with properties";
        var properties = new Dictionary<string, string>
        {
            { "prop1", "value1" },
            { "prop2", "value2" }
        };

        // Act
        _logger.Log(message, SeverityLevel.Information, properties);

        // Assert
        _testSink.LogEvents.Count.ShouldBe(1);
        var logEvent = _testSink.LogEvents[0];
        logEvent.Properties.ContainsKey("prop1").ShouldBeTrue();
        logEvent.Properties["prop1"].ToString().ShouldContain("value1");
        logEvent.Properties.ContainsKey("prop2").ShouldBeTrue();
        logEvent.Properties["prop2"].ToString().ShouldContain("value2");
    }

    [TestMethod]
    public void Log_WithEmptyProperties_ShouldLogSuccessfully()
    {
        // Arrange
        var message = "Test message without properties";
        var properties = new Dictionary<string, string>();

        // Act
        _logger.Log(message, SeverityLevel.Information, properties);

        // Assert
        _testSink.LogEvents.Count.ShouldBe(1);
        var logEvent = _testSink.LogEvents[0];
        logEvent.RenderMessage().ShouldBe(message);
    }

    [TestMethod]
    public void Exception_WithException_ShouldLogError()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var properties = new Dictionary<string, string>();

        // Act
        _logger.Exception(exception, properties);

        // Assert
        _testSink.LogEvents.Count.ShouldBe(1);
        var logEvent = _testSink.LogEvents[0];
        logEvent.Level.ShouldBe(LogEventLevel.Error);
        logEvent.Exception.ShouldBe(exception);
        logEvent.RenderMessage().ShouldContain("Test exception");
    }

    [TestMethod]
    public void Exception_WithProperties_ShouldIncludePropertiesInLogEvent()
    {
        // Arrange
        var exception = new ArgumentException("Test argument exception");
        var properties = new Dictionary<string, string>
        {
            { "userId", "12345" },
            { "action", "processing" }
        };

        // Act
        _logger.Exception(exception, properties);

        // Assert
        _testSink.LogEvents.Count.ShouldBe(1);
        var logEvent = _testSink.LogEvents[0];
        logEvent.Level.ShouldBe(LogEventLevel.Error);
        logEvent.Exception.ShouldBe(exception);
        logEvent.Properties.ContainsKey("userId").ShouldBeTrue();
        logEvent.Properties["userId"].ToString().ShouldContain("12345");
        logEvent.Properties.ContainsKey("action").ShouldBeTrue();
        logEvent.Properties["action"].ToString().ShouldContain("processing");
    }

    [TestMethod]
    public void Event_WithEventNameOnly_ShouldLogInformationWithEventName()
    {
        // Arrange
        const string eventName = "TestEvent";
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        _logger.Event(eventName, new Dictionary<string, string>(), new Dictionary<string, double>(), timestamp);

        // Assert
        _testSink.LogEvents.Count.ShouldBe(1);
        var logEvent = _testSink.LogEvents[0];
        logEvent.Level.ShouldBe(LogEventLevel.Information);
        logEvent.Properties.ContainsKey("EventName").ShouldBeTrue();
        logEvent.Properties["EventName"].ToString().ShouldContain(eventName);
    }

    [TestMethod]
    public void Event_WithPropertiesAndMetrics_ShouldLogStructuredEvent()
    {
        // Arrange
        const string eventName = "TestEvent";
        var properties = new Dictionary<string, string>
        {
            { "prop1", "value1" },
            { "prop2", "value2" }
        };
        var metrics = new Dictionary<string, double>
        {
            { "metric1", 123.45 },
            { "metric2", 67.89 }
        };
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        _logger.Event(eventName, properties, metrics, timestamp);

        // Assert
        _testSink.LogEvents.Count.ShouldBe(1);
        var logEvent = _testSink.LogEvents[0];
        logEvent.Level.ShouldBe(LogEventLevel.Information);
        logEvent.Properties.ContainsKey("EventName").ShouldBeTrue();
        logEvent.Properties["EventName"].ToString().ShouldContain(eventName);
        logEvent.Properties.ContainsKey("prop1").ShouldBeTrue();
        logEvent.Properties["prop1"].ToString().ShouldContain("value1");
        logEvent.Properties.ContainsKey("prop2").ShouldBeTrue();
        logEvent.Properties["prop2"].ToString().ShouldContain("value2");
        logEvent.Properties.ContainsKey("metric1").ShouldBeTrue();
        logEvent.Properties["metric1"].ToString().ShouldContain("123.45");
        logEvent.Properties.ContainsKey("metric2").ShouldBeTrue();
        logEvent.Properties["metric2"].ToString().ShouldContain("67.89");
    }

    [TestMethod]
    public void Event_WithPropertiesOnly_ShouldLogEventWithProperties()
    {
        // Arrange
        const string eventName = "TestEventWithProps";
        var properties = new Dictionary<string, string>
        {
            { "userId", "12345" },
            { "action", "login" }
        };
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        _logger.Event(eventName, properties, new Dictionary<string, double>(), timestamp);

        // Assert
        _testSink.LogEvents.Count.ShouldBe(1);
        var logEvent = _testSink.LogEvents[0];
        logEvent.Level.ShouldBe(LogEventLevel.Information);
        logEvent.Properties.ContainsKey("userId").ShouldBeTrue();
        logEvent.Properties["userId"].ToString().ShouldContain("12345");
        logEvent.Properties.ContainsKey("action").ShouldBeTrue();
        logEvent.Properties["action"].ToString().ShouldContain("login");
    }

    [TestMethod]
    public void Event_WithMetricsOnly_ShouldLogEventWithMetrics()
    {
        // Arrange
        const string eventName = "TestEventWithMetrics";
        var metrics = new Dictionary<string, double>
        {
            { "duration", 1234.56 },
            { "count", 42 }
        };
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        _logger.Event(eventName, new Dictionary<string, string>(), metrics, timestamp);

        // Assert
        _testSink.LogEvents.Count.ShouldBe(1);
        var logEvent = _testSink.LogEvents[0];
        logEvent.Level.ShouldBe(LogEventLevel.Information);
        logEvent.Properties.ContainsKey("duration").ShouldBeTrue();
        logEvent.Properties["duration"].ToString().ShouldContain("1234.56");
        logEvent.Properties.ContainsKey("count").ShouldBeTrue();
        logEvent.Properties["count"].ToString().ShouldContain("42");
    }

    [TestMethod]
    public void EmptyConnectionStringException_ThrowIfEmpty_WithEmptyString_ShouldThrow()
    {
        // Act & Assert
        Should.Throw<EmptyConnectionStringException>(() =>
            EmptyConnectionStringException.ThrowIfEmpty("")
        );
    }

    [TestMethod]
    public void EmptyConnectionStringException_ThrowIfEmpty_WithValidString_ShouldNotThrow()
    {
        // Act & Assert
        Should.NotThrow(() =>
            EmptyConnectionStringException.ThrowIfEmpty("valid connection string")
        );
    }
}