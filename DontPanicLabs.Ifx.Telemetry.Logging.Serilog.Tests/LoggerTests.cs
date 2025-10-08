using DontPanicLabs.Ifx.Telemetry.Logger.Contracts;
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
        // Configure our test logger with our test sink that collects log events in memory
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
    public void Log_SeverityLevel_ShouldLogWithCorrectLogLevel(SeverityLevel severityLevel,
        LogEventLevel expectedLogLevel)
    {
        // Arrange
        var message = "Boop";
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
        var message = "Boop with properties";
        var properties = new Dictionary<string, string>
        {
            { "bleep", "hey" },
            { "blorp", "world" }
        };

        // Act
        _logger.Log(message, SeverityLevel.Information, properties);

        // Assert
        _testSink.LogEvents.Count.ShouldBe(1);
        var logEvent = _testSink.LogEvents[0];
        logEvent.Properties.ContainsKey("bleep").ShouldBeTrue();
        logEvent.Properties["bleep"].ToString().ShouldContain("hey");
        logEvent.Properties.ContainsKey("blorp").ShouldBeTrue();
        logEvent.Properties["blorp"].ToString().ShouldContain("world");
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
    public void Logger_WithMinimumLevel_ShouldFilterLowerLevelMessages()
    {
        // Arrange - Create a logger with MinimumLevel set to Warning
        var testSink = new TestSink();
        var serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Warning() // Only Warning and above
            .WriteTo.Sink(testSink)
            .CreateLogger();
        ILogger logger = new Logger(serilogLogger);

        // Act - Log at various levels
        logger.Log("Verbose message", SeverityLevel.Verbose, null!);
        logger.Log("Information message", SeverityLevel.Information, null!);
        logger.Log("Warning message", SeverityLevel.Warning, null!);
        logger.Log("Error message", SeverityLevel.Error, null!);
        logger.Log("Critical message", SeverityLevel.Critical, null!);

        // Assert - Only Warning, Error, and Critical should be logged
        testSink.LogEvents.Count.ShouldBe(3);
        testSink.LogEvents[0].Level.ShouldBe(LogEventLevel.Warning);
        testSink.LogEvents[1].Level.ShouldBe(LogEventLevel.Error);
        testSink.LogEvents[2].Level.ShouldBe(LogEventLevel.Fatal);

        // Cleanup
        (logger as IDisposable)?.Dispose();
    }

    [TestMethod]
    public void Logger_ConcurrentLogging_ShouldNotThrow()
    {
        // Arrange
        var tasks = new List<Task>();
        var exceptions = new List<Exception>();
        var messageCount = 0;

        // Act
        for (int i = 0; i < 10; i++)
        {
            int threadId = i;
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    for (int j = 0; j < 50; j++)
                    {
                        _logger.Log(
                            $"Thread {threadId} message {j}",
                            SeverityLevel.Information,
                            new Dictionary<string, string> { { "ThreadId", threadId.ToString() } });
                        Interlocked.Increment(ref messageCount);
                    }
                }
                catch (Exception ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert - Primary goal: no exceptions thrown during concurrent logging
        exceptions.ShouldBeEmpty();
        messageCount.ShouldBe(500); // All 500 log calls completed successfully
        // Note: _testSink.LogEvents.Count may vary due to async processing and test cleanup timing
    }

    [TestMethod]
    public void Logger_ConcurrentExceptionLogging_ShouldNotThrow()
    {
        // Arrange
        var tasks = new List<Task>();
        var exceptions = new List<Exception>();
        var exceptionCount = 0;

        // Act
        for (int i = 0; i < 10; i++)
        {
            int threadId = i;
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    for (int j = 0; j < 20; j++)
                    {
                        _logger.Exception(
                            new InvalidOperationException($"Thread {threadId} exception {j}"),
                            new Dictionary<string, string> { { "ThreadId", threadId.ToString() } });
                        Interlocked.Increment(ref exceptionCount);
                    }
                }
                catch (Exception ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert - Primary goal: no exceptions thrown during concurrent exception logging
        exceptions.ShouldBeEmpty();
        exceptionCount.ShouldBe(200); // All 200 exception log calls completed successfully
        // Note: _testSink.LogEvents.Count may vary due to async processing and test cleanup timing
    }

    [TestMethod]
    public void Logger_ConcurrentMixedOperations_ShouldNotThrow()
    {
        // Arrange
        var tasks = new List<Task>();
        var exceptions = new List<Exception>();
        var operationCount = 0;

        // Act - Mix of Log, Exception, and Event calls
        for (int i = 0; i < 15; i++)
        {
            int threadId = i;
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    _logger.Log($"Thread {threadId} log", SeverityLevel.Information, null!);
                    Interlocked.Increment(ref operationCount);

                    _logger.Exception(new InvalidOperationException($"Thread {threadId} exception"), null!);
                    Interlocked.Increment(ref operationCount);

                    _logger.Event($"Thread{threadId}Event", null!, null!, DateTimeOffset.UtcNow);
                    Interlocked.Increment(ref operationCount);
                }
                catch (Exception ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert - Primary goal: no exceptions thrown during concurrent mixed operations
        exceptions.ShouldBeEmpty();
        operationCount.ShouldBe(45); // All 45 operations completed successfully (15 threads * 3 operations)
        // Note: _testSink.LogEvents.Count may vary due to async processing and test cleanup timing
    }

    [TestMethod]
    public void Log_WithNullProperties_ShouldNotThrow()
    {
        // Act & Assert
        Should.NotThrow(() => { _logger.Log("Test message", SeverityLevel.Information, null!); });

        _testSink.LogEvents.Count.ShouldBe(1);
    }

    [TestMethod]
    public void Log_WithEmptyProperties_ShouldNotThrow()
    {
        // Act & Assert
        Should.NotThrow(() =>
        {
            _logger.Log("Test message", SeverityLevel.Information, new Dictionary<string, string>());
        });

        _testSink.LogEvents.Count.ShouldBe(1);
    }

    [TestMethod]
    public void Exception_WithNullProperties_ShouldNotThrow()
    {
        // Act & Assert
        Should.NotThrow(() => { _logger.Exception(new InvalidOperationException("Test"), null!); });

        _testSink.LogEvents.Count.ShouldBe(1);
    }

    [TestMethod]
    public void Exception_WithEmptyProperties_ShouldNotThrow()
    {
        // Act & Assert
        Should.NotThrow(() =>
        {
            _logger.Exception(new InvalidOperationException("Test"), new Dictionary<string, string>());
        });

        _testSink.LogEvents.Count.ShouldBe(1);
    }

    [TestMethod]
    public void Event_WithNullProperties_ShouldNotThrow()
    {
        // Act & Assert
        Should.NotThrow(() =>
        {
            _logger.Event("TestEvent", null!, new Dictionary<string, double>(), DateTimeOffset.UtcNow);
        });

        _testSink.LogEvents.Count.ShouldBe(1);
    }

    [TestMethod]
    public void Event_WithNullMetrics_ShouldNotThrow()
    {
        // Act & Assert
        Should.NotThrow(() =>
        {
            _logger.Event("TestEvent", new Dictionary<string, string>(), null!, DateTimeOffset.UtcNow);
        });

        _testSink.LogEvents.Count.ShouldBe(1);
    }

    [TestMethod]
    public void Event_WithBothNullPropertiesAndMetrics_ShouldNotThrow()
    {
        // Act & Assert
        Should.NotThrow(() => { _logger.Event("TestEvent", null!, null!, DateTimeOffset.UtcNow); });

        _testSink.LogEvents.Count.ShouldBe(1);
    }

    [TestMethod]
    public void Log_WithEmptyMessage_ShouldNotThrow()
    {
        // Act & Assert
        Should.NotThrow(() => { _logger.Log("", SeverityLevel.Information, null!); });

        _testSink.LogEvents.Count.ShouldBe(1);
    }

    [TestMethod]
    public void Log_WithVeryLongMessage_ShouldNotTruncate()
    {
        // Arrange
        var longMessage = new string('a', 5000);

        // Act
        _logger.Log(longMessage, SeverityLevel.Information, null!);

        // Assert
        _testSink.LogEvents.Count.ShouldBe(1);
        _testSink.LogEvents[0].RenderMessage().Length.ShouldBe(5000);
    }

    [TestMethod]
    public void Log_WithManyProperties_ShouldIncludeAll()
    {
        // Arrange
        var properties = Enumerable.Range(0, 50)
            .ToDictionary(i => $"prop{i}", i => $"value{i}");

        // Act
        _logger.Log("Test", SeverityLevel.Information, properties);

        // Assert
        var logEvent = _testSink.LogEvents[0];
        properties.Keys.ToList().ForEach(key => { logEvent.Properties.ContainsKey(key).ShouldBeTrue(); });
    }

    [TestMethod]
    public void Log_WithSpecialCharactersInPropertyValues_ShouldHandleCorrectly()
    {
        // Arrange
        var properties = new Dictionary<string, string>
        {
            { "prop1", "value with \"quotes\"" },
            { "prop2", "value with 'apostrophes'" },
            { "prop3", "value with\nnewlines" },
            { "prop4", "value with\ttabs" },
            { "prop5", "{\"json\": \"object\"}" }
        };

        // Act
        _logger.Log("Test", SeverityLevel.Information, properties);

        // Assert
        var logEvent = _testSink.LogEvents[0];
        properties.Keys.ToList().ForEach(key => { logEvent.Properties.ContainsKey(key).ShouldBeTrue(); });
    }

    [TestMethod]
    public void Exception_WithInnerException_ShouldLogBoth()
    {
        // Arrange
        var inner = new InvalidOperationException("Inner exception message");
        var outer = new ArgumentException("Outer exception message", inner);

        // Act
        _logger.Exception(outer, null!);

        // Assert
        var logEvent = _testSink.LogEvents[0];
        logEvent.Exception.ShouldNotBeNull().ShouldBe(outer);
        logEvent.Exception.InnerException.ShouldBe(inner);
        logEvent.Exception.InnerException!.Message.ShouldBe("Inner exception message");
    }

    [TestMethod]
    public void Exception_WithStackTrace_ShouldPreserveStackTrace()
    {
        // Arrange
        Exception exception;
        try
        {
            throw new InvalidOperationException("Test exception with stack trace");
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Act
        _logger.Exception(exception, null!);

        // Assert
        var logEvent = _testSink.LogEvents[0];
        logEvent.Exception.ShouldNotBeNull().StackTrace.ShouldNotBeNullOrEmpty();
    }

    [TestMethod]
    public void Logger_Flush_CalledMultipleTimes_ShouldNotThrow()
    {
        // Act & Assert
        Should.NotThrow(() =>
        {
            _logger.Flush();
            _logger.Flush();
            _logger.Flush();
        });
    }

    [TestMethod]
    public void Logger_DisposablePattern_ShouldWork()
    {
        // Act & Assert
        Should.NotThrow(() =>
        {
            var testSink = new TestSink();
            var serilogLogger = new LoggerConfiguration()
                .WriteTo.Sink(testSink)
                .CreateLogger();

            using var disposable = new Logger(serilogLogger) as IDisposable;
            var logger = disposable as ILogger;
            logger!.Log("Test", SeverityLevel.Information, null!);
        });
    }

    [TestMethod]
    public void Logger_DisposeViaInterface_ShouldWork()
    {
        // Arrange
        var testSink = new TestSink();
        var serilogLogger = new LoggerConfiguration()
            .WriteTo.Sink(testSink)
            .CreateLogger();
        var logger = new Logger(serilogLogger);

        // Purposeful cast to IDisposable
        var disposable = logger as IDisposable;

        // Act & Assert
        disposable.ShouldNotBeNull();
        Should.NotThrow(() => { disposable.Dispose(); });
    }

    [TestMethod]
    public void Logger_DisposeCalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var testSink = new TestSink();
        var serilogLogger = new LoggerConfiguration()
            .WriteTo.Sink(testSink)
            .CreateLogger();
        var logger = new Logger(serilogLogger);

        // Purposeful cast to IDisposable
        var disposable = logger as IDisposable;

        // Act & Assert
        Should.NotThrow(() =>
        {
            disposable.Dispose();
            disposable.Dispose();
            disposable.Dispose();
        });
    }
}