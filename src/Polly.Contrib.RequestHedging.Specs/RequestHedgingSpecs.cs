using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly.Contrib.Hedging;
using Xunit;

namespace Polly.Contrib.Hedging.Specs
{
    public class MyContribSpecs
    {
        private readonly Func<Context, Task> onHedgingAsync;
        private readonly TimeSpan defaultHedgingDelay = TimeSpan.FromSeconds(1);
        IEnumerable<Func<Context, CancellationToken, Task<HttpResponseMessage>>> hedgedTaskFunctions;

        [Fact]
        public void PolicyIsAssignableToImplementedInterfaces()
        {
            AsyncHedgingPolicy<HttpResponseMessage> policy = Policy.Handle<Exception>()
                .OrResult<HttpResponseMessage>(_ => false)
                .HedgeAsync(
                    hedgedTaskFunctions,
                    defaultHedgingDelay,
                    onHedgingAsync);

            //policy.Should().BeAssignableTo<IAsyncPolicy>();
            policy.Should().BeAssignableTo<IHedgingPolicy>();
        }

        [Fact]
        public async Task PolicyExecutesThePassedDelegate()
        {
            bool executed = false;
            AsyncHedgingPolicy<HttpResponseMessage> policy = Policy.Handle<Exception>()
                .OrResult<HttpResponseMessage>(_ => false)
                .HedgeAsync(
                    hedgedTaskFunctions,
                    defaultHedgingDelay,
                    onHedgingAsync);

            await policy.ExecuteAsync(() =>
            {
                executed = true;
                return Task.FromResult(new HttpResponseMessage());
            });

            executed.Should().BeTrue();
        }
    }
}
