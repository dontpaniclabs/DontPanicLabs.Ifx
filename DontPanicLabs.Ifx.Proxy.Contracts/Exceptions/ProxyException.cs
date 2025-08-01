using System.Diagnostics.CodeAnalysis;

namespace DontPanicLabs.Ifx.Proxy.Contracts.Exceptions;

/// <summary>
/// Represents an exception that occurs when a proxy is misconfigured or used incorrectly.
/// </summary>
public class ProxyException : Exception
{
    public ProxyException(string? message) : base(message)
    {
    }

    public ProxyException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public static void ThrowIfTrue([DoesNotReturnIf(true)] bool condition, string message)
    {
        if (condition)
        {
            throw new ProxyException(message);
        }
    }

    public static void ThrowIfFalse([DoesNotReturnIf(false)] bool condition, string message)
    {
        ThrowIfTrue(!condition, message);
    }
}