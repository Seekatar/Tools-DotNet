using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using System.Net.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using WebSettings5Api;
using Seekatar.Tools;
using Microsoft.Extensions.Configuration;

namespace Seekatar.Tests;

public class SharedDevSettingsWeb5Test
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
        await RunTest(configFile: "ZZZ", expectedValue: "");
    }

    [Test]
    public async Task TestTurnedOff()
    {
        await RunTest(environment: "ZZZ", configFile: "", expectedValue: "");
    }

    async static Task RunTest(string environment = "Development", string configFile = "", string expectedValue = "DEV")
    {
        System.Environment.SetEnvironmentVariable("InEnvironment", "ENV");
        System.Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);
        System.Environment.SetEnvironmentVariable("NETCORE_ENVIRONMENT", environment);
        System.Environment.SetEnvironmentVariable("CONFIG_FILE", configFile);

        // .NET 5 way https://scotthannen.org/blog/2021/11/18/testserver-how-did-i-not-know.html
        var testServer = new TestServer(
                new WebHostBuilder()
                    .ConfigureAppConfiguration(configurationBuilder =>
                    {
                        configurationBuilder.InsertSharedDevSettings(reloadOnChange: false, configFile)
                                            .AddJsonFile("appsettings.json")
                                            .AddEnvironmentVariables();

                    })
                    .UseStartup<Startup>());
        var testClient = testServer.CreateClient();

        await GetEnvValue(testClient, "InAppSettings", "APP");
        await GetEnvValue(testClient, "InEnvironment", "ENV");
        await GetEnvValue(testClient, "InDevSettings", expectedValue);

        testClient?.Dispose();
        testServer?.Dispose();
    }
}

