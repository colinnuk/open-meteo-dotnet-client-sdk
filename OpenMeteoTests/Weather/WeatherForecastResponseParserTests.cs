using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Options;
using OpenMeteo.Weather.ResponseModel;
using OpenMeteoTests.Utilities;

namespace OpenMeteoTests.Weather
{
    [TestClass]
    public class WeatherForecastResponseParserTests
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        [TestMethod]
        public async Task DeserializeJsonAsync_UTCJsonFile_ProducesCorrectHourlyTime()
        {
            var filePath = Path.Combine("Weather", "ExampleResponses", "JsonResponse_UTC.json");
            var json = await File.ReadAllTextAsync(filePath);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
            var options = new WeatherForecastOptions
            {
                Timezone = "GMT",
                Hourly = new HourlyOptions(HourlyOptionsParameter.temperature_2m)
            };
            var parser = new WeatherForecastResponseParser(_jsonSerializerOptions);

            var forecast = await parser.DeserializeJsonAsync(response, options);

            Assert.IsNotNull(forecast);
            Assert.IsNotNull(forecast.Hourly);
            Assert.IsNotNull(forecast.Hourly.Time);
            foreach (var dt in forecast.Hourly.Time)
            {
                Assert.AreEqual(TimeSpan.Zero, dt.Offset); // GMT offset is zero
            }
        }

        [TestMethod]
        public async Task DeserializeJsonAsync_NonUTCJsonFile_ProducesCorrectHourlyTime()
        {
            var filePath = Path.Combine("Weather", "ExampleResponses", "JsonResponse_NonUTC.json");
            var json = await File.ReadAllTextAsync(filePath);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
            var options = new WeatherForecastOptions
            {
                Timezone = "America/Vancouver",
                Hourly = new HourlyOptions(HourlyOptionsParameter.temperature_2m)
            };
            var parser = new WeatherForecastResponseParser(_jsonSerializerOptions);

            var forecast = await parser.DeserializeJsonAsync(response, options);

            Assert.IsNotNull(forecast);
            Assert.IsNotNull(forecast.Hourly);
            Assert.IsNotNull(forecast.Hourly.Time);
            foreach (var dt in forecast.Hourly.Time)
            {
                Assert.IsTrue(dt.Offset.Hours <= -7 && dt.Offset.Hours >= -8);
            }
        }

        [TestMethod]
        public async Task ConvertFlatBuffersAsync_UTCFlatBuffersFile_ProducesCorrectHourlyTime()
        {
            var filePath = Path.Combine("Weather", "ExampleResponses", "FlatbuffersResponse_UTC");
            var bytes = await File.ReadAllBytesAsync(filePath);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            };
            var options = new WeatherForecastOptions
            {
                Timezone = "GMT",
                Hourly = new HourlyOptions(HourlyOptionsParameter.temperature_2m)
            };
            var parser = new WeatherForecastResponseParser(_jsonSerializerOptions);

            var forecast = await parser.ConvertFlatBuffersAsync(response, options);

            Assert.IsNotNull(forecast);
            Assert.IsNotNull(forecast.Hourly);
            Assert.IsNotNull(forecast.Hourly.Time);
            foreach (var dt in forecast.Hourly.Time)
            {
                Assert.AreEqual(TimeSpan.Zero, dt.Offset); // GMT offset is zero
            }
        }

        [TestMethod]
        public async Task ConvertFlatBuffersAsync_NonUTCFlatBuffersFile_ProducesCorrectHourlyTime()
        {
            var filePath = Path.Combine("Weather", "ExampleResponses", "FlatbuffersResponse_NonUTC");
            var bytes = await File.ReadAllBytesAsync(filePath);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            };
            var options = new WeatherForecastOptions
            {
                Timezone = "America/Vancouver",
                Hourly = new HourlyOptions(HourlyOptionsParameter.temperature_2m)
            };
            var parser = new WeatherForecastResponseParser(_jsonSerializerOptions);

            var forecast = await parser.ConvertFlatBuffersAsync(response, options);

            Assert.IsNotNull(forecast);
            Assert.IsNotNull(forecast.Hourly);
            Assert.IsNotNull(forecast.Hourly.Time);
            foreach (var dt in forecast.Hourly.Time)
            {
                Assert.IsTrue(dt.Offset.Hours <= -7 && dt.Offset.Hours >= -8);
            }
        }

        [TestMethod]
        public async Task FlatbufferAndJsonProduceIdenticalWeatherForecastObjects()
        {
            var jsonPath = Path.Combine("Weather", "ExampleResponses", "forecast_hrrr_nyc_20251111_021159.json");
            var binPath = Path.Combine("Weather", "ExampleResponses", "forecast_hrrr_nyc_20251111_021158.bin");

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

            var options = ForecastOptionsHelper.GetOptions(40.7128f, -74.0060f);
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
            Assert.IsTrue(WeatherForecastComparer.WeatherForecastsAreEqual(jsonForecast, flatbufferForecast));
        }
    }
}
