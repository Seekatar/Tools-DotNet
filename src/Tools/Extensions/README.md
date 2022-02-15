# Configuration Extension Methods

This helps solve the issue of developers needing environment variables or app settings that have secrets in them, but you don't want them in your repo, or copy have to copy around config files to multiple folders. There are different flavors to cover the different types of apps:

* Console (top-level or not)
* .NET 5 ASP.NET
* .NET 6 Minimal API

By default, it looks for a file named `shared.appsettings.Development.json` in the binaries' folder, then marches up the folder structure looking for one until it finds it. It's useful to put one of these in a higher-level folder so multiple projects can find it. For example, for client/server apps you can put the JSON file above both project folders so they can share the configuration.

Note by default it is installed as the _lowest priority_ configuration source so higher ones like `appsettings.json` and the environment will override them. If you want it highest, simply call `AddJsonFile(GetSharedSettingsPath())` as the last source.

## Set Environment Variables -- IMPORTANT

This is a development-only feature. It will only read the file if one of these environment variables is set to 'Development'. I usually set `ASPNETCORE_ENVIRONMENT` at the machine level so it is always set.

* ASPNETCORE_ENVIRONMENT="Development" (set by default for ASP.NET apps)
* NETCORE_ENVIRONMENT="Development"

## Using in a Console App

Usually in a console app, you control the creation of the configuration. This code puts it as the lowest priority by adding it first.

```csharp
        var configuration = new ConfigurationBuilder()
                       .AddSharedDevSettings()
                       .AddJsonFile("appsettings.json", true, true)
                       .AddEnvironmentVariables()
                       .Build();
```

## Using in ASP.NET App

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

## Using in ASP.NET Minimal API App

In `program.cs` simply add `InsertSharedDevSettings` to add it after creating the builder.

```csharp
using Seekatar.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.InsertSharedDevSettings();
```

## Parameters

Each of the extension methods takes the following parameters with default values (which are passed to the JSON source):

| Name           | Default                             | Description                                        |
| -------------- | ----------------------------------- | -------------------------------------------------- |
| fileName       | shared_appsettings.Development.json | Name of file to load                               |
| reloadOnChange | false                               | If true reloads the settings when the file changes |
