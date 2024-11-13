using DontPanicLabs.Ifx.Configuration.Contracts;
using DontPanicLabs.Ifx.Configuration.Local;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DPL.Ifx.Configuration.Tests.AppsettingsSuccess
{
    [TestClass]
    public class ManualVerificationTests
    {
        [TestMethod]
        [TestCategoryCI]
        public void Configuration_Appsettings_Success()
        {
            IConfig config = new Config();

            var appSettings = config["appsettings:appsettings"];
            var ifx = config["ifx:appsettings"];

            Assert.AreEqual("success", appSettings);
            Assert.AreEqual("success", ifx);
        }
    }
}