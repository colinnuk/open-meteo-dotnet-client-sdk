using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;

namespace OpenMeteoTests
{
    [TestClass]
    public class ElevationUrlBuilderTests
    {
        [TestMethod]
        public void Build_WithOptions_Test()
        {
            var url = new ElevationUrlBuilder()
                .WithOptions(GetElevationOptions())
                .Build();

            var expectedUrl = "https://api.open-meteo.com/v1/elevation?latitude=40.7128&longitude=-74.006";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithOptions_WithApiKey_Test()
        {
            var url = new ElevationUrlBuilder()
                .WithApiKey("testApiKey")
                .WithOptions(GetElevationOptions())
                .Build();

            var expectedUrl = "https://customer-api.open-meteo.com/v1/elevation?latitude=40.7128&longitude=-74.006&apikey=testapikey";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithCustomBaseUri_Test()
        {
            var customUri = new Uri("https://custom.example.com");
            var url = new ElevationUrlBuilder(customUri)
                .WithOptions(GetElevationOptions())
                .Build();

            var expectedUrl = "https://custom.example.com/v1/elevation?latitude=40.7128&longitude=-74.006";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithCustomBaseUri_WithApiKey_Test()
        {
            var customUri = new Uri("https://custom.example.com");
            var url = new ElevationUrlBuilder(customUri)
                .WithApiKey("testApiKey")
                .WithOptions(GetElevationOptions())
                .Build();

            var expectedUrl = "https://customer-api.custom.example.com/v1/elevation?latitude=40.7128&longitude=-74.006&apikey=testapikey";
            Assert.AreEqual(expectedUrl, url);
        }

        private static ElevationOptions GetElevationOptions() => new(40.7128f, -74.006f);
    }
}
