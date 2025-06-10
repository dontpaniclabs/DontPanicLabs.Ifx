using DontPanicLabs.Ifx.Telemetry.Logger.Contracts;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.Tests.ApplicationInsights.ServerTelemetryChannel;

/// <summary>
/// To run this test locally, provide an app insights `ConnectionString` in this project's appsettings.json.
/// </summary>
[TestClass]
[TestCategoryLocal]
public class Test
{
    private Dictionary<string, string>? _customProperties;

    private ILogger? _logger;

    [TestInitialize]
    public void Initialize()
    {
        _customProperties = new Dictionary<string, string>
        {
            {"RunId", Guid.NewGuid().ToString() }
        };

        _logger = new Azure.ApplicationInsights.Logger();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        _logger!.Flush();

        // `Flush` is not synchronous when using `ServerTelemetryChannel`, wait a bit to allow flush to complete.
        await Task.Delay(10000);
    }

    [TestMethod]
    public void LogWithServerTelemetryChannel()
    {
        _logger!.Log("Unit Test Log Message using ServerTelemetryChannel", SeverityLevel.Information, _customProperties!);
    }
}