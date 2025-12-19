using DontPanicLabs.Ifx.Configuration.Contracts;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using Microsoft.Extensions.Configuration;

namespace DontPanicLabs.Ifx.Configuration.Local.Tests.ConfigurationOrder;

/* To test User Secrets in the configuration priority chain, add the following
   to the User Secrets for this test project:
   {
     "SecretKey": "SecretValue"
   }
*/

[TestClass]
[TestCategoryCI]
public class ConfigTests
{
    [TestMethod]
    [Description("Development file overrides base appsettings.json values")]
    public void Config_DevelopmentFileOverridesBase()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        IConfig config = new RebuildableConfig();

        // Development file should override base values
        Assert.AreEqual("DevelopmentValue", config["TestKey"]);
        Assert.AreEqual("DevelopmentValue", config["ChainTest"]);

        // Keys only in Development file should be available
        Assert.AreEqual("DevOnlyValue", config["DevOnly"]);

        // Keys only in base file should still be available
        Assert.AreEqual("BaseOnlyValue", config["BaseOnly"]);
    }

    [TestMethod]
    [Description("Environment variables have highest priority in the chain")]
    public void Config_EnvVarHasHighestPriority()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        Environment.SetEnvironmentVariable("ChainTest", "EnvVarValue");

        IConfig config = new RebuildableConfig();

        // Env var should override both appsettings.json and appsettings.Development.json
        Assert.AreEqual("EnvVarValue", config["ChainTest"]);
    }

    [TestMethod]
    [Description("Tests complete configuration priority chain including User Secrets")]
    public void Config_UserSecretsInPriorityChain()
    {
        // Without env var, User Secret should override base value
        IConfig config = new RebuildableConfig();
        var secretValue = config["SecretKey"];

        // Verify User Secrets are configured - should be "SecretValue", not "BaseValue"
        Assert.AreEqual("SecretValue", secretValue,
            "SecretKey should be 'SecretValue' from User Secrets. Please configure User Secrets as described in the comment at the top of this file.");

        // With env var set, env var should override even User Secrets
        Environment.SetEnvironmentVariable("SecretKey", "EnvVarValue");
        config = new RebuildableConfig();

        Assert.AreEqual("EnvVarValue", config["SecretKey"],
            "Environment variable should override User Secrets");
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
