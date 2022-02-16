using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Seekatar.Interfaces;
using Seekatar.Tools;
using Shouldly;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Seekatar.Tests;

public class SharedDevSettingsConsoleNegativeTest
{
    [SetUp]
    public void Setup()
    {
    }

    static IConfiguration SetupConsole(string? filename = null, string? expectedName = "DEV", bool reloadOnChange = false)
    {
        Environment.SetEnvironmentVariable("InEnvironment", "ENV");

        // disable using shared settings
        Environment.SetEnvironmentVariable("NETCORE_ENVIRONMENT", "ZZZ");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "ZZZ");

        var configuration = new ConfigurationBuilder()
                       .AddSharedDevSettings(reloadOnChange, filename)
                       .AddJsonFile("appsettings.json", true, true)
                       .AddEnvironmentVariables()
                       .Build();
        configuration["InEnvironment"].ShouldBe("ENV");
        configuration["InAppSettings"].ShouldBe("APP");
        configuration["InDevSettings"].ShouldBe(expectedName);
        return configuration;

    }

    [Test]
    public void TestConsoleTurnedOff()
    {
        SetupConsole(null, null);
    }

    [TearDown]
    public void TearDown()
    {
    }
}
