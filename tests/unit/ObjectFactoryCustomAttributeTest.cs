using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Seekatar.Interfaces;
using Seekatar.Tools;
using Shouldly;
using System;
using System.Linq;

namespace Seekatar.Tests;

class WorkerAttributeFactory : ObjectFactory<ITestWorker>
{
    public WorkerAttributeFactory(IServiceProvider provider, IOptions<ObjectFactoryOptions> options) : base(provider, options)
    { }

    protected override bool Predicate(Type type) => base.Predicate(type) && type.GetCustomAttributes(typeof(WorkerAttribute), false).Any();

    protected override string ObjectName(Type type) => (type.GetCustomAttributes(typeof(WorkerAttribute), false).FirstOrDefault() as WorkerAttribute)!.Name;
}

public class ObjectFactoryCustomAttributeTest
{
    private ServiceProvider? _provider;
    private IObjectFactory<ITestWorker>? _factory;

    [SetUp]
    public void Setup()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IObjectFactory<ITestWorker>, WorkerAttributeFactory>();
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
    public void TestAdd()
    {
        var worker = _factory!.GetInstance("add");
        worker.ShouldNotBeNull();
        worker.RunWorker(1, 4).ShouldBe(5);
    }
    [Test]
    public void TestSubtract()
    {
        var worker = _factory!.GetInstance(typeof(TestSubtracter).Name);
        worker.ShouldBeNull();
    }
    [Test]
    public void TestMultiplierFromNuGet()
    {
        // TestMultiplier not reference here to avoid it gettting loaded automatically
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
