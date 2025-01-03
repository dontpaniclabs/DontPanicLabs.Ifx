using System.Diagnostics.CodeAnalysis;

namespace DontPanicLabs.Ifx.Proxy.Contracts.Exceptions
{
    /// <summary>
    /// Exception thrown when a type is not in or does not have a valid namespace
    /// to support the proxy pattern.
    /// </summary>
    public sealed class NamespaceException : SystemException
    {
        private const string NullMessage = "Namespace is null for the type '{0}'.";
        private const string NotSubsystemMessage = "Invalid subsystem. You can only use a Manager interface to access a subsystem.";
        private const string NotComponentMessage = "Invalid component call. You can only use an Engine or Accessor interface to access a component.";

        NamespaceException(string message) : base(message)
        {
        }

        public static void ThrowIfNamespaceNull([NotNull] Type type)
        {
            if (type.Namespace is null)
            {
                throw new NamespaceException(string.Format(NullMessage, type.FullName!));
            }
        }

        public static void ThrowIfNotSubsystem([NotNull] Type type)
        {
            ThrowIfNamespaceNull(type);

            if (!type.Namespace!.Contains("Manager"))
            {
                throw new NamespaceException(NotSubsystemMessage);
            }
        }

        public static void ThrowIfNotComponent([NotNull] Type type)
        {
            ThrowIfNamespaceNull(type);

            var ns = type.Namespace!;

            if (!ns.Contains("Engine") && !ns.Contains("Accessor"))
            {
                throw new NamespaceException(NotComponentMessage);
            }
        }
    }
}