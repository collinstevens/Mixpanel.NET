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
    public class ClientTests
    {
        public Mock<HttpClient> EmptyHttpClientMock { get; }

        public string EmptyToken { get; }

        public ClientTests()
        {
            EmptyHttpClientMock = new Mock<HttpClient>(MockBehavior.Strict);
            EmptyToken = "token";
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenTokenIsNull() => Assert.Throws<ArgumentNullException>(() => new Client(null, EmptyHttpClientMock.Object));

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenTokenIsEmpty() => Assert.Throws<ArgumentException>(() => new Client(string.Empty, EmptyHttpClientMock.Object));

        [Fact]
        public void Constructor_ThrowsArgumentException_WhenTokenIsWhiteSpace() => Assert.Throws<ArgumentException>(() => new Client(" ", EmptyHttpClientMock.Object));

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenHttpClientIsNull() => Assert.Throws<ArgumentNullException>(() => new Client(EmptyToken, null));

        [Fact]
        public void Constructor_Constructs()
        {
            IClient client = new Client(EmptyToken, EmptyHttpClientMock.Object);
        }

        [Fact]
        public void TrackAsync_ThrowsArgumentNullException_WhenEventIsNull()
        {
            IClient client = new Client(EmptyToken, EmptyHttpClientMock.Object);

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

            IClient client = new Client(EmptyToken, httpClient);

            var @event = client.CreateEvent();

            var successful = await client.TrackAsync(@event).ConfigureAwait(false);

            Assert.True(successful);

            httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(request => request.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>());
        }
    }
}
