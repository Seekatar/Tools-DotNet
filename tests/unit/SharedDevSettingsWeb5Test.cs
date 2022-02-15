using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using WebSettings5Api;
using Seekatar.Tools;
using Microsoft.Extensions.Configuration;

namespace Seekatar.Tests;

public class SharedDevSettingsWeb5Test
{
    private TestServer? _testServer;
    private HttpClient? _testClient;

    async Task GetEnvValue(string value, string expected)
    {
        var response = await _testClient!.GetStringAsync($"/config/{value}");

        response.ShouldBe(expected);
    }

    [Test]
    public async Task TestDefault()
    {
        await GetEnvValue("InAppSettings", "APP");
        await GetEnvValue("InEnvironment", "ENV");
        await GetEnvValue("InDevSettings", "DEV");
    }

    [SetUp]
    public void Setup()
    {
        System.Environment.SetEnvironmentVariable("InEnvironment", "ENV");
        System.Environment.SetEnvironmentVariable("NETCORE_ENVIRONMENT", "Development");

        // .NET 5 way https://scotthannen.org/blog/2021/11/18/testserver-how-did-i-not-know.html
            _testServer = new TestServer(
                new WebHostBuilder()
                    .ConfigureAppConfiguration(configurationBuilder =>
                    {
                        configurationBuilder.InsertSharedDevSettings()
                                            .AddJsonFile("appsettings.json")
                                            .AddEnvironmentVariables();

                    })
                    .UseStartup<Startup>());
            _testClient = _testServer.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _testClient?.Dispose();
        _testServer?.Dispose();
    }
}

