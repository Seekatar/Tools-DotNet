using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using System.Net.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using WebSettingsApi;
using Seekatar.Tools;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Seekatar.Tests;

public class SharedDevSettingsWebTest
{
    async static Task GetEnvValue(HttpClient client, string value, string expected)
    {
        var response = await client!.GetStringAsync($"/config/{value}");

        response.ShouldBe(expected);
    }

    [Test]
    public async Task TestDefault()
    {
        await RunTest();
    }

    [Test]
    public async Task TestNoConfigFile()
    {
        await RunTest(configFile: "ZZZ1", expectedValue: "", stdOutput: "In development environment and InsertSharedDevSettings called, but couldn't find");
    }

    [Test]
    public async Task TestNoConfigFileSuppressWarning()
    {
        await RunTest(configFile: "ZZZ2", expectedValue: "", suppressWarnings: true, stdOutput: "");
    }

    [Test]
    public async Task TestTurnedOff()
    {
        await RunTest(environment: "ZZZ3", configFile: "", expectedValue: "");
    }

    async static Task RunTest(string environment = "Development", string configFile = "", string expectedValue = "DEV", bool suppressWarnings = false, string stdOutput = "")
    {
        // override stdout to catch the warning
        var originalOut = System.Console.Out;
        using var consoleOutput = new StringWriter();
        System.Console.SetOut(consoleOutput);

        System.Environment.SetEnvironmentVariable("InEnvironment", "ENV");
        System.Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);
        System.Environment.SetEnvironmentVariable("NETCORE_ENVIRONMENT", environment);
        System.Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", environment);
        System.Environment.SetEnvironmentVariable("CONFIG_FILE", configFile);

        // .NET 5 way https://scotthannen.org/blog/2021/11/18/testserver-how-did-i-not-know.html
        var testServer = new TestServer(
                new WebHostBuilder()
                    .ConfigureAppConfiguration(configurationBuilder =>
                    {
                        configurationBuilder.InsertSharedDevSettings(reloadOnChange: false, configFile, suppressWarnings)
                                            .AddJsonFile("appsettings.json")
                                            .AddEnvironmentVariables();

                    })
                    .UseStartup<Startup>());
        var testClient = testServer.CreateClient();

        await GetEnvValue(testClient, "InAppSettings", "APP");
        await GetEnvValue(testClient, "InEnvironment", "ENV");
        await GetEnvValue(testClient, "InDevSettings", expectedValue);

        var output = consoleOutput.ToString();

        if (!string.IsNullOrWhiteSpace(stdOutput))
            output.ShouldStartWith(stdOutput);
        else
            output.ShouldBeEmpty();
        // restore stdout
        System.Console.SetOut(originalOut);

        testClient?.Dispose();
        testServer?.Dispose();
    }
}

