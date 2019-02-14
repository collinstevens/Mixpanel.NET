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

        public async Task<bool> TrackAsync(Event @event) => await TrackAsync(@event, CancellationToken.None);

        public async Task<bool> TrackAsync(Event @event, CancellationToken cancellationToken)
        {
            Condition.Requires(@event, nameof(@event)).IsNotNull();

            cancellationToken.ThrowIfCancellationRequested();

            var serializedEvent = JsonConvert.SerializeObject(@event, Formatting.Indented);

            var eventBytes = Encoding.UTF8.GetBytes(serializedEvent);

            var base64 = Convert.ToBase64String(eventBytes);

            var uri = new Uri(TrackUri, $"?data={base64}");

            var response = await HttpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return content.Equals("1");
        }
    }
}
