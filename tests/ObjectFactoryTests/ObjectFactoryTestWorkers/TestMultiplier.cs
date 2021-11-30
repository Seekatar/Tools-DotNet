using Seekatar.Tests;
using Seekatar.Tools;

namespace ObjectFactoryTestWorkers;

// IF YOU MAKE CHANGES HERE
// be sure to re-pack the nuget package with a newer version. run.ps1 will do it for you
[Worker(Name = "times")]
[ObjectName(Name = "times")]
public class TestMultiplier : ITestWorker
{
    public int RunWorker(int a, int b) => a * b;
}
