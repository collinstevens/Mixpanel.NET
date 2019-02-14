using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Mixpanel.IntegrationTests
{
    public class ClientIntegrationTests
    {
        private const string Token = "623a27f5c5d9f3ba331965c489b61484";

        [Fact]
        public async Task TrackAsync_CanSendSingleEvent()
        {
            var client = new Client(Token, new HttpClient());

            var @event = client.CreateEvent();

            @event.Name = nameof(TrackAsync_CanSendSingleEvent);

            var successful = await client.TrackAsync(@event).ConfigureAwait(false);

            Assert.True(successful);
        }
    }
}
