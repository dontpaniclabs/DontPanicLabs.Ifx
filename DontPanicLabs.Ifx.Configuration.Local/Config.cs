using DontPanicLabs.Ifx.Configuration.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace DontPanicLabs.Ifx.Configuration.Local
{
    public class Config : IConfig, IConfiguration
    {
        protected const string IfxSectionPrefix = "ifx";
        protected const string AppSectionPrefix = "appSettings";
        protected const string UserSecretsIdKey = "userSecretsId";
        protected const string SkipEnvironmentVariablesKey = "skipEnvironmentVariables";

        protected static readonly Lazy<IConfiguration> _Configuration = new Lazy<IConfiguration>(BuildConfiguration, LazyThreadSafetyMode.ExecutionAndPublication);

        public string? this[string key] { get => _Configuration.Value[key]; set => _Configuration.Value[key] = value; }

        protected static IConfiguration BuildConfiguration()
        {
            // Build Config 
            var configBuilder = GetConfigurationBuilder();

            // Build and cache runtime configs
            var configRoot = configBuilder.Build();

            // Validate required default sections
            ValidateConfigSections(configRoot);

            return configRoot;
        }

        protected static string? GetUserSecretsId()
        {
            // Get Secrets ID from appsettings.json file
            var secretsConfig = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", true)
                .Build();

            var secretsId = secretsConfig[$"{IfxSectionPrefix}:{UserSecretsIdKey}"];

            return secretsId;
        }

        protected static bool SkipEnvironmentVariables()
        {
            bool skipEnvironmentVariables = false;

            // Look for flag to skip loading environment variables in appsettings.json file
            var envVarConfig = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", true)
                .Build();

            string? value = envVarConfig[$"{SkipEnvironmentVariablesKey}"];

            if (value != null)
            { 
                skipEnvironmentVariables = bool.Parse(value);
            }

            return skipEnvironmentVariables;
        }

        protected static IConfigurationBuilder GetConfigurationBuilder()
        {
            // Always include Environment Varibles and appsettings.json
            var configBuilder = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory);

            if (!SkipEnvironmentVariables())
            {
                configBuilder.AddEnvironmentVariables();
            }
                
            configBuilder.AddJsonFile("appsettings.json", true);

            // Get the usersecrets.json file id if specified
            var secretsId = GetUserSecretsId();

            // Add user secrets if id found
            if (!string.IsNullOrEmpty(secretsId))
            {
                configBuilder.AddUserSecrets(secretsId);
            }

            return configBuilder;
        }

        protected static void ValidateConfigSections(IConfigurationRoot configRoot)
        {
            List<Exception> exceptions = new List<Exception>();

            try
            {
                configRoot.GetRequiredSection(IfxSectionPrefix);
            }
            catch (InvalidOperationException ex)
            {
                exceptions.Add(ex);
            }

            try
            {
                configRoot.GetRequiredSection(AppSectionPrefix);
            }
            catch (InvalidOperationException ex)
            {
                exceptions.Add(ex);
            }

            if (exceptions.Count > 0)
            {
                var aggregate = new AggregateException(exceptions);

                throw aggregate;
            }

        }

        IConfigurationSection IConfiguration.GetSection(string key)
        {
            return _Configuration.Value.GetSection(key);
        }

        IEnumerable<IConfigurationSection> IConfiguration.GetChildren()
        {
            return _Configuration.Value.GetChildren();
        }

        IChangeToken IConfiguration.GetReloadToken()
        {
            return _Configuration.Value.GetReloadToken();
        }
    }
}
