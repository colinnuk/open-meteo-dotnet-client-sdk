using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;

namespace OpenMeteoTests
{
    [TestClass]
    public class AirQualityUrlBuilderTests
    {
        [TestMethod]
        public void Build_WithOptions_Test()
        {
            var url = new AirQualityUrlBuilder()
                .WithOptions(GetAirQualityOptions())
                .Build();

            var expectedUrl = "https://air-quality-api.open-meteo.com/v1/air-quality?latitude=40.7128&longitude=-74.006&domains=global&timeformat=iso8601&timezone=america/new_york&hourly=pm10,pm2_5";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithOptions_WithApiKey_Test()
        {
            var url = new AirQualityUrlBuilder()
                .WithApiKey("testApiKey")
                .WithOptions(GetAirQualityOptions())
                .Build();

            var expectedUrl = "https://customer-air-quality-api.open-meteo.com/v1/air-quality?latitude=40.7128&longitude=-74.006&domains=global&timeformat=iso8601&timezone=america/new_york&hourly=pm10,pm2_5&apikey=testapikey";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithCustomBaseUri_Test()
        {
            var customUri = new Uri("https://custom.example.com");
            var url = new AirQualityUrlBuilder(customUri)
                .WithOptions(GetAirQualityOptions())
                .Build();

            var expectedUrl = "https://custom.example.com/v1/air-quality?latitude=40.7128&longitude=-74.006&domains=global&timeformat=iso8601&timezone=america/new_york&hourly=pm10,pm2_5";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithCustomBaseUri_WithApiKey_Test()
        {
            var customUri = new Uri("https://custom.example.com");
            var url = new AirQualityUrlBuilder(customUri)
                .WithApiKey("testApiKey")
                .WithOptions(GetAirQualityOptions())
                .Build();

            var expectedUrl = "https://customer-air-quality-api.custom.example.com/v1/air-quality?latitude=40.7128&longitude=-74.006&domains=global&timeformat=iso8601&timezone=america/new_york&hourly=pm10,pm2_5&apikey=testapikey";
            Assert.AreEqual(expectedUrl, url);
        }

        private static AirQualityOptions GetAirQualityOptions() => new()
        {
            Latitude = 40.7128f,
            Longitude = -74.006f,
            Domains = "global",
            Timeformat = "iso8601",
            Timezone = "America/New_York",
            Hourly = new AirQualityOptions.HourlyOptions([AirQualityOptions.HourlyOptionsParameter.pm10, AirQualityOptions.HourlyOptionsParameter.pm2_5])
        };
    }
}
