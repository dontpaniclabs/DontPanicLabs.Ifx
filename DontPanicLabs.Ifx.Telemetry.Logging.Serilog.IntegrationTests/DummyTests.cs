using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DontPanicLabs.Ifx.Telemetry.Logging.Serilog.IntegrationTests;

[TestClass]
[TestCategoryCI]
public class DummyTests
{
    [TestMethod]
    public void Dummy()
    {
        // This is just a placeholder to ensure the test project builds and runs in CI/CD pipelines, which fail if
        // a project contains no tests matching the targeted filter.
    }
}