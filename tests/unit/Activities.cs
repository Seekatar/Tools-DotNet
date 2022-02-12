using Microsoft.Extensions.Logging;
using Seekatar.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools_Test
{
    record TestInput(string Name, int Num, bool Async);

    record TestOutput(string Result);

    [Worker(Name = "StringInt")]
    internal class TestActivity : TestActivityBase<TestInput, TestOutput>
    {
        public TestActivity(ILogger<TestActivity> logger) : base(logger)
        {
        }

        public override async Task Start(TestInput? input)
        {
            if (input?.Async ?? false)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                    await Complete(new TestOutput($"Got input of {input?.Name ?? ""} and {input?.Num ?? 0}")).ConfigureAwait(false);
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            else
            {
                await Complete(new TestOutput($"Got input of {input?.Name ?? ""} and {input?.Num ?? 0}")).ConfigureAwait(false);
            }
        }
    }
}
