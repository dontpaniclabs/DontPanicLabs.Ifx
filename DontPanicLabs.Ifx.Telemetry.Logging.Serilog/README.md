# DontPanicLabs.Ifx.Telemetry.Logging.Serilog

A Serilog-based implementation of `DontPanicLabs.Ifx.Telemetry.Logger.Contracts.ILogger` that writes logs to SQL Server.

## Features

- **SQL Server Logging**: Uses Serilog.Sinks.MSSqlServer to write logs to a SQL Server database
- **Auto-create Tables**: Optionally create log tables automatically using Serilog's default schema
- **Batched Writes**: Configurable batch size and period for efficient database writes
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

Configure the logger in your `appsettings.json`:

```json
{
  "ifx": {
    "telemetry": {
      "logging": {
        "serilog": {
          "connectionString": "Server=localhost;Database=ApplicationLogs;Integrated Security=true;",
          "tableName": "Logs",
          "schemaName": "dbo",
          "autoCreateTable": true,
          "batchSize": 50,
          "batchPeriodSeconds": 5
        }
      }
    }
  }
}
```

### Configuration Options

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `connectionString` | string | *required* | SQL Server connection string |
| `tableName` | string | "Logs" | Name of the table to store logs |
| `schemaName` | string | "dbo" | Database schema name |
| `autoCreateTable` | bool | true | Whether to automatically create the log table if it doesn't exist |
| `batchSize` | int | 50 | Number of log events to batch before writing |
| `batchPeriodSeconds` | int | 5 | Time period (in seconds) to wait between batch writes |

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

## Dependencies

- Serilog (>= 4.2.0)
- Serilog.Sinks.MSSqlServer (>= 8.0.0)
- DontPanicLabs.Ifx.Telemetry.Logger.Contracts
- DontPanicLabs.Ifx.Configuration.Local
- DontPanicLabs.Ifx.Configuration.Contracts
