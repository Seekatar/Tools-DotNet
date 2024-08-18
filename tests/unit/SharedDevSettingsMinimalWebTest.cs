using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Seekatar.Tests;

public class SharedDevSettingsMinimalWebTest
{
    async static Task GetEnvValue(HttpClient client, string value, string expected)
    {
        var response = await client!.GetStringAsync($"/config/{value}");

        response.ShouldBe(expected);
    }

    async static Task RunTest(string? environment = "Development", string configFile = "", string expectedValue = "DEV")
    {
        System.Environment.SetEnvironmentVariable("InEnvironment", "ENV");
        System.Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);
        System.Environment.SetEnvironmentVariable("NETCORE_ENVIRONMENT", environment);
        System.Environment.SetEnvironmentVariable("CONFIG_FILE", configFile);

        var application = new WebApplicationFactory<Program>();
        var client = application.CreateClient();

        await GetEnvValue(client, "InAppSettings", "APP");
        await GetEnvValue(client, "InEnvironment", "ENV");
        await GetEnvValue(client, "InDevSettings", expectedValue);

        application?.Dispose();
        client?.Dispose();
    }

    [Test]
    public async Task TestDefault()
    {
        await RunTest();
    }

    [Test]
    public async Task TestNoConfigFile()
    {
        await RunTest(configFile:"ZZZ", expectedValue: "");
    }

    [TestCase("ZZZZ")]
    [TestCase("Release")]
    [TestCase(null)]
    public async Task TestTurnedOff(string ? environment)
    {
        await RunTest(environment: "ZZZ", configFile: "", expectedValue: "");
    }
}

