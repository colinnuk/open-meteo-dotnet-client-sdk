using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Url;

namespace OpenMeteoTests.Url
{
    [TestClass]
    public class UrlBuilderTests
    {
        [TestMethod]
        public void Build_WithDefaults_Test()
        {
            var url = new UrlBuilder()
                .Build();
            Assert.AreEqual("https://open-meteo.com/", url);
        }

        [TestMethod]
        public void Build_WithSubdomain_Test()
        {
            var url = new UrlBuilder()
                .WithSubdomain("api")
                .Build();
            Assert.AreEqual("https://api.open-meteo.com/", url);
        }

        [TestMethod]
        public void Build_WithPath_Test()
        {
            var url = new UrlBuilder()
                .WithSubdomain("api")
                .WithPath("/v1/forecast")
                .Build();
            Assert.AreEqual("https://api.open-meteo.com/v1/forecast", url);
        }

        [TestMethod]
        public void Build_WithParameters_Test()
        {
            var url = new UrlBuilder()
                .WithSubdomain("api")
                .WithPath("/v1/forecast")
                .AddParameter("latitude", "40.7128")
                .AddParameter("longitude", "-74.006")
                .Build();
            Assert.AreEqual("https://api.open-meteo.com/v1/forecast?latitude=40.7128&longitude=-74.006", url);
        }

        [TestMethod]
        public void Build_WithApiKey_Test()
        {
            var url = new UrlBuilder()
                .WithSubdomain("api")
                .WithPath("/v1/forecast")
                .WithApiKey("testKey123")
                .Build();
            Assert.AreEqual("https://api.open-meteo.com/v1/forecast?apikey=testKey123", url);
        }

        [TestMethod]
        public void Build_WithCustomBaseUri_Test()
        {
            var customUri = new Uri("https://custom.example.com");
            var url = new UrlBuilder()
                .WithBaseUri(customUri)
                .WithPath("/v1/forecast")
                .Build();
            Assert.AreEqual("https://custom.example.com/v1/forecast", url);
        }

        [TestMethod]
        public void Build_WithCustomBaseUriAndSubdomain_Test()
        {
            var customUri = new Uri("https://example.com");
            var url = new UrlBuilder()
                .WithBaseUri(customUri)
                .WithSubdomain("api")
                .WithPath("/v1/forecast")
                .Build();
            Assert.AreEqual("https://api.example.com/v1/forecast", url);
        }

        [TestMethod]
        public void Build_WithCollection_Test()
        {
            var url = new UrlBuilder()
                .WithSubdomain("api")
                .WithPath("/v1/forecast")
                .AddCollection("hourly", ["temperature_2m", "windspeed_10m"])
                .Build();
            Assert.AreEqual("https://api.open-meteo.com/v1/forecast?hourly=temperature_2m,windspeed_10m", url);
        }

        [TestMethod]
        public void Build_WithPathNoLeadingSlash_Test()
        {
            var url = new UrlBuilder()
                .WithSubdomain("api")
                .WithPath("v1/forecast")
                .Build();
            Assert.AreEqual("https://api.open-meteo.com/v1/forecast", url);
        }
    }
}
