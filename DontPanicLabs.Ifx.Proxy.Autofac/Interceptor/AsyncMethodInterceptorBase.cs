using System.Reflection;
using Castle.DynamicProxy;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Interceptor;

/// <summary>
/// Base interceptor class that specifically targets method calls while ignoring property gets and sets.
/// Subclasses should implement the <see cref="InterceptMethodInvocation"/> method to define custom behavior
/// and call .Invoke() on the <see cref="IInvocationProceedInfo"/> to continue the method execution when done.
/// </summary>
public abstract class AsyncMethodInterceptorBase : IInterceptor
{
    /// <summary>
    /// This property keeps track of how many times the interceptor has been invoked.
    /// It's useful for testing and debugging purposes to ensure that the interceptor is being called as expected.
    /// If used improperly, interceptors can get stuck in loops.
    /// </summary>
    internal int InterceptionCount { get; private set; }

    public void Intercept(IInvocation invocation)
    {
        // Proceed info must be captured ahead of any async calls that might come before the invocation proceeds.
        var proceed = invocation.CaptureProceedInfo();

        if (IsMethodInvocation(invocation.Method))
        {
            InterceptionCount++;

            InterceptMethodInvocation(proceed, invocation);
        }
        else
        {
            proceed.Invoke();
        }
    }

    protected abstract void InterceptMethodInvocation(
        IInvocationProceedInfo invocationProceedInfo,
        IInvocation invocation
    );

    private static bool IsMethodInvocation(MethodInfo method)
    {
        return !method.IsSpecialName;
    }
}