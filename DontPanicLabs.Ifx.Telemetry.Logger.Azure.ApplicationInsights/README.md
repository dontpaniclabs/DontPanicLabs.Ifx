# Azure Application Insights Logger Implementation
This project provides an implementation of the `DontPanicLabs.Ifx.Telemetry.Logger.ILogger` interface using Azure Application Insights.  

## Configuration Sections

### ifx
This package requires only one configuration item:

Configuration File Example:
```json
{
  "ifx": {
    "telemetry": {
      "logging": {
        "applicationInsights": {
          "ConnectionString": "copy app insights connection string from azure portal"
        }
      }
    }
  }
}
```

Environment Variable Example:
```
ifx__telemetry__logging__applicationInsights__ConnectionString="copy app insights connection string from azure portal"
```


## Configuration Exceptions
When creating a new instance of the `DontPanicLabs.Ifx.Telemetry.Logger.ApplicationInsights.Logger`, the constructor will attempt to read the Application Insights connection string from the configuration.

If the configuration key does not exist in the hosting environment, the underlying `DontPanicLabs.Ifx.Configuration.Local` package will throw a `NullConfigurationValueException`.

If the configuration key exists, but value is empty, the logger will throw an `EmptyConnectionStringException`.

## Logging Methods

### Log()
The `Log()` method will log a trace to Application Insights.  The developer can specify the message, log severity, and custom properties.  Custom properties can be used for advanced queries in Application Insights.  These messages can be found in the `traces` table in Application Insights.

### Event()
The `Event()` method will log a custom event to Application Insights.  The developer can specify the event name, custom properties, and custom metrics.  Custom properties can be used for advanced queries in Application Insights.  These events can be found in the `customEvents` table in Application Insights.

### Exception()
The `Exception()` method will log an exception to Application Insights.  The developer can specify the exception and custom properties. Custom properties can be used for advanced queries in Application Insights.  These exceptions can be found in the `exceptions` table in Application Insights, or under the Failures tab in the Application Insights portal.
