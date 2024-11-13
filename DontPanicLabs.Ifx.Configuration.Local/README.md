# Local Configuration
This project supports three configuration sources:
 - Environment variables
 - The `appsettings.json` file
 - The .NET User Secrets store

## Environment Variables
The Config class first checks the `appsettings.json` file for the presence of the `skipEnvironmentVariables` entry. If this entry is found, and it is set to true, environment variables will not be loaded.
```json
{
    "skipEnvironmentVariables": true,
    "ifx": {},
    "appsettings": {}
}
```
This setting is primarily intended to ensure consistent environment configuration for testing purposes.

## appsettings.json
After loading environment variables (or not), the Config class loads the `appsettings.json` file.  `appsettings.json` is technically optional.  This file should never contain sensitive keys or secrets, as it is checked into source control.  Also, unintended values in this file could override environment variables in non-development environments.

## User Secrets
The recommended method for managing configuration in a development environment is to use User Secrets:  https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows.

User secrets are stored outside of the project directory, so they are not checked into source control.  The Config class will load user secrets after loading environment variables and the `appsettings.json` file.  

In order to load user secrets, you must specify the secrets file name in the `ifx` section of the config:
```json
{
  "appSettings": { },
  "ifx": { 
    "userSecretsId": "dpl.ifx.configuration.tests" 
  }
}
```
The user secrets file name is specified in the project file.
```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <UserSecretsId>dpl.ifx.configuration.tests</UserSecretsId>
</PropertyGroup>
```

## Configuration Sections

### ifx
The `ifx` section is intended for infrastructure items that do not directly relate to business operation of the application.  Items such as connection strings, service endpoints, secret keys, etc.

### appsettings
The `appsettings` section is intended for configuration items that affect the operation of the business/domain logic of the application.  Items such as feature flags, business rules, etc.

## Configuration Exceptions
When the config initializes, it requires the presence of the `ifx` and `appsettings` sections.  If either of these sections are missing, the config will throw an exception.

Due to how the .NET configuration libraries work, the following will not result in a valid config file:
```json
{
    "ifx": { },
    "appsettings": { }
}
```
The `ifx` and `appsettings` sections must contain at least one key-value pair.  This will prevent the config from throwing an exception if no configuration has been specified for the application:
```json
{
    "ifx": { 
        "_": "" 
    },
    "appsettings": { 
        "_": "" 
    }
}
```

## Development Setup
In a development setting, the following is the recommended approach to managing configuration:

`appsettings.json`
```json
{
    "ifx": {
        "userSecretsId": "location.specified.in.project.file"
    }
}
```
`location.specified.in.project.file/secrets.json`
```json
{
    "ifx": {
        "Secret": "secret_value",
    },
    "appSettings": {
        "Application_Setting": "value",
        "Add more setting": "add some more"
    }
}
```