using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Seekatar.Interfaces;
using Seekatar.Tools;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tools_Test;

namespace Seekatar.Tests;
class ObjectAttributeFactory<T> : ObjectFactory<T>  where T : class
{
    public ObjectAttributeFactory(IServiceProvider provider, IOptions<ObjectFactoryOptions> options) : base(provider, options)
    { }

    protected override bool Predicate(Type type) => base.Predicate(type) && type.GetCustomAttributes(typeof(WorkerAttribute), false).Any();

    protected override string ObjectName(Type type) => (type.GetCustomAttributes(typeof(WorkerAttribute), false).FirstOrDefault() as WorkerAttribute)!.Name;
}


public class ObjectFactoryGenericTest
{
    private ServiceProvider? _provider;
    private IObjectFactory<ITestActivity>? _factory;

    [SetUp]
    public void Setup()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IObjectFactory<ITestActivity>, ObjectFactory<ITestActivity>>();

        serviceCollection.AddOptions<ObjectFactoryOptions>().Configure(options => {
            options.AssemblyNameMask = "O*";
            options.AssemblyNameRegEx = "(O.*|Tools-Test)";
        });

        serviceCollection.AddSingleton(typeof(ILogger<TestActivity>), new NullLogger<TestActivity>());

        _provider = serviceCollection.BuildServiceProvider();
        _provider.ShouldNotBeNull();

        _factory = _provider!.GetService<IObjectFactory<ITestActivity>>();
        _factory.ShouldNotBeNull();
    }

    [Test]
    public void TestLoadedCount()
    {
        foreach( var t in _factory!.LoadedTypes)
        {
            System.Diagnostics.Debug.WriteLine($">>> Loaded {t.Key} => {t.Value.Name}");
        }
        _factory!.LoadedTypes.Count.ShouldBe(1);
    }

    [Test]
    public async Task TestStringIntActivity()
    {
        var worker = _factory!.GetInstance("StringInt");
        worker.ShouldNotBeNull();
        await worker.Start("{\"Name\":\"test\",\"Num\":123}").ConfigureAwait(false);
        (worker as TestActivity)?.Result?.Result.ShouldNotBeNull();
    }

    [Test]
    public async Task TestStringIntActivityAsync()
    {
        var worker = _factory!.GetInstance("StringInt");
        worker.ShouldNotBeNull();
        await worker.Start("{\"Name\":\"test\",\"Num\":123,\"Async\":true}").ConfigureAwait(false);
        await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
        (worker as TestActivity)?.Result?.Result.ShouldNotBeNull();
    }
}
