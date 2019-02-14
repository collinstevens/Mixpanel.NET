using CuttingEdge.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mixpanel
{
    public class Client : IClient
    {
        public Client(string token, HttpClient httpClient)
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

        public Event CreateEvent() => new Event(Token);

        public async Task<TrackingOutcome> TrackAsync(Event @event) => await TrackAsync(@event, false, CancellationToken.None).ConfigureAwait(false);

        public async Task<TrackingOutcome> TrackAsync(Event @event, CancellationToken cancellationToken) => await TrackAsync(@event, false, cancellationToken).ConfigureAwait(false);

        public async Task<TrackingOutcome> TrackAsync(Event @event, bool verbose) => await TrackAsync(@event, verbose, CancellationToken.None).ConfigureAwait(false);

        public async Task<TrackingOutcome> TrackAsync(Event @event, bool verbose, CancellationToken cancellationToken)
        {
            Condition.Requires(@event, nameof(@event)).IsNotNull();

            cancellationToken.ThrowIfCancellationRequested();

            var serializedEvent = JsonConvert.SerializeObject(@event);

            var eventBytes = Encoding.UTF8.GetBytes(serializedEvent);

            var base64 = Convert.ToBase64String(eventBytes);

            var verbosePart = verbose ? "&verbose=1" : string.Empty;

            var relativeUri = $"?data={base64}{verbosePart}";

            var uri = new Uri(TrackUri, relativeUri);

            var response = await HttpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            if (verbose)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var trackingResponse = JsonConvert.DeserializeObject<TrackingResponse>(content);

                return new TrackingOutcome(verbose, trackingResponse.Status.ToString(), trackingResponse.Error);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return new TrackingOutcome(verbose, content, null);
            }
        }
    }
}
