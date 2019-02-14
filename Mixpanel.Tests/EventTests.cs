using Moq;
using System;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Mixpanel.Tests
{
    public class EventTests
    {
        public Mock<HttpClient> EmptyHttpClientMock { get; }

        public string EmptyToken { get; }

        public IClient EmptyClient { get; set; }

        public EventTests()
        {
            EmptyHttpClientMock = new Mock<HttpClient>(MockBehavior.Strict);
            EmptyToken = "token";
            EmptyClient = new Client(EmptyToken, EmptyHttpClientMock.Object);
        }

        [Fact]
        public void Indexer_ThrowsArgumentException_WhenSettingAReservedProperty()
        {
            var @event = EmptyClient.CreateEvent();

            Assert.Throws<ArgumentException>(() => @event["distinct_id"] = "user_id");
            Assert.Throws<ArgumentException>(() => @event["token"] = "token");
            Assert.Throws<ArgumentException>(() => @event["time"] = DateTime.Now);
            Assert.Throws<ArgumentException>(() => @event["ip"] = IPAddress.None.ToString());
            Assert.Throws<ArgumentException>(() => @event["$insert_id"] = "unique_event_id");
            Assert.Throws<ArgumentException>(() => @event["mp_property"] = "value");
        }
    }
}
