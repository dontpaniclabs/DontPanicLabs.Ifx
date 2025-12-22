using System.Text.Json;
using DontPanicLabs.Ifx.Configuration.Contracts;
using DontPanicLabs.Ifx.Tests.Shared.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace DontPanicLabs.Ifx.Configuration.Local.Tests.ConfigurationOrder;

[TestClass]
[TestCategoryCI]
public class ConfigTests
{
    private readonly string _userSecretsId = "dpl.ifx.configuration.tests.configurationorder";

    [TestInitialize]
    public void Setup()
    {
        // Get the secrets file path                                                                                                                                                                                                                                                                                                                       
        var secretsPath = PathHelper.GetSecretsPathFromSecretsId(_userSecretsId);
        
        // Ensure directory exists                                                                                                                                                                                                                                                                                                                         
        Directory.CreateDirectory(Path.GetDirectoryName(secretsPath));
        
        // Write your test secrets                                                                                                                                                                                                                                                                                                                         
        var secrets = new
        {
            PyramidKey1 = "FromUserSecrets",
            PyramidKey2 = "FromUserSecrets"
        };
        File.WriteAllText(secretsPath, JsonSerializer.Serialize(secrets));
    }

    [TestMethod]
    [Description("Tests complete configuration priority pyramid: Base -> Development -> User Secrets -> Environment Variables")]
    public void Config_PyramidPriorityChain()
    {
        // Set environment to Development and configure the top-level override
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        Environment.SetEnvironmentVariable("PyramidKey1", "FromEnvironmentVariable");

        IConfig config = new RebuildableConfig();

        // PyramidKey1: Should be from Environment Variable (highest priority - overrides all 3 lower levels)
        Assert.AreEqual("FromEnvironmentVariable", config["PyramidKey1"],
            "PyramidKey1 should come from Environment Variable (top of pyramid - priority 4/4)");

        // PyramidKey2: Should be from User Secrets (overrides 2 lower levels: Development and Base)
        var secretPath = PathHelper.GetSecretsPathFromSecretsId(_userSecretsId);
        var secretsJson = File.ReadAllText(secretPath);
        var secretsDict = JsonSerializer.Deserialize<Dictionary<string, string>>(secretsJson);
        Assert.IsTrue(secretsDict.ContainsKey("PyramidKey1"), "PyramidKey1 should exist in User Secrets");
        Assert.IsTrue(secretsDict.ContainsKey("PyramidKey2"), "PyramidKey2 should exist in User Secrets");

        Assert.AreEqual("FromUserSecrets", config["PyramidKey2"],
            "PyramidKey2 should come from User Secrets (priority 3/4). Please configure User Secrets for this test project to run this test.");
        
        // PyramidKey3: Should be from Development file (overrides 1 lower level: Base)
        var devJson = File.ReadAllText("appsettings.Development.json");
        var devDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(devJson);
        Assert.IsTrue(devDict.ContainsKey("PyramidKey1"), "PyramidKey1 should exist in appsettings.Development.json");
        Assert.IsTrue(devDict.ContainsKey("PyramidKey2"), "PyramidKey2 should exist in appsettings.Development.json");
        Assert.IsTrue(devDict.ContainsKey("PyramidKey3"), "PyramidKey3 should exist in appsettings.Development.json");
    
        Assert.AreEqual("FromDevelopment", config["PyramidKey3"],
            "PyramidKey3 should come from appsettings.Development.json (priority 2/4)");

        // PyramidKey4: Should be from Base appsettings.json (no overrides - lowest priority)
        var baseJson = File.ReadAllText("appsettings.json");
        var baseDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(baseJson);
        Assert.IsTrue(baseDict.ContainsKey("PyramidKey1"), "PyramidKey1 should exist in appsettings.json");
        Assert.IsTrue(baseDict.ContainsKey("PyramidKey2"), "PyramidKey2 should exist in appsettings.json");
        Assert.IsTrue(baseDict.ContainsKey("PyramidKey3"), "PyramidKey3 should exist in appsettings.json");
        Assert.IsTrue(baseDict.ContainsKey("PyramidKey4"), "PyramidKey4 should exist in appsettings.json");
    
        Assert.AreEqual("FromBase", config["PyramidKey4"],
            "PyramidKey4 should come from appsettings.json (base of pyramid - priority 1/4)");
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
