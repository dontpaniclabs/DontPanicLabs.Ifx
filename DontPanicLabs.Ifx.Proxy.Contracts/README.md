# DontPanicLabs.Ifx.Proxy.Contracts

The base contracts for using Don't Panic Labs Proxy packages on nuget.org.

The contracts and base classes in this package are used to create a concrete Proxy implementation.  This package alone does not provide a functioning proxy.

## Getting started

This package should be automatically added as a dependency of other Don't Panic Labs Proxy packages.

## Configuration Sections

### ifx
This package requires only one configuration item:

Configuration File Example:
```json
{
  "ifx": {
    "proxy": {
      "autoDiscoverServices": false,
      "serviceRegistrations": {
        "Fully.Qualified.Namespace.ITestSubsystem, Fully.Qualified.Namespace.AssemblyName": [
          "Fully.Qualified.Namespace.TestSubsystem, Fully.Qualified.Namespace.AssemblyName"
        ]
      }
    }
  }
}
```

Environment Variable Example:
```
ifx__proxy__autoDiscoverServices=false
```
It is recommended to JSON configuration files for this package due to the complexity of representing arrays and escaping keys in environment variables.

#### autoDiscoverServices
This setting is a boolean value that determines whether the proxy will automatically discover services in the current assembly. If set to `true`, the proxy will search the current assembly for classes that implement the `IService` interface and register them with the proxy. If set to `false`, the proxy will only register services that are explicitly defined in the `serviceRegistrations` section.

This setting only applies to proxy implementations that support service discovery.  Check the documentation for your specific proxy implementation to see if this setting is supported.

#### serviceRegistrations
This setting is a dictionary of service registrations. The key is the fully qualified name of the service interface, and the value is an array of fully qualified names of the service implementations.

The specific behavior of this setting is dependent on the proxy implementation.  Check the documentation for your specific proxy implementation to see how this setting is used.

## Configuration Exceptions
When creating a new instance of a class that inherits from `DPL.Ifx.Proxy.Contracts.ProxyFactoryBase`, the constructor will attempt to read the proxy section from configuration.

Any configuration keys that do not exist in the hosting environment will cause the underlying `DontPanicLabs.Ifx.Configuration.Local` package to throw a `NullConfigurationValueException`.

If key/value pairs are specified in the `serviceRegistrations` section, the proxy configuration will attempt to reflect the types specified.  If any of those types cannot be found a `DontPanicLabs.Ifx.Proxy.Contracts.Exceptions.DontPanicLabs.Ifx.Proxy.Contracts.Exceptions` will be thrown.

There may be additional configuration exceptions thrown by the proxy implementation.  Check the documentation for your specific proxy implementation to see if there are additional configuration requirements.


## Proxy Methods

### `ForSubsystem<I>()`

The `ForSubsystem<I>()` method attempts to retrieve an instance of the specified service interface `I` from the proxy.
 - If the interface has been registered with the proxy, the proxy will return an instance of the registered service implementation.
 - If the interface has not been registered with the proxy, the proxy will throw a `DontPanicLabs.Ifx.Proxy.Contracts.Exceptions.ProxyTypeLoadException`
 - If the interface does not 

### ForComponent`<I>`()


## Feedback

Submit issues at:

https://github.com/dontpaniclabs/DontPanicLabs.Ifx/issues