using DontPanicLabs.Ifx.Telemetry.Logger.Azure.ApplicationInsights.Exceptions;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.Tests.ApplicationInsights.InvalidTelemetryChannel;

[TestClass]
[TestCategoryCI]
public class Test
{
    [TestMethod]
    public void CreateLogger_AppInsightsTelemetryChannelInvalid_ThrowsInvalidTelemetryChannelException()
    {
        var exception = Assert.ThrowsException<InvalidTelemetryChannelException>(() =>
        {
            var logger = new Azure.ApplicationInsights.Logger();
        });
    }
}