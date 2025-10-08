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

This logger uses Serilog's native configuration format via `Serilog.Settings.Configuration`. Configure one or more logging sinks in your `appsettings.json` under the `ifx:telemetry:logging:serilog` section:

### Single Sink Example (SQL Server)

```json
{
  "ifx": {
    "telemetry": {
      "logging": {
        "serilog": {
          "Using": ["Serilog.Sinks.MSSqlServer"],
          "MinimumLevel": "Verbose",
          "WriteTo": [
            {
              "Name": "MSSqlServer",
              "Args": {
                "connectionString": "Server=localhost;Database=ApplicationLogs;Integrated Security=true;",
                "sinkOptionsSection": {
                  "tableName": "Logs",
                  "schemaName": "dbo",
                  "autoCreateSqlTable": true,
                  "batchPostingLimit": 50,
                  "batchPeriod": "0.00:00:05"
                }
              }
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
          "Using": ["Serilog.Sinks.MSSqlServer", "Serilog.Sinks.File", "Serilog.Sinks.Console"],
          "MinimumLevel": "Verbose",
          "WriteTo": [
            {
              "Name": "MSSqlServer",
              "Args": {
                "connectionString": "Server=localhost;Database=ApplicationLogs;Integrated Security=true;",
                "sinkOptionsSection": {
                  "tableName": "ApplicationLogs",
                  "schemaName": "dbo",
                  "autoCreateSqlTable": true,
                  "batchPostingLimit": 50,
                  "batchPeriod": "0.00:00:05"
                }
              }
            },
            {
              "Name": "File",
              "Args": {
                "path": "logs/app-.log",
                "rollingInterval": "Day",
                "retainedFileCountLimit": 7
              }
            },
            {
              "Name": "Console"
            }
          ]
        }
      }
    }
  }
}
```

### Configuration Format

The configuration follows Serilog's standard format:

- **Using**: Array of Serilog sink assemblies to load (e.g., `["Serilog.Sinks.MSSqlServer"]`)
- **MinimumLevel**: Minimum log level (Verbose, Debug, Information, Warning, Error, Fatal)
- **WriteTo**: Array of sink configurations, each with:
  - **Name**: The sink method name (e.g., "MSSqlServer", "File", "Console")
  - **Args**: Object containing sink-specific arguments

For detailed documentation on available sinks and their configuration options, see the [official Serilog documentation](https://github.com/serilog/serilog/wiki/Provided-Sinks)

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
3. **Configure batching**: Adjust `batchPostingLimit` and `batchPeriod` based on your log volume and performance requirements
4. **Structure your logs**: Use the properties dictionary to add contextual information that can be queried later
5. **Set appropriate minimum level**: Use `MinimumLevel` to filter logs at the source and reduce overhead

## Complete Configuration Example

```json
{
  "ifx": {
    "telemetry": {
      "logging": {
        "serilog": {
          "Using": ["Serilog.Sinks.MSSqlServer", "Serilog.Sinks.File"],
          "MinimumLevel": {
            "Default": "Information",
            "Override": {
              "Microsoft": "Warning",
              "System": "Warning"
            }
          },
          "WriteTo": [
            {
              "Name": "MSSqlServer",
              "Args": {
                "connectionString": "Server=logging-server;Database=ApplicationLogs;User Id=log_writer;Password=secure_password;",
                "sinkOptionsSection": {
                  "tableName": "ApplicationLogs",
                  "schemaName": "dbo",
                  "autoCreateSqlTable": true,
                  "batchPostingLimit": 100,
                  "batchPeriod": "0.00:00:10"
                }
              }
            },
            {
              "Name": "File",
              "Args": {
                "path": "logs/app-.log",
                "rollingInterval": "Day",
                "retainedFileCountLimit": 30
              }
            }
          ]
        }
      }
    }
  }
}
```

## Extending with Additional Sinks

This logger uses Serilog's native configuration system via `Serilog.Settings.Configuration`, which automatically supports **any Serilog sink** without requiring code changes. Simply install the sink package and configure it in your `appsettings.json`.

### Adding a New Sink (e.g., Elasticsearch)

**Step 1: Add the Serilog sink package**

```bash
dotnet add package Serilog.Sinks.Elasticsearch
```

**Step 2: Configure in appsettings.json**

```json
{
  "ifx": {
    "telemetry": {
      "logging": {
        "serilog": {
          "Using": ["Serilog.Sinks.Elasticsearch"],
          "MinimumLevel": "Verbose",
          "WriteTo": [
            {
              "Name": "Elasticsearch",
              "Args": {
                "nodeUris": "http://localhost:9200",
                "indexFormat": "logs-{0:yyyy.MM}",
                "autoRegisterTemplate": true
              }
            }
          ]
        }
      }
    }
  }
}
```

**That's it!** No code changes required.

### Other Popular Sinks

All official and community Serilog sinks work automatically:

- **Seq**: `Serilog.Sinks.Seq`
- **Elasticsearch**: `Serilog.Sinks.Elasticsearch`
- **Splunk**: `Serilog.Sinks.Splunk`
- **RabbitMQ**: `Serilog.Sinks.RabbitMQ`
- **Azure**: `Serilog.Sinks.AzureTableStorage`, `Serilog.Sinks.AzureBlobStorage`
- And [many more](https://github.com/serilog/serilog/wiki/Provided-Sinks)

### Architecture Benefits

- **Zero code maintenance**: New sinks work automatically without code changes
- **Full feature support**: All Serilog features and options available immediately
- **Standard configuration**: Uses Serilog's official configuration format
- **Community ecosystem**: Access to 100+ community-maintained sinks

## Dependencies

- Serilog (>= 4.2.0)
- Serilog.Settings.Configuration (>= 9.0.0)
- Serilog.Sinks.MSSqlServer (>= 8.0.0)
- Serilog.Sinks.File (>= 7.0.0)
- Serilog.Sinks.Console (>= 6.0.0)
- DontPanicLabs.Ifx.Telemetry.Logger.Contracts
- DontPanicLabs.Ifx.Configuration.Local
- DontPanicLabs.Ifx.Configuration.Contracts
