# DontPanicLabs.Ifx.Services.Contracts

Base contracts for building SOA services in .NET.

## Getting started

Add the package and decorate your service contracts with either ISubsystem or IComponent based upon their role in your architecture.

## Usage

```csharp
[ServiceContract]
public interface IMyService : ISubsystem  
{
    [OperationContract]
    void MyOperation();
}

[ServiceContract]
public interface IMyOtherService : IComponent  
{
    [OperationContract]
    void MyOperation();
}
```

## Feedback

Submit issues at:

https://github.com/dontpaniclabs/DontPanicLabs.Ifx/issues