using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Seekatar.Tools;
using Shouldly;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Seekatar.Tests;

public class SharedDevSettingsConsoleTest
{
    [SetUp]
    public void Setup()
    {
    }

    static IConfiguration SetupConsole(string? filename = null, string? expectedName = "DEV", bool reloadOnChange = false, bool suppressWarnings = false, string stdOutput = "")
    {
        // override stdout to catch the warning
        var originalOut = System.Console.Out;
        using var consoleOutput = new StringWriter();
        System.Console.SetOut(consoleOutput);

        System.Environment.SetEnvironmentVariable("InEnvironment", "ENV");
        Environment.SetEnvironmentVariable("InEnvironment", "ENV");
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var configuration = new ConfigurationBuilder()
                       .AddSharedDevSettings(reloadOnChange, filename, suppressWarnings: suppressWarnings)
                       .AddJsonFile("appsettings.json", true, true)
                       .AddEnvironmentVariables()
                       .Build();
        configuration["InEnvironment"].ShouldBe("ENV");
        configuration["InAppSettings"].ShouldBe("APP");
        configuration["InDevSettings"].ShouldBe(expectedName);

        var output = consoleOutput.ToString();

        if (!string.IsNullOrWhiteSpace(stdOutput))
            output.ShouldStartWith(stdOutput);
        else
            output.ShouldBeEmpty();
        // restore stdout
        System.Console.SetOut(originalOut);

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
    public void TestConsoleNameDoesntExist()
    {
        SetupConsole("notfound.json",  expectedName:null, stdOutput: "In development environment and AddSharedDevSettings called, but couldn't find");
    }

    [Test]
    public void TestConsoleNameDoesntExistSuppressWarnings()
    {
        SetupConsole("notfound.json", expectedName:null, suppressWarnings: true);
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
