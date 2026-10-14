using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Forecast.Options;
using OpenMeteo.Weather.PreviousRuns;
using OpenMeteo.Weather.PreviousRuns.Options;
using System;

namespace OpenMeteoTests.Weather.PreviousRuns
{
    [TestClass]
    public class PreviousRunsUrlBuilderTests
    {
        [TestMethod]
        public void Build_WithPreviousDays_ExpandsHourlyParameters()
        {
            var options = new PreviousRunsOptions
            {
                Latitude = 52.52f,
                Longitude = 13.41f,
                Previous_Days = 2,
                Hourly = new HourlyOptions([HourlyOptionsParameter.temperature_2m, HourlyOptionsParameter.rain])
            };

            var url = new PreviousRunsUrlBuilder().WithOptions(options).Build();

            Assert.AreEqual("https://previous-runs-api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=GMT&timeformat=iso8601&cell_selection=land&hourly=temperature_2m_previous_day1,temperature_2m_previous_day2,rain_previous_day1,rain_previous_day2", url);
        }

        [TestMethod]
        public void Build_WithInvalidPreviousDays_Throws()
        {
            var options = new PreviousRunsOptions { Previous_Days = 0 };

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new PreviousRunsUrlBuilder().WithOptions(options));
        }
    }
}
