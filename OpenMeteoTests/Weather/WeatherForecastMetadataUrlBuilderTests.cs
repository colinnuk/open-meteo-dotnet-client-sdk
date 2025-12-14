using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Metadata;
using OpenMeteo.Weather.Options;

namespace OpenMeteoTests.Weather
{
    [TestClass]
    public class WeatherForecastMetadataUrlBuilderTests
    {
        [TestMethod]
        public void Build_DefaultConstructor_WithModel_Test()
        {
            var url = new WeatherForecastMetadataUrlBuilder()
            .WithModel(WeatherModelOptionsParameter.gfs_hrrr)
            .Build();
            var expectedUrl = "https://api.open-meteo.com/data/ncep_hrrr_conus/static/meta.json";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_CustomBaseUri_WithModel_Test()
        {
            var customUri = new Uri("https://custom.example.com");
            var url = new WeatherForecastMetadataUrlBuilder(customUri)
            .WithModel(WeatherModelOptionsParameter.gfs_global)
            .Build();
            var expectedUrl = "https://custom.example.com/data/ncep_gfs013/static/meta.json";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_ApiKey_WithModel_Test()
        {
            var url = new WeatherForecastMetadataUrlBuilder("testApiKey")
            .WithModel(WeatherModelOptionsParameter.gfs_hrrr)
            .Build();
            var expectedUrl = "https://customer-api.open-meteo.com/data/ncep_hrrr_conus/static/meta.json?apikey=testApiKey";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_CustomBaseUri_ApiKey_WithModel_Test()
        {
            var customUri = new Uri("https://custom.example.com");
            var url = new WeatherForecastMetadataUrlBuilder(customUri, "testApiKey")
            .WithModel(WeatherModelOptionsParameter.gfs_global)
            .Build();
            var expectedUrl = "https://custom.example.com/data/ncep_gfs013/static/meta.json?apikey=testApiKey";
            Assert.AreEqual(expectedUrl, url);
        }
    }
}
