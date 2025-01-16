using System.Diagnostics.CodeAnalysis;

namespace DontPanicLabs.Ifx.Telemetry.Logger.Azure.ApplicationInsights.Exceptions
{
    /// <summary>
    /// Exception thrown when the AppInsights connection string is empty.
    /// This is a configuration issue.
    /// </summary>
    public sealed class EmptyConnectionStringException(string message)
        : ArgumentException(message)
    {
        private const string ConnectionStringEmpty = "The entry for the AppInsights connection string was found in configuration, " +
                                                     "but the value was blank or empty.";

        public static void ThrowIfEmpty(string connectionString)
        {
            if (connectionString.Length == 0)
            {
                throw new EmptyConnectionStringException(ConnectionStringEmpty);
            }
        }
    }
}