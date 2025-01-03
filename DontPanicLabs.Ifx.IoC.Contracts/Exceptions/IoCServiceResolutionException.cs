namespace DontPanicLabs.Ifx.IoC.Contracts.Exceptions;

/// <summary>
/// Exception thrown when a service is found in the container but the resolution process fails.
/// This is most likely caused by a configuration issue. See inner exception for details.
/// </summary>
public class IoCServiceResolutionException(string message, Exception innerException)
    : Exception(message, innerException)
{
}