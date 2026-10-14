using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Forecast.Options;
using OpenMeteo.Weather.SingleRuns;
using OpenMeteo.Weather.SingleRuns.Options;
using System;

namespace OpenMeteoTests.Weather.SingleRuns
{
    [TestClass]
    public class SingleRunsUrlBuilderTests
    {
        [TestMethod]
        public void Build_WithOptions_UsesSingleRunsEndpointAndRun()
        {
            var options = new SingleRunsOptions
            {
                Latitude = 52.52f,
                Longitude = 13.41f,
                Run = new DateTime(2026, 10, 12, 0, 0, 0, DateTimeKind.Utc),
                Hourly = new HourlyOptions(HourlyOptionsParameter.temperature_2m),
                Models = new WeatherModelOptions(WeatherModelOptionsParameter.ecmwf_ifs)
            };

            var url = new SingleRunsUrlBuilder().WithOptions(options).Build();

            Assert.AreEqual("https://single-runs-api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=GMT&timeformat=iso8601&cell_selection=land&run=2026-10-12T00%3A00&hourly=temperature_2m&models=ecmwf_ifs", url);
        }

        [TestMethod]
        public void Build_WithUnspecifiedRun_Throws()
        {
            Assert.ThrowsException<ArgumentException>(() => new SingleRunsUrlBuilder().WithOptions(new SingleRunsOptions()));
        }
    }
}