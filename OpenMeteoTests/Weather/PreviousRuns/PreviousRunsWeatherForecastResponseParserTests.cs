using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Forecast.Options;
using OpenMeteo.Weather.PreviousRuns.Options;
using OpenMeteo.Weather.PreviousRuns.ResponseModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenMeteoTests.Weather.PreviousRuns
{
    [TestClass]
    public class PreviousRunsWeatherForecastResponseParserTests
    {
        [TestMethod]
        public async Task DeserializeJsonAsync_GroupsHourlyValuesAndUnitsByPreviousDay()
        {
            const string json = """
                {
                  "latitude": 52.5,
                  "longitude": 13.4,
                  "elevation": 38.0,
                  "generationtime_ms": 0.5,
                  "utc_offset_seconds": 0,
                  "timezone": "GMT",
                  "timezone_abbreviation": "GMT",
                  "hourly_units": {
                    "time": "iso8601",
                    "temperature_2m_previous_day1": "°C",
                    "temperature_2m_previous_day2": "°C"
                  },
                  "hourly": {
                    "time": ["2026-01-01T00:00"],
                    "temperature_2m_previous_day1": [4.5],
                    "temperature_2m_previous_day2": [3.5]
                  }
                }
                """;
            using var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var parser = new PreviousRunsWeatherForecastResponseParser(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var forecast = await parser.DeserializeJsonAsync(response, new PreviousRunsOptions
            {
                Hourly = new HourlyOptions(HourlyOptionsParameter.temperature_2m)
            });

            Assert.IsNotNull(forecast);
            Assert.IsNotNull(forecast.Hourly);
            Assert.AreEqual(4.5f, forecast.Hourly.Runs[1].Temperature_2m![0]);
            Assert.AreEqual(3.5f, forecast.Hourly.Runs[2].Temperature_2m![0]);
            Assert.AreEqual("°C", forecast.Hourly.Units[1].Temperature_2m);
            Assert.AreEqual("°C", forecast.Hourly.Units[2].Temperature_2m);
        }
    }
}
