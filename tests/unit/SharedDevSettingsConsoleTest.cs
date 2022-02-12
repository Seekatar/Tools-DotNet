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

public class SharedDevSettingsConsoleTest
{
    [SetUp]
    public void Setup()
    {
    }

    static IConfiguration SetupConsole(string? filename = null, string expectedName = "DEV", bool optional = true, bool reloadOnChange = false)
    {
        Environment.SetEnvironmentVariable("InEnvironment", "ENV");

        var configuration = new ConfigurationBuilder()
                       .AddSharedDevSettings(optional, reloadOnChange, filename)
                       .AddJsonFile("appsettings.json", true, true)
                       .AddEnvironmentVariables()
                       .Build();
        configuration["InEnvironment"].ShouldBe("ENV");
        configuration["InAppSettings"].ShouldBe("APP");
        configuration["InDevSettings"].ShouldBe(expectedName);
        return configuration;

    }

    [Test]
    public void TestConsoleDefault()
    {
        SetupConsole();
    }

    [Test]
    public void TestConsoleNameChange()
    {
        SetupConsole("differentName.appsettings.Development.json", "DIFFERENT");
    }

    [Test]
    public async Task TestConsoleReload()
    {
        var fileName = HostBuilderExtensions.GetSharedSettingsPath();
        var newName = $"{fileName}.junk";
        File.Copy(fileName, newName, true);

        var configuration = SetupConsole(Path.GetFileName(newName), reloadOnChange: true);

        File.WriteAllText(newName, "{\"InDevSettings\":\"Updated\"}");
        await Task.Delay(1000); // let background update the file
        configuration["InDevSettings"].ShouldBe("Updated");
        File.Delete(newName);
    }

    [TearDown]
    public void TearDown()
    {
    }
}
