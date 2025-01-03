namespace DontPanicLabs.Ifx.IoC.Contracts.Exceptions;

/// <summary>
/// Exception thrown when an error occurs while building the container.
/// This is most likely an issue with your builder options. See inner exception for more details.
/// </summary>
public sealed class IoCContainerBuildException(string message, Exception innerException)
    : InvalidOperationException(message, innerException)
{
}