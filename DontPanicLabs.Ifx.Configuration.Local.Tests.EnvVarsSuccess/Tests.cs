using DontPanicLabs.Ifx.Configuration.Contracts;
using DontPanicLabs.Ifx.Configuration.Local;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DPL.Ifx.Configuration.Tests.EnvVarsSuccess
{
    [TestClass]
    public class ConfigTests
    {
        [TestMethod]
        [TestCategoryCI]
        public void Configuration_Environment_Variables_Success()
        {
            IConfig config = new Config();

            var appSettings = config["appSettings:envvar"];
            var ifx = config["ifx:envvar"];

            Assert.AreEqual("success", appSettings);
            Assert.AreEqual("success", ifx);
        }
    }
}