﻿using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Seekatar.Tests;

public class SharedDevSettingsWebTest
{
    async static Task GetEnvValue(HttpClient client, string value, string expected)
    {
        var response = await client!.GetStringAsync($"/config/{value}");

        response.ShouldBe(expected);
    }

    async static Task RunTest(string environment = "Development", string configFile = "", string expectedValue = "DEV")
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

    [Test]
    public async Task TestTurnedOff()
    {
        await RunTest(environment: "ZZZ", configFile: "", expectedValue: "");
    }
}

