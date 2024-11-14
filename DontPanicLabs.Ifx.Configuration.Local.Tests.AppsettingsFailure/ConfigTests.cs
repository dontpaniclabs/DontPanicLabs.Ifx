using DontPanicLabs.Ifx.Configuration.Contracts;
using DontPanicLabs.Ifx.Configuration.Local;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DPL.Ifx.Configuration.Tests.AppsettingsFailure
{
    [TestClass]
    public class ConfigTests
    {
        [TestMethod]
        [TestCategoryCI]
        public void Config_AppsettingsIsMissingDefaultSections_RetrievalShouldThrow()
        {
            IConfig config = new Config();
            
            var aggEx = Assert.ThrowsException<AggregateException>(() => config["appSettings:thisdoesntmatter"]);

            var ifxEx = aggEx.InnerExceptions.FirstOrDefault(ex => ex.Message == "Section 'ifx' not found in configuration.");
            var appEx = aggEx.InnerExceptions.FirstOrDefault(ex => ex.Message == "Section 'appSettings' not found in configuration.");
            
            Assert.IsNotNull(ifxEx, "A configuration section of 'ifx' was found in one of the configuration providers.");       
            Assert.IsNotNull(appEx, "A configuration section of 'appSettings' was found in one of the configuration providers.");
        }
    }
}