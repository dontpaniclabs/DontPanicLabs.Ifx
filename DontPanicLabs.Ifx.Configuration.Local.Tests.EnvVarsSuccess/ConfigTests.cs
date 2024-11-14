using DontPanicLabs.Ifx.Configuration.Contracts;
using DontPanicLabs.Ifx.Configuration.Local;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using Microsoft.Extensions.Configuration;
namespace DontPanicLabs.Ifx.Configuration.Local.Tests.EnvVarsSuccess
{
    [TestClass]
    [TestCategoryCI]
    public class ConfigTests
    {
        [DataTestMethod]
        [DataRow("appsettings:appsettings", "appsettings success", DisplayName = "appsettings values are returned")]
        [DataRow("ifx:appsettings", "ifx success", DisplayName = "ifx values are returned")]
        [DataRow("appsettings:missing", null, DisplayName = "missing keys returns null")]
        public void Config_EnvironmentVariablesLoaded(string key, string? expectedValue)
        {
            Environment.SetEnvironmentVariable("appsettings", "root setting required");
            Environment.SetEnvironmentVariable("ifx", "root setting required");
            IConfig config = new RebuildableConfig();
            Environment.SetEnvironmentVariable(key, expectedValue);
            var value = config[key];
            Assert.AreEqual(expectedValue, value);
        }
    }
}
/// <summary>
/// Class that allows the configuration to be rebuilt to test different environment variables.
/// </summary>
file class RebuildableConfig : Config
{
    public RebuildableConfig()
    {
        Rebuild();
    }
    private static void Rebuild()
    {
        _Configuration = new Lazy<IConfiguration>(BuildConfiguration, LazyThreadSafetyMode.ExecutionAndPublication);
    }
}