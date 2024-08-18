using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Unit;

public class TestsServerTests
{
    private WebApplicationFactory<Program>? _application;
    private HttpClient? _client;

    [Test]
    public async Task TestHttpCall()
    {
    }

    [SetUp]
    public void Setup()
    {
        _application = new WebApplicationFactory<Program>();
        _client = _application.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _application?.Dispose();
        _client?.Dispose();
    }
}

