# DontPanicLabs.Ifx.Telemetry.Logger.Serilog

A Serilog-based implementation of `DontPanicLabs.Ifx.Telemetry.Logger.Contracts.ILogger` with support for multiple
logging destinations.

## Features

- **Multiple Sinks**: Configure one or more logging destinations (SQL Server, File, Console, and 100+ community sinks)
- **Minimal Dependencies**: Core package includes only Serilog runtime; consumers add references to desired sinks

## Installation

Install the core logging package:

```bash
dotnet add package DontPanicLabs.Ifx.Telemetry.Logger.Serilog
```

**IMPORTANT**: You must also install the Serilog sink packages you want to use. The core package does not include any
sinks.

For example, to use SQL Server, File, and Console sinks:

```bash
dotnet add package Serilog.Sinks.MSSqlServer
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Console
```

## Configuration

This logger uses Serilog's [native configuration format](https://github.com/serilog/serilog-settings-configuration).
Configure one or more [logging sinks](https://github.com/serilog/serilog/wiki/Provided-Sinks) by adding standard
serilog settings for the sink in your app settings under `ifx:telemetry:logging:serilog`.

### Single Sink Example (SQL Server)

This sample configuration configures logging to SQL server. The `columnOptionsSection` shown here configures writing
the `metrics` and `properties` `IDictionary<string, object>` parameters passed to the logging methods to be written
as JSON to the `LogEvent` column in the SQL table. If `columnOptionsSection` is not specified, the default Serilog
behavior writes this data as XML in the `Properties` column.

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
                "connectionString": "{YOUR SQL CONNECTION STRING HERE}",
                "sinkOptionsSection": {
                  "tableName": "Logs",
                  "schemaName": "dbo",
                  "autoCreateSqlTable": true,
                  "batchPostingLimit": 50,
                  "batchPeriod": "0.00:00:05"
                },
                "columnOptionsSection": {
                  "addStandardColumns": ["LogEvent"],
                  "removeStandardColumns": ["Properties"],
                  "logEvent": {
                    "excludeStandardColumns": true
                  }
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

Log to a rolling file log and the console, both using JSON formatting to display structured log data.

```json
{
  "ifx": {
    "telemetry": {
      "logging": {
        "serilog": {
          "Using": ["Serilog.Sinks.File", "Serilog.Sinks.Console"],
          "MinimumLevel": "Verbose",
          "WriteTo": [
            {
              "Name": "File",
              "Args": {
                "formatter": "Serilog.Formatting.Json.JsonFormatter, Serilog",
                "path": "logs/app-.log",
                "rollingInterval": "Day",
                "retainedFileCountLimit": 7
              }
            },
            {
              "Name": "Console",
              "Args": {
                "formatter": "Serilog.Formatting.Json.JsonFormatter, Serilog"
              }
            }
          ]
        }
      }
    }
  }
}
```

## Usage

### Basic Setup

```csharp

// Create the logger (automatically loads configuration from appsettings.json); namespaces shown in-line for clarity.
// Consuming code should instantiate the logger but use the `ILogger` interface to interact.
DontPanicLabs.Ifx.Telemetry.Logger.Contracts.ILogger logger = new DontPanicLabs.Ifx.Telemetry.Logger.Serilog.Logger();

// Use the logger
logger.Log("Application started", SeverityLevel.Information, null);
```
