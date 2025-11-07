using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;

namespace OpenMeteoTests
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

            var expectedUrl = "https://geocoding-api.open-meteo.com/v1/search?name=new york&count=100&format=json&language=en";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithOptions_WithApiKey_Test()
        {
            var url = new GeocodingUrlBuilder()
                .WithApiKey("testApiKey")
                .WithOptions(GetGeocodingOptions())
                .Build();

            var expectedUrl = "https://customer-geocoding-api.open-meteo.com/v1/search?name=new york&count=100&format=json&language=en&apikey=testapikey";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithCustomBaseUri_Test()
        {
            var customUri = new Uri("https://custom.example.com");
            var url = new GeocodingUrlBuilder(customUri)
                .WithOptions(GetGeocodingOptions())
                .Build();

            var expectedUrl = "https://custom.example.com/v1/search?name=new york&count=100&format=json&language=en";
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

            var expectedUrl = "https://customer-geocoding-api.custom.example.com/v1/search?name=new york&count=100&format=json&language=en&apikey=testapikey";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithDefaultCount_Test()
        {
            var options = new GeocodingOptions("Berlin");
            var url = new GeocodingUrlBuilder()
                .WithOptions(options)
                .Build();

            var expectedUrl = "https://geocoding-api.open-meteo.com/v1/search?name=berlin&count=1&format=json&language=en";
            Assert.AreEqual(expectedUrl, url);
        }

        private static GeocodingOptions GetGeocodingOptions() => new("New York");
    }
}
