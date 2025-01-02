using System.Diagnostics.CodeAnalysis;

namespace DontPanicLabs.Ifx.IoC.Contracts.Exceptions
{
    /// <summary>
    /// Exception thrown whenever a service is requested and cannot be found in the container.
    /// This is most likely caused by a configuration issue.
    /// </summary>
    public sealed class IoCServiceNotFoundException(string message) : InvalidOperationException(message)
    {
        private const string ServiceNotFoundMessage = "Service of type '{0}' was not registered. " +
                                                      "Did you forget to add it to the container?";

        public static void ThrowIfFalse([DoesNotReturnIf(false)] bool condition, Type type)
        {
            if (!condition)
            {
                throw new IoCServiceNotFoundException(string.Format(ServiceNotFoundMessage, type.Name));
            }
        }

        public static void ThrowIfNull([NotNull] object? service, Type type)
        {
            ThrowIfFalse(service is not null, type);
        }
    }
}