using Castle.DynamicProxy;

namespace DontPanicLabs.Ifx.Proxy.Autofac.Extensions.Interceptor;

public static class IInvocationExtensions
{
    public static T GetFirstArgument<T>(this IInvocation invocation)
    {
        if (invocation.Arguments.Length < 1)
        {
            throw new ArgumentException("Invocation must have at least one argument.");
        }

        if (invocation.Arguments[0] is not T)
        {
            throw new ArgumentException($"The first argument must be of type '{typeof(T).Name}'.");
        }

        return (T) invocation.Arguments[0];
    }

    public static Type GetTaskResultType(this IInvocation invocation)
    {
        if (invocation.Method.ReturnType.IsGenericType &&
            invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            return invocation.Method.ReturnType.GetGenericArguments()[0];
        }

        throw new ArgumentException(
            $"Method '{invocation.Method.Name}' must return a Task<T>."
        );
    }

    public static bool IsTaskReturning(this IInvocation invocation)
    {
        return invocation.Method.ReturnType.IsGenericType &&
               invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
    }
}