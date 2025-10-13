using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.IO;

namespace Seekatar.Tests;

public class SharedDevSettingsMinimalWebTest
{
    async static Task GetEnvValue(HttpClient client, string value, string expected)
    {
        var response = await client!.GetStringAsync($"/config/{value}");

        response.ShouldBe(expected);
    }

    async static Task RunTest(string? environment = "Development", string configFile = "", string expectedValue = "DEV", bool suppressWarnings = false, string stdOutput = "")
    {
        // override stdout to catch the warning
        var originalOut = System.Console.Out;
        using var consoleOutput = new StringWriter();
        System.Console.SetOut(consoleOutput);

        System.Environment.SetEnvironmentVariable("InEnvironment", "ENV");
        System.Environment.SetEnvironmentVariable("InEnvironment", "ENV");
        System.Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);
        System.Environment.SetEnvironmentVariable("NETCORE_ENVIRONMENT", environment);
        System.Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", environment);
        System.Environment.SetEnvironmentVariable("CONFIG_FILE", configFile);
        System.Environment.SetEnvironmentVariable("SUPPRESS_WARNINGS", suppressWarnings.ToString());

        var application = new WebApplicationFactory<Program>();
        var client = application.CreateClient();

        await GetEnvValue(client, "InAppSettings", "APP");
        await GetEnvValue(client, "InEnvironment", "ENV");
        await GetEnvValue(client, "InDevSettings", expectedValue);

        application?.Dispose();
        client?.Dispose();
        var output = consoleOutput.ToString();

        if (!string.IsNullOrWhiteSpace(stdOutput))
            output.ShouldStartWith(stdOutput);
        else
            output.ShouldBeEmpty();
        // restore stdout
        System.Console.SetOut(originalOut);
    }

    [Test]
    public async Task TestDefault()
    {
        await RunTest();
    }

    [Test]
    public async Task TestNoConfigFile()
    {
        await RunTest(configFile: "ZZZ4", expectedValue: "", stdOutput: "In development environment and InsertSharedDevSettings called, but couldn't find");
    }

    [Test]
    public async Task TestNoConfigFileSuppressWarning()
    {
        await RunTest(configFile: "ZZZ5", expectedValue: "", suppressWarnings: true, stdOutput: "");
    }

    [TestCase("ZZZZ")]
    [TestCase("Release")]
    [TestCase(null)]
    public async Task TestTurnedOff(string ? environment)
    {
        await RunTest(environment: "ZZZ6", configFile: "", expectedValue: "");
    }
}

