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

            var mixpanelEvent = client.CreateEvent();
            
            mixpanelEvent.Name = nameof(TrackAsync_CanSendSingleEvent);

            var outcome = await client.TrackAsync(mixpanelEvent, true).ConfigureAwait(false);

            Assert.True(outcome.Successful);
        }

        [Fact]
        public async Task TrackAsync_CanSendSingleEvent_WithDateTimeProperty()
        {
            var client = new Client(Token, new HttpClient());

            var mixpanelEvent = client.CreateEvent();

            mixpanelEvent.Name = nameof(TrackAsync_CanSendSingleEvent_WithDateTimeProperty);
            mixpanelEvent["DateTimeProperty"] = DateTime.Now;

            var outcome = await client.TrackAsync(mixpanelEvent, true).ConfigureAwait(false);

            Assert.True(outcome.Successful);
        }
    }
}
