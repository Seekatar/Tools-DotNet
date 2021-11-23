# Seekatar's .NET Tools

This repo contains various .NET tools for .NET 6

[![.NET](https://github.com/Seekatar/WebApi-DotNet-6/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Seekatar/WebApi-DotNet-6/actions/workflows/dotnet.yml)

[ObjectFactory](src/Tools/ObjectFactory/README.md) discovers derived `Types` and allows them to be created later.

## Pulling From GitHub NuGet Repo

Add a new nuget source

```PowerShell
dotnet nuget add source --username $username --password $pat --store-password-in-clear-text --name github "https://nuget.pkg.github.com/seekatar/index.json"
```
