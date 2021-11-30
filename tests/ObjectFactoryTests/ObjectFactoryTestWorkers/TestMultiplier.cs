using Seekatar.Tests;
using Seekatar.Tools;

namespace ObjectFactoryTestWorkers;

[Worker(Name = "times")]
[ObjectName(Name = "times")]
public class TestMultiplier : ITestWorker
{
    public int RunWorker(int a, int b) => a * b;
}
