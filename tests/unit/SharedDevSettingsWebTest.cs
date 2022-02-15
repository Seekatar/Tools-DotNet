using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Seekatar.Tests;

public class SharedDevSettingsWebTest
{
#if NET6_0_OR_GREATER
    private WebApplicationFactory<Program>? _application;
    private HttpClient? _client;
#elif NET5_0
    private TestServer _testServer;
    private HttpClient _testClient;
#else
#error "Must be .NET 5 or higher"
#endif

    async Task GetEnvValue(string value, string expected)
    {
        var response = await _client!.GetStringAsync($"/config/{value}");

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

#if NET6_0_OR_GREATER
        _application = new WebApplicationFactory<Program>();
        _client = _application.CreateClient();
#elif NET5_0


        // .NET 5 way https://scotthannen.org/blog/2021/11/18/testserver-how-did-i-not-know.html
            _testServer = new TestServer(
                new WebHostBuilder()
                    .ConfigureAppConfiguration(configurationBuilder =>
                    {
                        configurationBuilder.AddJsonFile("appsetting.json");
                    })
                    .
                    .UseStartup<Startup>());
            _testClient = _testServer.CreateClient();
#endif
    }

    [TearDown]
    public void TearDown()
    {
#if NET6_0_OR_GREATER
        _application?.Dispose();
        _client?.Dispose();
#elif NET5_0

        _testClient?.Dispose();
        _testServer?.Dispose();
#endif
    }
}

