using DontPanicLabs.Ifx.Configuration.Contracts;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;

namespace DontPanicLabs.Ifx.Configuration.Local.Tests.Debugging
{
    [TestClass]
    [TestCategoryLocal]
    public class ConfigTests
    {
        /* In order for these tests to succeed, you need to have the following
           in your secrets.json file for this test project:
         {
             "appsettings": {
               "appsettings": "appsettings secrets success"
             },
             "ifx": {
               "appsettings": "ifx secrets success",
               "onlyInSecretsValue": "only in secrets"
             }
         }
         */

        [DataTestMethod]
        [DataRow("appsettings:appsettings", "appsettings secrets success",
            DisplayName = "appsettings values can be overridden in secrets.json")]
        [DataRow("ifx:appsettings", "ifx secrets success",
            DisplayName = "ifx values can be overridden in secrets.json")]
        [DataRow("appsettings:onlyInAppsettingsValue", "only in appsettings.json",
            DisplayName = "non-overridden from appsettings.json values are returned")]
        [DataRow("ifx:onlyInSecretsValue", "only in secrets",
            DisplayName = "values not in appsettings.json can be pulled from secrets.json")]
        public void Config_UserSecretsLoaded(string key, string? expectedValue)
        {
            IConfig config = new Config();

            var value = config[key];

            Assert.AreEqual(expectedValue, value);
        }
    }
}