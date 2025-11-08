using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.AirQuality;
using OpenMeteo.Elevation;
using OpenMeteo.Geocoding;
using OpenMeteo.Url;
using OpenMeteo.Weather;

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
    }
}
