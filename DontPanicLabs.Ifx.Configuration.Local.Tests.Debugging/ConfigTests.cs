using DontPanicLabs.Ifx.Configuration.Contracts;
using DontPanicLabs.Ifx.Configuration.Local;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DPL.Ifx.Configuration.Tests.Debugging
{
    [TestClass]
    public class ConfigTests
    {
        [TestMethod]
        [Ignore]
        [TestCategoryLocal]
        public void Config_Local_UserSecretsLoaded()
        {
            // In order for this test to succeed:
            // 1. secrets.json must have the necessary settings
            // 2. appsettings.json must have an entry for the `userSecretsId`

            IConfig config = new Config();

            var appSettings = config["appSettings:file"];
            var ifx = config["ifx:file"];

            Assert.AreEqual("secrets.json", appSettings);
            Assert.AreEqual("secrets.json", ifx);
        }
    }
}