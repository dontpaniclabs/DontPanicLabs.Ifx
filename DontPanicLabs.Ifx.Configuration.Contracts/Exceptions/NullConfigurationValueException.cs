using System.Diagnostics.CodeAnalysis;

namespace DontPanicLabs.Ifx.Configuration.Contracts.Exceptions
{
    public sealed class NullConfigurationValueException : NullReferenceException
    {
        private const string NullConfigValue = "The configuration value is null. " +
                                               "Ensure the configuration key '{0}' exists and has a value.";

        public NullConfigurationValueException(string settingsKey)
            : base(string.Format(NullConfigValue, settingsKey))
        {
        }

        public static void ThrowIfNull([NotNull] object? property, string settingsKey)
        {
            if (property is null)
            {
                throw new NullConfigurationValueException(settingsKey);
            }
        }
    }
}