using DontPanicLabs.Ifx.Configuration.Contracts;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
namespace DontPanicLabs.Ifx.Configuration.Local.Tests.AppsettingsSuccess
{
    [TestClass]
    [TestCategoryCI]
    public class ConfigTests
    {
        [DataTestMethod]
        [DataRow("appsettings:appsettings", "appsettings success", DisplayName = "appsettings values are returned")]
        [DataRow("ifx:appsettings", "ifx success", DisplayName = "ifx values are returned")]
        [DataRow("appsettings:missing", null, DisplayName = "missing keys returns null")]
        public void Config_AppsettingsLoaded(string key, string? expectedValue)
        {
            IConfig config = new Config();
            var value = config[key];
            Assert.AreEqual(expectedValue, value);
        }
    }
}