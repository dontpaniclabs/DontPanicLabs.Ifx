using System.Diagnostics.CodeAnalysis;

namespace DontPanicLabs.Ifx.Proxy.Contracts.Exceptions
{
    public sealed class ProxyTypeLoadException : TypeLoadException
    {
        private const string TypeLoadError = "Could not create a type for the name '{0}'." +
                                             "Ensure your full type name and specified are correct." +
                                             "Example: 'Full.Type.Namespace.And.Name, Full.Assembly.Name'" +
                                             "If you did not specify an assembly, ensure the type exists in your current context.";

        public ProxyTypeLoadException(string typeName) 
            : base(string.Format(TypeLoadError, typeName))
        {
        }

        public static void ThrowIfNull([NotNull] Type? type, string typeName)
        {
            if (type is null)
            {
                throw new ProxyTypeLoadException(typeName);
            }
        }
    }
}