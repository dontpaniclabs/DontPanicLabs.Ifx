using DontPanicLabs.Ifx.Telemetry.Logger.Azure.ApplicationInsights.Exceptions;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.Tests.ApplicationInsights.NoConfigException
{
    [TestClass]
    [TestCategoryCI]
    public class LoggerAppInsightsNoConfigTests
    {
        [TestMethod]
        public void CreateLogger_AppInsightsConnectionStringEmpty_ThrowEmptyConnectionStringException()
        {
            var exception = Assert.ThrowsException<EmptyConnectionStringException>(() =>
            {
                var logger = new Azure.ApplicationInsights.Logger();
            });
        }
    }
}