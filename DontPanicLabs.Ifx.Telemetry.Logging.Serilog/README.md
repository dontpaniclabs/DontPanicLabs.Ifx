# DontPanicLabs.Ifx.Telemetry.Logging.Serilog

A Serilog-based implementation of `DontPanicLabs.Ifx.Telemetry.Logger.Contracts.ILogger` with support for multiple logging destinations.

## Features

- **Multiple Sinks**: Configure one or more logging destinations (SQL Server, File, Console)
- **Extensible Architecture**: Easy to add custom sink types via polymorphic configuration
- **SQL Server Logging**: Uses Serilog.Sinks.MSSqlServer with auto-table creation and batched writes
- **File Logging**: Rolling file logs with configurable retention policies
- **Console Logging**: Output to console for development and debugging
- **Structured Logging**: Full support for properties and metrics through Serilog's context enrichment
- **ILogger Interface**: Implements the standard DPL Ifx ILogger interface (Log, Exception, Event, Flush)

## Installation

Add a reference to this project in your application:

```xml
<ItemGroup>
  <ProjectReference Include="..\DontPanicLabs.Ifx.Telemetry.Logging.Serilog\DontPanicLabs.Ifx.Telemetry.Logging.Serilog.csproj" />
</ItemGroup>
```

## Configuration

Configure one or more logging sinks in your `appsettings.json`:

### Single Sink Example (SQL Server)

```json
{
  "ifx": {
    "telemetry": {
      "logging": {
        "serilog": {
          "sinks": [
            {
              "type": "sql",
              "connectionString": "Server=localhost;Database=ApplicationLogs;Integrated Security=true;",
              "tableName": "Logs",
              "schemaName": "dbo",
              "autoCreateSqlTable": true,
              "batchPostingLimit": 50,
              "batchPeriodSeconds": 5
            }
          ]
        }
      }
    }
  }
}
```

### Multiple Sinks Example

Log to SQL Server, rolling files, and console simultaneously:

```json
{
  "ifx": {
    "telemetry": {
      "logging": {
        "serilog": {
          "sinks": [
            {
              "type": "sql",
              "connectionString": "Server=localhost;Database=Logs;Integrated Security=true;",
              "tableName": "ApplicationLogs"
            },
            {
              "type": "file",
              "filePath": "logs/app-.log",
              "rollingInterval": "Day",
              "retainedFileCountLimit": 7
            },
            {
              "type": "console"
            }
          ]
        }
      }
    }
  }
}
```

### Configuration Options by Sink Type

#### SQL Server Sink (`type: "sql"`)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `connectionString` | string | *required* | SQL Server connection string |
| `tableName` | string | "Logs" | Name of the table to store logs |
| `schemaName` | string | "dbo" | Database schema name |
| `autoCreateSqlTable` | bool | true | Whether to automatically create the log table if it doesn't exist |
| `batchPostingLimit` | int | 50 | Number of log events to batch before writing |
| `batchPeriodSeconds` | int | 5 | Time period (in seconds) to wait between batch writes |

#### File Sink (`type: "file"`)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `filePath` | string | "logs/log-.txt" | Path to log file (supports date tokens) |
| `rollingInterval` | string | "Day" | Rolling interval: Infinite, Year, Month, Day, Hour, Minute |
| `retainedFileCountLimit` | int | 7 | Number of log files to retain (null = unlimited) |

#### Console Sink (`type: "console"`)

No additional configuration required.

### Backward Compatibility

The legacy single-sink configuration format (without the `sinks` array) is still supported for SQL Server:

```json
{
  "ifx": {
    "telemetry": {
      "logging": {
        "serilog": {
          "connectionString": "Server=localhost;Database=Logs;Integrated Security=true;",
          "tableName": "Logs"
        }
      }
    }
  }
}
```

## Usage

### Basic Setup

```csharp
using DontPanicLabs.Ifx.Telemetry.Logger.Contracts;
using DontPanicLabs.Ifx.Telemetry.Logging.Serilog;

// Create the logger (automatically loads configuration from appsettings.json)
ILogger logger = new Logger();

// Log with severity levels
logger.Log("Application started", SeverityLevel.Information, null);
logger.Log("Debug information", SeverityLevel.Verbose, new Dictionary<string, string>
{
    { "UserId", "12345" },
    { "Component", "Startup" }
});

// Log exceptions
try
{
    // ... some code
}
catch (Exception ex)
{
    logger.Exception(ex, new Dictionary<string, string>
    {
        { "Operation", "ProcessOrder" },
        { "OrderId", "ABC123" }
    });
}

// Log events with metrics
logger.Event(
    "OrderProcessed",
    properties: new Dictionary<string, string> { { "OrderId", "ABC123" } },
    metrics: new Dictionary<string, double> { { "Duration", 1.23 }, { "ItemCount", 5 } },
    timeStamp: DateTimeOffset.UtcNow
);

// Flush logs before shutdown
logger.Flush();
```

## Database Schema

The logger uses Serilog's default SQL Server table schema:

| Column | Type | Description |
|--------|------|-------------|
| Id | INT IDENTITY | Auto-incrementing primary key |
| Message | NVARCHAR(MAX) | Rendered log message |
| MessageTemplate | NVARCHAR(MAX) | Template with placeholders |
| Level | NVARCHAR(128) | Log level (Verbose, Information, Warning, Error, Fatal) |
| TimeStamp | DATETIME2 | When the log occurred |
| Exception | NVARCHAR(MAX) | Full exception details |
| Properties | NVARCHAR(MAX) | JSON blob of structured data |

Properties, metrics, and custom fields are stored as JSON in the `Properties` column.

## Severity Level Mapping

The logger maps DPL Ifx `SeverityLevel` enum to Serilog log levels:

| DPL Ifx SeverityLevel | Serilog LogEventLevel |
|-----------------------|----------------------|
| Verbose | Verbose |
| Information | Information |
| Warning | Warning |
| Error | Error |
| Critical | Fatal |

## Best Practices

1. **Use a separate logging database**: Use a dedicated SQL database for logs with its own connection string and user
2. **Dispose properly**: Call `Flush()` before application shutdown to ensure all buffered logs are written
3. **Configure batching**: Adjust `batchSize` and `batchPeriodSeconds` based on your log volume and performance requirements
4. **Structure your logs**: Use the properties dictionary to add contextual information that can be queried later

## Example: appsettings.json

```json
{
  "ifx": {
    "telemetry": {
      "logging": {
        "serilog": {
          "connectionString": "Server=logging-server;Database=ApplicationLogs;User Id=log_writer;Password=secure_password;",
          "tableName": "ApplicationLogs",
          "schemaName": "dbo",
          "autoCreateTable": true,
          "batchSize": 100,
          "batchPeriodSeconds": 10
        }
      }
    }
  },
  "appSettings": {}
}
```

## Extending with Custom Sinks

The logger uses a polymorphic configuration pattern that makes it easy to add new sink types without modifying existing code.

### Step 1: Create a Configuration Class

Implement `ISerilogConfiguration` and define the `ConfigureSink` method:

```csharp
using Serilog;

namespace DontPanicLabs.Ifx.Telemetry.Logging.Serilog.Configuration;

public class CustomSinkConfiguration : ISerilogConfiguration
{
    public string? CustomProperty { get; init; }

    public void ConfigureSink(LoggerConfiguration loggerConfig)
    {
        // Configure your custom sink
        loggerConfig.WriteTo.YourCustomSink(CustomProperty);
    }
}
```

### Step 2: Register in ConfigurationExtensions

Update `ConfigurationExtensions.GetSerilogConfigurations()` to recognize your new sink type:

```csharp
ISerilogConfiguration? sinkConfig = sinkType switch
{
    "sql" => sinkSection.Get<SqlSinkConfiguration>(),
    "file" => sinkSection.Get<FileSinkConfiguration>(),
    "console" => sinkSection.Get<ConsoleSinkConfiguration>(),
    "custom" => sinkSection.Get<CustomSinkConfiguration>(),  // Add this line
    _ => null
};
```

### Step 3: Use in Configuration

```json
{
  "ifx": {
    "telemetry": {
      "logging": {
        "serilog": {
          "sinks": [
            {
              "type": "custom",
              "customProperty": "value"
            }
          ]
        }
      }
    }
  }
}
```

The polymorphic design means the `Logger` class never needs to change when adding new sink types!

## Dependencies

- Serilog (>= 4.2.0)
- Serilog.Sinks.MSSqlServer (>= 8.0.0)
- Serilog.Sinks.File (>= 7.0.0)
- Serilog.Sinks.Console (>= 6.0.0)
- DontPanicLabs.Ifx.Telemetry.Logger.Contracts
- DontPanicLabs.Ifx.Configuration.Local
- DontPanicLabs.Ifx.Configuration.Contracts
