using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Seekatar.Tools;
using Shouldly;
using System;

namespace Seekatar.Tests;

public class SharedDevSettingsConsoleNegativeTest
{
    [SetUp]
    public void Setup()
    {
    }

    static IConfiguration SetupConsole(string? filename = null, string? expectedName = "DEV", bool reloadOnChange = false, string? netCoreValue = null)
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

    [TestCase("ZZZZ")]
    [TestCase("Release")]
    [TestCase(null)]
    public void TestConsoleTurnedOff(string? environment)
    {
        SetupConsole(null, null, netCoreValue: environment);
    }

    [TearDown]
    public void TearDown()
    {
    }
}
