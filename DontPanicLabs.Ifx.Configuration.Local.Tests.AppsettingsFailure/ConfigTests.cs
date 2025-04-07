using System.Reflection;
using DontPanicLabs.Ifx.Configuration.Contracts;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DontPanicLabs.Ifx.Configuration.Local.Tests.AppsettingsFailure
{
    [TestClass]
    [TestCategoryCI]
    public class ConfigTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            // Reset the config to ensure that the tests are not affected by previous tests.
            Config.Reset();
        }
        
        [TestMethod]
        public void Config_AppsettingsIsMissingDefaultSections_RetrievalShouldThrow()
        {
            CopyAppSettingsToCurrentDirectory("appsettings.json");
            IConfig config = new Config();
            
            var aggEx = Assert.ThrowsException<AggregateException>(() => config["appSettings:thisdoesntmatter"]);

            var ifxEx = aggEx.InnerExceptions.FirstOrDefault(ex => ex.Message == "'ifx' is missing from configuration or exists but is not nestable.");
            var appEx = aggEx.InnerExceptions.FirstOrDefault(ex => ex.Message == "'appSettings' is missing from configuration or exists but is not nestable.");
            
            Assert.IsNotNull(ifxEx, "A configuration section of 'ifx' was found in one of the configuration providers.");       
            Assert.IsNotNull(appEx, "A configuration section of 'appSettings' was found in one of the configuration providers.");
        }

        [TestMethod]
        [Description("This test is to ensure that the ifx and appsettings are nested.")]
        public void Config_AppsettingsNotNested_RetrievalShouldThrow()
        {
            CopyAppSettingsToCurrentDirectory("appsettings.invalidtype.json");
            IConfig config = new Config();

            var aggEx = Assert.ThrowsException<AggregateException>(() => config["appsettings"]);
            
            var ifxEx = aggEx.InnerExceptions.FirstOrDefault(ex => ex.Message == "'ifx' is missing from configuration or exists but is not nestable.");
            var appEx = aggEx.InnerExceptions.FirstOrDefault(ex => ex.Message == "'appSettings' is missing from configuration or exists but is not nestable.");
            
            Assert.IsNotNull(ifxEx, "A configuration section of 'ifx' was found in one of the configuration providers.");
            Assert.IsNotNull(appEx, "A configuration section of 'appSettings' was found in one of the configuration providers.");
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