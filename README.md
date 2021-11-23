# Seekatar's .NET Tools

This repo contains various .NET tools for .NET 6

## [ObjectFactory](src/Tools/ObjectFactory/README.md)
ObjectFactory discovers derived `Types` and allows them to be created later.

[![.NET](https://github.com/Seekatar/Tools-DotNet/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Seekatar/Tools-DotNet/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/Seekatar/Tools-DotNet/branch/main/graph/badge.svg?token=X3J5MU9T3C)](https://codecov.io/gh/Seekatar/Tools-DotNet)

## Pulling From GitHub NuGet Repo

Add a new nuget source

```PowerShell
dotnet nuget add source --username $username --password $pat --store-password-in-clear-text --name github "https://nuget.pkg.github.com/seekatar/index.json"
```
