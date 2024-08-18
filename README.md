# Seekatar's .NET Tools

This repo contains various .NET tools for .NET 6+

[![.NET](https://github.com/Seekatar/Tools-DotNet/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Seekatar/Tools-DotNet/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/Seekatar/Tools-DotNet/branch/main/graph/badge.svg?token=X3J5MU9T3C)](https://codecov.io/gh/Seekatar/Tools-DotNet)

## [ObjectFactory](src/Tools/ObjectFactory/README.md)

ObjectFactory discovers derived `Types` and allows them to be created later.

## [Configuration Extension For Devs](src/Tools/Extensions/README.md)

This set of configuration extension methods adds a low-priority JSON file to the `IConfiguration` sources for sharing configuration across projects.

## Install

Installing from the NuGet.org

```powershell
dotnet add package Seekatar.Tools
```

## Pulling From GitHub NuGet Repo

Originally was in a GitHub NuGet repo. To add a new NuGet source

```powershell
dotnet nuget add source --username $username --password $pat --store-password-in-clear-text --name github "https://nuget.pkg.github.com/seekatar/index.json"
```
