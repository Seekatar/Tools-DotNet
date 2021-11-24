using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Seekatar.Tests
{
    public interface ITestActivity
    {
        Task Start(string input);

        Task Complete(object? output);
    }

    public abstract class TestActivityBase<TInput, TOutput> : ITestActivity
    {
        private readonly ILogger _logger;

        protected TestActivityBase(ILogger logger)
        {
            _logger = logger;
        }

        public TOutput? Result { get; private set; }

        public async Task Start(string input)
        {
            var inputObj = JsonSerializer.Deserialize<TInput>(input, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            await Start(inputObj).ConfigureAwait(false);
        }

        public abstract Task Start(TInput? input);

        public Task Complete(TOutput? output)
        {
            Result = output;
            return Complete((object?)output);
        }

        public Task Complete(object? output)
        {
            _logger.LogInformation("Complete activity {name}", this.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
