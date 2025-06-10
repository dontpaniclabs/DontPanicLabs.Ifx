# Azure Application Insights Logger Implementation
This project provides an implementation of the `DontPanicLabs.Ifx.Telemetry.Logger.ILogger` interface using Azure Application Insights.  

## Configuration Sections

### ifx
#### Required config:
This package requires only one configuration item - the app insights connection string. 

#### Optional config:
Optionally, you can specify the telemetry channel to use. Microsoft recommends `ServerTelemetryChannel` by default in
all production environments. This package defaults to `InMemoryChannel` for historical reasons / to avoid changes
in existing code, but the recommended configuration is to use `ServerTelemetryChannel` in production. See
https://learn.microsoft.com/en-us/azure/azure-monitor/app/telemetry-channels for details.

Configuration File Example:
```json
{
  "ifx": {
    "telemetry": {
      "logging": {
        "applicationInsights": {
          "ConnectionString": "copy app insights connection string from azure portal",
          "TelemetryChannel": "optional; see above for notes on telemetry channel"
        }
      }
    }
  }
}
```

Environment Variable Example:
```
ifx__telemetry__logging__applicationInsights__ConnectionString="copy app insights connection string from azure portal"
// Optional: 
ifx__telemetry__logging__applicationInsights__TelemetryChannel="optional; see above for notes on telemetry channel"
```


## Configuration Exceptions
When creating a new instance of the `DontPanicLabs.Ifx.Telemetry.Logger.ApplicationInsights.Logger`, the constructor will attempt to read the Application Insights connection string from the configuration.

If the configuration key does not exist in the hosting environment, the underlying `DontPanicLabs.Ifx.Configuration.Local` package will throw a `NullConfigurationValueException`.

If the configuration key exists, but value is empty, the logger will throw an `EmptyConnectionStringException`.

For the optional `TelemetryChannel` configuration, if the value is not a valid telemetry channel type, the logger will throw an `InvalidTelemetryChannelException`.

## Logging Methods

### Log()
The `Log()` method will log a trace to Application Insights.  The developer can specify the message, log severity, and custom properties.  Custom properties can be used for advanced queries in Application Insights.  These messages can be found in the `traces` table in Application Insights.

### Event()
The `Event()` method will log a custom event to Application Insights.  The developer can specify the event name, custom properties, and custom metrics.  Custom properties can be used for advanced queries in Application Insights.  These events can be found in the `customEvents` table in Application Insights.

### Exception()
The `Exception()` method will log an exception to Application Insights.  The developer can specify the exception and custom properties. Custom properties can be used for advanced queries in Application Insights.  These exceptions can be found in the `exceptions` table in Application Insights, or under the Failures tab in the Application Insights portal.