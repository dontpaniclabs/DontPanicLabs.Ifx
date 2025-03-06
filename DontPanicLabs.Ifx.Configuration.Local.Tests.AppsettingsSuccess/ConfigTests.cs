using System.Reflection;
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
            CopyAppSettingsToCurrentDirectory("appsettings.json");
            IConfig config = new Config();
            var value = config[key];
            Assert.AreEqual(expectedValue, value);
        }

        [TestMethod]
        public void Config_SectionExistsButEmpty()
        {
            CopyAppSettingsToCurrentDirectory("appsettings.empty.json");
            IConfig config = new Config();
            Assert.AreEqual("world", config["hello"]);
        }
        
        /// <summary>
        /// Copies the appSettingsfileName from the embedded resource to the environments current directory as "appsettings.json".
        /// Use this function to test different appsettings files.
        /// </summary>
        /// <param name="appSettingsFileName">The appsettings file name you want copied.</param>
        private void CopyAppSettingsToCurrentDirectory(string appSettingsFileName)
        {
            var appsettingsPath = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
            File.Delete(appsettingsPath);
            
            var resourceName = $"{GetType().Namespace}.{appSettingsFileName}";
            using Stream output = File.Create(Path.Combine(Environment.CurrentDirectory, "appsettings.json"));
            using var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            resourceStream!.CopyTo(output);
        }
    }
}