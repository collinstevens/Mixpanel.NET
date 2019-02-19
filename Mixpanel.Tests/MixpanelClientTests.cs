using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Mixpanel.Tests
{
    public class MixpanelClientTests
    {
        public Mock<HttpClient> EmptyHttpClientMock { get; }

        public string EmptyToken { get; }

        public MixpanelClientTests()
        {
            EmptyHttpClientMock = new Mock<HttpClient>(MockBehavior.Strict);
            EmptyToken = "token";
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenTokenIsNull() => Assert.Throws<ArgumentNullException>(() => new MixpanelClient(null, EmptyHttpClientMock.Object));

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenTokenIsEmpty() => Assert.Throws<ArgumentException>(() => new MixpanelClient(string.Empty, EmptyHttpClientMock.Object));

        [Fact]
        public void Constructor_ThrowsArgumentException_WhenTokenIsWhiteSpace() => Assert.Throws<ArgumentException>(() => new MixpanelClient(" ", EmptyHttpClientMock.Object));

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenHttpClientIsNull() => Assert.Throws<ArgumentNullException>(() => new MixpanelClient(EmptyToken, null));

        [Fact]
        public void Constructor_Constructs()
        {
            IMixpanelClient client = new MixpanelClient(EmptyToken, EmptyHttpClientMock.Object);
        }

        [Fact]
        public void TrackAsync_ThrowsArgumentNullException_WhenEventIsNull()
        {
            IMixpanelClient client = new MixpanelClient(EmptyToken, EmptyHttpClientMock.Object);

            Assert.ThrowsAsync<ArgumentNullException>(async () => await client.TrackAsync(null).ConfigureAwait(false));
        }

        [Fact]
        public async Task TrackAsync_SendsSuccessfully_WhenEventIsNotNull()
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("1") })
                .Verifiable();

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);

            IMixpanelClient client = new MixpanelClient(EmptyToken, httpClient);

            var @event = client.CreateEvent();

            var outcome = await client.TrackAsync(@event).ConfigureAwait(false);

            Assert.True(outcome.Successful);

            httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(request => request.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>());
        }
    }
}
