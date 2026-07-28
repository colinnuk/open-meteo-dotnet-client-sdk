using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Forecast.Options;
using OpenMeteo.Weather.Forecast.ResponseModel;
using OpenMeteoTests.Utilities;

namespace OpenMeteoTests.Weather.Forecast
{
    [TestClass]
    public class WeatherForecastResponseParserTests
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        [DataTestMethod]
        [DataRow("JsonResponse_UTC.json", "GMT", 0)]
        [DataRow("JsonResponse_NonUTC.json", "America/Vancouver", -8)]
        public async Task DeserializeJsonAsync_JsonFile_ProducesCorrectHourlyTime(string fileName, string timezone, int expectedOffsetHours)
        {
            var filePath = Path.Combine("Weather", "Forecast", "ExampleResponses", fileName);
            var json = await File.ReadAllTextAsync(filePath);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
            var options = new WeatherForecastOptions
            {
                Timezone = timezone,
                Hourly = new HourlyOptions(HourlyOptionsParameter.temperature_2m)
            };
            var parser = new WeatherForecastResponseParser(_jsonSerializerOptions);

            var forecast = await parser.DeserializeJsonAsync(response, options);

            Assert.IsNotNull(forecast);
            Assert.IsNotNull(forecast.Hourly);
            Assert.IsNotNull(forecast.Hourly.Time);
            foreach (var dt in forecast.Hourly.Time)
            {
                Assert.AreEqual(TimeSpan.FromHours(expectedOffsetHours), dt.Offset);
            }
        }

        [TestMethod]
        public async Task DeserializeJsonListAsync_MultipleForecasts_ReturnsAllForecasts()
        {
            const string json = "[{\"latitude\":52.52,\"longitude\":13.41},{\"latitude\":50.12,\"longitude\":8.68}]";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
            var parser = new WeatherForecastResponseParser(_jsonSerializerOptions);

            var forecasts = await parser.DeserializeJsonListAsync(response, new WeatherForecastOptions());

            Assert.IsNotNull(forecasts);
            Assert.AreEqual(2, forecasts.Count);
            Assert.AreEqual(52.52f, forecasts[0].Latitude);
            Assert.AreEqual(8.68f, forecasts[1].Longitude);
        }

        [DataTestMethod]
        [DataRow("FlatbuffersResponse_UTC", "GMT", 0)]
        [DataRow("FlatbuffersResponse_NonUTC", "America/Vancouver", -8)]
        public async Task ConvertFlatBuffersAsync_FlatBuffersFile_ProducesCorrectHourlyTime(string fileName, string timezone, int expectedOffsetHours)
        {
            var filePath = Path.Combine("Weather", "Forecast", "ExampleResponses", fileName);
            var bytes = await File.ReadAllBytesAsync(filePath);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            };
            var options = new WeatherForecastOptions
            {
                Timezone = timezone,
                Hourly = new HourlyOptions(HourlyOptionsParameter.temperature_2m)
            };
            var parser = new WeatherForecastResponseParser(_jsonSerializerOptions);

            var forecast = await parser.ConvertFlatBuffersAsync(response, options);

            Assert.IsNotNull(forecast);
            Assert.IsNotNull(forecast.Hourly);
            Assert.IsNotNull(forecast.Hourly.Time);
            foreach (var dt in forecast.Hourly.Time)
            {
                Assert.AreEqual(TimeSpan.FromHours(expectedOffsetHours), dt.Offset);
            }
        }

        [TestMethod]
        public async Task ConvertFlatBuffersListAsync_ConcatenatedResponses_ReturnsAllForecasts()
        {
            var filePath = Path.Combine("Weather", "Forecast", "ExampleResponses", "FlatbuffersResponse_UTC");
            var bytes = await File.ReadAllBytesAsync(filePath);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes.Concat(bytes).ToArray())
            };
            var parser = new WeatherForecastResponseParser(_jsonSerializerOptions);

            var forecasts = await parser.ConvertFlatBuffersListAsync(response, new WeatherForecastOptions { Timezone = "GMT" });

            Assert.IsNotNull(forecasts);
            Assert.AreEqual(2, forecasts.Count);
            Assert.AreEqual(forecasts[0].Latitude, forecasts[1].Latitude);
            Assert.AreEqual(forecasts[0].Longitude, forecasts[1].Longitude);
        }

        [DataTestMethod]
        [DataRow("forecast_hrrr_nyc_20251111_021159.json", "forecast_hrrr_nyc_20251111_021158.bin", false)]
        [DataRow("forecast_hrrr_nyc_all_20251111_032924.json", "forecast_hrrr_nyc_all_20251111_032915.bin", true)]
        public async Task FlatbufferAndJsonProduceIdenticalWeatherForecastObjects(string jsonFile, string binFile, bool isAllOptions)
        {
            var jsonPath = Path.Combine("Weather", "Forecast", "ExampleResponses", jsonFile);
            var binPath = Path.Combine("Weather", "Forecast", "ExampleResponses", binFile);

            var json = await File.ReadAllTextAsync(jsonPath);
            var bin = await File.ReadAllBytesAsync(binPath);

            var jsonResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
            var binResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bin)
            };

            var options = isAllOptions ? 
                  ForecastOptionsHelper.GetAllOptions(40.7128f, -74.0060f)
                : ForecastOptionsHelper.GetOptions(40.7128f, -74.0060f);
            var parser = new WeatherForecastResponseParser(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var swJson = System.Diagnostics.Stopwatch.StartNew();
            var jsonForecast = await parser.DeserializeJsonAsync(jsonResponse, options);
            swJson.Stop();
            Console.WriteLine($"JSON parse time: {swJson.ElapsedMilliseconds} ms");

            var swFlat = System.Diagnostics.Stopwatch.StartNew();
            var flatbufferForecast = await parser.ConvertFlatBuffersAsync(binResponse, options);
            swFlat.Stop();
            Console.WriteLine($"FlatBuffer parse time: {swFlat.ElapsedMilliseconds} ms");

            Assert.IsNotNull(jsonForecast);
            Assert.IsNotNull(flatbufferForecast);

            var result = WeatherForecastComparer.Compare(jsonForecast, flatbufferForecast);
            if (!result.IsEqual)
            {
                Console.WriteLine("Unequal fields:");
                foreach (var field in result.UnequalFields)
                    Console.WriteLine(field);
            }
            Assert.IsTrue(result.IsEqual);
        }
    }
}
