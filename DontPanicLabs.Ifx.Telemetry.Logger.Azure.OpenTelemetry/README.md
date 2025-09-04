# Azure Open Telemetry Logger Implementation

This project provides an implementation of the `DontPanicLabs.Ifx.Telemetry.Logger.ILogger` interface using Azure Open Telemetry.

## Configuration Sections

### ifx

#### Required config:

This package requires only one configuration item - the open telemetry connection string.

Configuration File Example:

```json
{
  "ifx": {
    "openTelemetry": {
      "connectionString": "copy open telemetry connection string from azure portal"
    }
  }
}
```

Environment Variable Example:

```
ifx__openTelemetry__connectionString="copy open telemetry connection string from azure portal"
```

## Configuration Exceptions

When creating a new instance of the `DontPanicLabs.Ifx.Telemetry.Logger.OpenTelemetry.Logger`, the constructor will attempt to read the Open Telemetry connection string from the
configuration.

If the configuration key does not exist in the hosting environment, the underlying `DontPanicLabs.Ifx.Configuration.Local` package will throw a `NullConfigurationValueException`.

If the configuration key exists, but value is empty, the logger will throw an `EmptyConnectionStringException`.

## Custom Property Logging

Open Telemetry does not support custom properties to be logged for `Log()` and `Exception()` methods. If the developer attempts to pass custom properties to these methods, a
`PlatformNotSupportedException` will be thrown. If you need to log custom properties, use the `Event()` method.

## Logging Methods

### Log()

The `Log()` method will log a trace to Azure Monitor. The developer can specify the message and log severity. Custom properties are not supported. These messages can be found in the `traces` table in Azure Monitor.

### Event()

The `Event()` method will log a custom event to Azure Monitor. The developer can specify the event name, custom properties, and custom metrics. Custom properties can be used for
advanced queries in Azure Monitor. These events can be found in the `customEvents` table in Azure Monitor.

### Exception()

The `Exception()` method will log an exception to Azure Monitor. The developer can specify the exception. Custom properties are not supported in Open Telemetry. These exceptions
can be found in the `exceptions` table in Azure Monitor, or under the Failures tab in the Azure Monitor portal.