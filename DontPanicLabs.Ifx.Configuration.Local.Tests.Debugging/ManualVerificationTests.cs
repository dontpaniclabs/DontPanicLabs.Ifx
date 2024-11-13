using DontPanicLabs.Ifx.Configuration.Contracts;
using DontPanicLabs.Ifx.Configuration.Local;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DPL.Ifx.Configuration.Tests.Debugging
{
    [TestClass]
    public class ManualVerificationTests
    {
        [TestMethod]
        [Ignore]
        [TestCategoryLocal]
        public void Configuration_Local_UserSecrets()
        {
            // In order for this test to succeed:
            // 1. userSecrets.json must have the necessary settings
            // 2. appSettings.json must have an entry for the `userSecretsId`

            IConfig config = new Config();

            var appSettings = config["appSettings:file"];
            var ifx = config["ifx:file"];

            Assert.AreEqual("secrets.json", appSettings);
            Assert.AreEqual("secrets.json", ifx);
        }
    }
}