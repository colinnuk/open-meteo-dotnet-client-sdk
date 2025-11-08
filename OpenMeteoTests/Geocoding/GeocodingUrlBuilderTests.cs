using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;
using OpenMeteo.Geocoding;

namespace OpenMeteoTests.Geocoding
{
    [TestClass]
    public class GeocodingUrlBuilderTests
    {
        [TestMethod]
        public void Build_WithOptions_Test()
        {
            var url = new GeocodingUrlBuilder()
                .WithOptions(GetGeocodingOptions())
                .Build();

            var expectedUrl = "https://geocoding-api.open-meteo.com/v1/search?name=New York&count=100&format=json&language=en";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithOptions_WithApiKey_Test()
        {
            var url = new GeocodingUrlBuilder("testApiKey")
                .WithOptions(GetGeocodingOptions())
                .Build();

            var expectedUrl = "https://customer-geocoding-api.open-meteo.com/v1/search?name=New York&count=100&format=json&language=en&apikey=testApiKey";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithCustomBaseUri_Test()
        {
            var customUri = new Uri("https://custom.example.com");
            var url = new GeocodingUrlBuilder(customUri)
                .WithOptions(GetGeocodingOptions())
                .Build();

            var expectedUrl = "https://custom.example.com/v1/search?name=New York&count=100&format=json&language=en";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithCustomBaseUri_WithApiKey_Test()
        {
            var customUri = new Uri("https://custom.example.com");
            var url = new GeocodingUrlBuilder(customUri)
                .WithApiKey("testApiKey")
                .WithOptions(GetGeocodingOptions())
                .Build();

            var expectedUrl = "https://custom.example.com/v1/search?name=New York&count=100&format=json&language=en&apikey=testApiKey";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithCustomBaseUriAndApiKey_Constructor_Test()
        {
            var customUri = new Uri("https://custom.example.com");
            var url = new GeocodingUrlBuilder(customUri, "testApiKey")
                .WithOptions(GetGeocodingOptions())
                .Build();

            var expectedUrl = "https://custom.example.com/v1/search?name=New York&count=100&format=json&language=en&apikey=testApiKey";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithDefaultCount_Test()
        {
            var options = new GeocodingOptions("Berlin");
            var url = new GeocodingUrlBuilder()
                .WithOptions(options)
                .Build();

            var expectedUrl = "https://geocoding-api.open-meteo.com/v1/search?name=Berlin&count=100&format=json&language=en";
            Assert.AreEqual(expectedUrl, url);
        }

        private static GeocodingOptions GetGeocodingOptions() => new("New York");
    }
}
