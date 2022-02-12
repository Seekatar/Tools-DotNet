using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Seekatar.Interfaces;
using Seekatar.Tools;
using Shouldly;
using System;
using System.Linq;

namespace Seekatar.Tests;

public class ObjectFactoryAttributeTest
{
    private ServiceProvider? _provider;
    private IObjectFactory<ITestWorker>? _factory;

    [SetUp]
    public void Setup()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IObjectFactory<ITestWorker>, ObjectFactoryUsingNameAttribute<ITestWorker>>();
        serviceCollection.AddSingleton<ITestWorker, TestSummer>();

        serviceCollection.AddOptions<ObjectFactoryOptions>().Configure(options =>
        {
            options.AssemblyNameMask = "ObjectFactory*";
            options.AssemblyNameRegEx = "(ObjectFactory.*|Tools-Test)";
        });

        _provider = serviceCollection.BuildServiceProvider();
        _provider.ShouldNotBeNull();

        _factory = _provider!.GetService<IObjectFactory<ITestWorker>>();
        _factory.ShouldNotBeNull();
    }

    [Test]
    public void TestSubtract()
    {
        var worker = _factory!.GetInstance("subtract");
        worker.ShouldNotBeNull();
        worker.RunWorker(5, 4).ShouldBe(1);
    }
    [Test]
    public void TestSubtractByType()
    {
        var worker = _factory!.GetInstance(typeof(TestSubtracter));
        worker.ShouldNotBeNull();
        worker.RunWorker(5, 4).ShouldBe(1);
    }
    [Test]
    public void TestAdd()
    {
        var worker = _factory!.GetInstance("add");
        worker.ShouldBeNull();
    }
    [Test]
    public void TestMultiplierFromNuGet()
    {
        // TestMultiplier not reference here to avoid it getting loaded automatically
        // that way this tests the ObjectFactory.LoadAssemblies method
        var worker = _factory!.GetInstance("times");
        worker.ShouldNotBeNull();
        worker.RunWorker(5, 6).ShouldBe(30);
    }
    [Test]
    public void TestLoadedCount()
    {
        _factory!.LoadedTypes.Count.ShouldBe(2);
    }
}
