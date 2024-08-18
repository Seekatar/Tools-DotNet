# Seekatar's .NET Tools

This repo contains various .NET tools for .NET 6+

[![.NET](https://github.com/Seekatar/Tools-DotNet/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Seekatar/Tools-DotNet/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/Seekatar/Tools-DotNet/branch/main/graph/badge.svg?token=X3J5MU9T3C)](https://codecov.io/gh/Seekatar/Tools-DotNet)

## Install

Installing from the NuGet.org

```powershell
dotnet add package Seekatar.Tools
```

## Configuration Extension For Devs

This set of configuration extension methods adds a low-priority JSON settings file to the `IConfiguration` sources for sharing configuration across projects. It easier way to share configuration and secrets as opposed `launchSettings.json` or Visual Studio's `User Secrets`. By looking up the folder structure, it allows you to share configuration across projects, and avoid committing them to a repo. There are different extension methods to cover the different types of apps:

* Console (top-level or not)
* .NET 8 ASP.NET with Controllers
* .NET 8 Minimal API

By default, it looks for a file named `shared.appsettings.Development.json` in the binaries' folder, then marches up the folder structure looking for one until it finds it. It's useful to put one of these in a higher-level folder so multiple projects can find it. For example, for client/server apps you can put the JSON file above both project folders so they can share the configuration.

Note by default it is installed as the _lowest priority_ configuration source so higher ones like `appsettings.json` and the environment will override them. If you want it highest, simply call `AddJsonFile(GetSharedSettingsPath())` as the last source.

### Set Environment Variables -- IMPORTANT

This is a development-only feature. It will only read the file if one of these environment variables is set to 'Development'. I usually set `ASPNETCORE_ENVIRONMENT` at the machine level so it is always set.

* ASPNETCORE_ENVIRONMENT="Development" (set by default for ASP.NET apps)
* DOTNET_ENVIRONMENT="Development"
* NETCORE_ENVIRONMENT="Development"

### Using in a Console App

Usually in a console app, you control the creation of the configuration. This code puts it as the lowest priority by adding it first.

```csharp
        var configuration = new ConfigurationBuilder()
                       .AddSharedDevSettings()
                       .AddJsonFile("appsettings.json", true, true)
                       .AddEnvironmentVariables()
                       .Build();
```

### Using in ASP.NET App

In `main.cs` simply add `InsertSharedDevSettings` to add it as the lowest priority.

```csharp
using Seekatar.Tools;
...

        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .InsertSharedDevSettings()
                .Build()
                .Run();
        }
```

### Using in ASP.NET Minimal API App

In `program.cs` simply add `InsertSharedDevSettings` to add it after creating the builder.

```csharp
using Seekatar.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.InsertSharedDevSettings();
```

### Parameters

Each of the extension methods takes the following parameters with default values (which are passed to the JSON source):

| Name           | Default                             | Description                                        |
| -------------- | ----------------------------------- | -------------------------------------------------- |
| fileName       | shared.appsettings.Development.json | Name of file to load                               |
| reloadOnChange | false                               | If true reloads the settings when the file changes |

## [ObjectFactory](src/Tools/ObjectFactory/README.md)

ObjectFactory discovers derived `Types` and allows them to be created later.
