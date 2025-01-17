using DontPanicLabs.Ifx.Telemetry.Logger.Contracts;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.Tests.ApplicationInsights.Success
{
    [TestClass]
    [TestCategoryLocal]
    public class LoggerAppInsightsTest
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
        public void Cleanup()
        {
            _logger!.Flush();
        }

        [TestMethod]
        public void LogAllSeverities()
        {
            var severities = Enum.GetValues(typeof(SeverityLevel));

            foreach (var severity in severities)
            { 
                _logger!.Log("Unit Test Log Message", (SeverityLevel)severity, _customProperties!);
            }
        }

        [TestMethod]
        public void LogEvents()
        {
            var metrics = new Dictionary<string, double>
            {
                {"TestMetric", 100 }
            };

            _logger!.Event("Unit Test Event", _customProperties!, metrics, DateTime.UtcNow);
        }

        [TestMethod]
        public void LogExceptions()
        {
            _logger!.Exception(new Exception("Unit Test Exception"), _customProperties!);
        }
    }
}