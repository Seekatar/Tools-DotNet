# Test files

| Name                  | Description                                                             |
| --------------------- | ----------------------------------------------------------------------- |
| unit                  | Unit tests for the libraries. Run with `./run.ps1 test`                 |
| WebSettingsMinimalApi | A minimal API project that uses the SharedDevSettings library           |
| WebSettingsApi        | An ASP.NET Core Web API project that uses the SharedDevSettings library |

The WebSettingsMinimalApi project is a minimal API project that uses the SharedDevSettings library. It will use a project reference by default. If you set the `NUGET_VERSION` environment variable to a version number, it will use that version from NuGet instead. That is use for testing a PR build.

```pwsh
cd tests\WebSettingsMinimalApi
$env:NUGET_VERSION="0.2.3.75-prerelease"
dotnet run
```
