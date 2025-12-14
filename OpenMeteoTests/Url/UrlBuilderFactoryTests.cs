using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.AirQuality;
using OpenMeteo.Elevation;
using OpenMeteo.Geocoding;
using OpenMeteo.Url;
using OpenMeteo.Weather.Forecast;
using OpenMeteo.Weather.Forecast.Metadata;
using OpenMeteo.Weather.Forecast.Options;

namespace OpenMeteoTests.Url
{
    [TestClass]
    public class UrlBuilderFactoryTests
    {
        [TestMethod]
        public void Create_ElevationUrlBuilder_Default_Works()
        {
            var builder = UrlBuilderFactory.Create<ElevationUrlBuilder>();
            Assert.IsInstanceOfType(builder, typeof(ElevationUrlBuilder));
        }

        [TestMethod]
        public void Create_GeocodingUrlBuilder_WithApiKey_Works()
        {
            var builder = UrlBuilderFactory.Create<GeocodingUrlBuilder>(apiKey: "test-key");
            Assert.IsInstanceOfType(builder, typeof(GeocodingUrlBuilder));
        }

        [TestMethod]
        public void Create_AirQualityUrlBuilder_WithCustomUri_Works()
        {
            var builder = UrlBuilderFactory.Create<AirQualityUrlBuilder>(customBaseUri: new Uri("https://custom.com"));
            Assert.IsInstanceOfType(builder, typeof(AirQualityUrlBuilder));
        }

        [TestMethod]
        public void Create_WeatherForecastUrlBuilder_WithBoth_Works()
        {
            var builder = UrlBuilderFactory.Create<WeatherForecastUrlBuilder>(
                customBaseUri: new Uri("https://custom.com"),
                apiKey: "test-key"
            );
            Assert.IsInstanceOfType(builder, typeof(WeatherForecastUrlBuilder));
        }

        [TestMethod]
        public void Create_AirQualityUrlBuilder_WithCustomUri_BuildsExpectedUrl()
        {
            var customUri = new Uri("https://custom.com");
            var builder = UrlBuilderFactory.Create<AirQualityUrlBuilder>(customBaseUri: customUri);
            var url = builder.Build();
            StringAssert.StartsWith(url, "https://custom.com");
        }

        [TestMethod]
        public void Create_WeatherForecastUrlBuilder_WithBoth_BuildsExpectedUrl()
        {
            var customUri = new Uri("https://custom.com");
            var apiKey = "test-key";
            var builder = UrlBuilderFactory.Create<WeatherForecastUrlBuilder>(customBaseUri: customUri, apiKey: apiKey);
            var url = builder.Build();
            StringAssert.StartsWith(url, "https://custom.com");
            StringAssert.Contains(url, "test-key");
        }

        [TestMethod]
        public void Create_WeatherForecastMetadataUrlBuilder_Default_Works()
        {
            var builder = UrlBuilderFactory.Create<WeatherForecastMetadataUrlBuilder>();
            Assert.IsInstanceOfType(builder, typeof(WeatherForecastMetadataUrlBuilder));
            var url = builder.WithModel(WeatherModelOptionsParameter.gfs_hrrr).Build();
            StringAssert.StartsWith(url, "https://api.open-meteo.com/data/ncep_hrrr_conus/static/meta.json");
        }

        [TestMethod]
        public void Create_WeatherForecastMetadataUrlBuilder_WithCustomUri_Works()
        {
            var customUri = new Uri("https://custom.com");
            var builder = UrlBuilderFactory.Create<WeatherForecastMetadataUrlBuilder>(customBaseUri: customUri);
            Assert.IsInstanceOfType(builder, typeof(WeatherForecastMetadataUrlBuilder));
            var url = builder.WithModel(WeatherModelOptionsParameter.gfs_hrrr).Build();
            StringAssert.StartsWith(url, "https://custom.com/data/ncep_hrrr_conus/static/meta.json");
        }

        [TestMethod]
        public void Create_WeatherForecastMetadataUrlBuilder_WithApiKey_Works()
        {
            var builder = UrlBuilderFactory.Create<WeatherForecastMetadataUrlBuilder>(apiKey: "test-key");
            Assert.IsInstanceOfType(builder, typeof(WeatherForecastMetadataUrlBuilder));
            var url = builder.WithModel(WeatherModelOptionsParameter.gfs_hrrr).Build();
            StringAssert.StartsWith(url, "https://customer-api.open-meteo.com/data/ncep_hrrr_conus/static/meta.json");
            StringAssert.Contains(url, "apikey=test-key");
        }

        [TestMethod]
        public void Create_WeatherForecastMetadataUrlBuilder_WithCustomUri_AndApiKey_Works()
        {
            var customUri = new Uri("https://custom.com");
            var builder = UrlBuilderFactory.Create<WeatherForecastMetadataUrlBuilder>(customBaseUri: customUri, apiKey: "test-key");
            Assert.IsInstanceOfType(builder, typeof(WeatherForecastMetadataUrlBuilder));
            var url = builder.WithModel(WeatherModelOptionsParameter.gfs_hrrr).Build();
            StringAssert.StartsWith(url, "https://custom.com/data/ncep_hrrr_conus/static/meta.json");
            StringAssert.Contains(url, "apikey=test-key");
        }

        [TestMethod]
        public void Create_WeatherForecastMetadataUrlBuilder_WithNullCustomUri_AndApiKey_Works()
        {
            var builder = UrlBuilderFactory.Create<WeatherForecastMetadataUrlBuilder>(customBaseUri: null, apiKey: "test-key");
            Assert.IsInstanceOfType(builder, typeof(WeatherForecastMetadataUrlBuilder));
            var url = builder.WithModel(WeatherModelOptionsParameter.gfs_hrrr).Build();
            StringAssert.StartsWith(url, "https://customer-api.open-meteo.com/data/ncep_hrrr_conus/static/meta.json");
            StringAssert.Contains(url, "apikey=test-key");
        }
    }
}
