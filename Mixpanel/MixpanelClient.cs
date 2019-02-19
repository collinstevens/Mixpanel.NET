using CuttingEdge.Conditions;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mixpanel
{
    public class MixpanelClient : IMixpanelClient
    {
        public MixpanelClient(string token, HttpClient httpClient)
        {
            Condition.Requires(token, nameof(token)).IsNotNull().IsNotEmpty().IsNotNullOrWhiteSpace();
            Condition.Requires(httpClient, nameof(httpClient)).IsNotNull();

            Token = token;
            HttpClient = httpClient;
        }

        public Uri BaseUri { get; set; } = new Uri("https://api.mixpanel.com");

        public Uri TrackUri => new Uri(BaseUri, "track");

        public string Token { get; }

        private HttpClient HttpClient { get; }

        public MixpanelEvent CreateEvent() => new MixpanelEvent(Token);

        public async Task<TrackingOutcome> TrackAsync(MixpanelEvent @event) => await TrackAsync(@event, CancellationToken.None).ConfigureAwait(false);

        public async Task<TrackingOutcome> TrackAsync(MixpanelEvent @event, CancellationToken cancellationToken)
        {
            Condition.Requires(@event, nameof(@event)).IsNotNull();

            cancellationToken.ThrowIfCancellationRequested();

            var serializedEvent = JsonConvert.SerializeObject(@event);

            var eventBytes = Encoding.UTF8.GetBytes(serializedEvent);

            var base64 = Convert.ToBase64String(eventBytes);

            var relativeUri = $"?data={base64}&verbose=1";

            var uri = new Uri(TrackUri, relativeUri);

            var response = await HttpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var trackingResponse = JsonConvert.DeserializeObject<TrackingResponse>(content);

            return new TrackingOutcome(trackingResponse.Status.ToString(), trackingResponse.Error);
        }
    }
}
