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

        /// <summary>
        /// DO NOT MERGE: A "demo" test showing memory behavior of multiple instantiations of `Logger` to be used in an
        /// "A / B" test showing a failing test (OOM exception) when `Logger` is not disposed. A commit will then
        /// be added  showing no OOM occurs after we `.Dispose()` the logger.
        /// </summary>
        [TestMethod]
        public void MultipleInstantiations_ShouldNotLeakResources()
        {
            // Force GC before baseline snapshot
            GC.Collect();
            GC.WaitForPendingFinalizers();
            long totalMemBefore = GC.GetTotalMemory(true);

            for (int i = 0; i < 250000; i++)
            {
                // Instantiate `Logger` without `Dispose` to show memory leak
                var logger = new Azure.ApplicationInsights.Logger();
                // logger.Dispose();
            }

            // Force GC again; 5Mb memory growth is just used as a rough heuristic for "acceptable" memory growth; the
            // "A/B" test should be convincing without needing GC to do anything specific (e.g. behave like we would
            // ideally want it to for
            GC.Collect();
            GC.WaitForPendingFinalizers();
            long totalMemAfter = GC.GetTotalMemory(true);

            long leak = (totalMemAfter - totalMemBefore);
            long leakThreshold = 5 * 1024 * 1024;

            Assert.IsTrue(leak < leakThreshold, $"Leaked too much memory: {leak}Mb");
        }
    }
}