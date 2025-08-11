# DontPanicLabs.Ifx.Proxy.Extensions

Extension methods and utilities for the Don't Panic Labs Infrastructure Framework (Ifx) proxy system. This package provides helpful extensions for testing, interceptor development, and proxy management.

## Features

### Interceptor Extensions
- **IInvocationExtensions**: Helper methods for working with Castle DynamicProxy invocations
  - `GetFirstArgument<T>()` - Safely extract and cast the first argument
  - `GetTaskResultType()` - Get the result type from Task<T> return types
  - `IsTaskReturning()` - Check if method returns a Task<T>

### Testing Utilities
- **MockProxyGenerator**: A test-friendly implementation of `IProxyGenerator` that returns mocks instead of real proxies
- **MockExtensions**: Extension methods for easily mocking dependencies in unit tests
- **MockProxyContext**: A simple test context for use with mock proxy generators

## Usage

### Unit Testing with Mock Dependencies

```csharp
// In your unit test
var serviceUnderTest = new MyProxyEnabledService();
var mockDependency = serviceUnderTest.MockDependency<IMyDependency>();

// Configure the mock
mockDependency.Setup(x => x.DoSomething()).Returns("test result");

// Test your service - it will receive the mock when it calls ProxyForService<IMyDependency>()
var result = await serviceUnderTest.ProcessData();
```

### Interceptor Development

```csharp
public class MyInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        if (invocation.IsTaskReturning())
        {
            var resultType = invocation.GetTaskResultType();
            // Handle async methods
        }
        
        var criteria = invocation.GetFirstArgument<MyCriteria>();
        // Process the criteria
        
        invocation.Proceed();
    }
}
```

## Installation

```bash
dotnet add package DontPanicLabs.Ifx.Proxy.Extensions
```

## Requirements

- .NET 8.0 or later
- DontPanicLabs.Ifx.Proxy.Contracts
- DontPanicLabs.Ifx.Services.Contracts

## Feedback

Submit issues at: https://github.com/dontpaniclabs/DontPanicLabs.Ifx/issues