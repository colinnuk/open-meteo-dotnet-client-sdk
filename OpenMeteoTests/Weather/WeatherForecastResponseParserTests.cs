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

        [DataTestMethod]
        [DataRow("JsonResponse_UTC.json", "GMT", 0)]
        [DataRow("JsonResponse_NonUTC.json", "America/Vancouver", -8)]
        public async Task DeserializeJsonAsync_JsonFile_ProducesCorrectHourlyTime(string fileName, string timezone, int expectedOffsetHours)
        {
            var filePath = Path.Combine("Weather", "ExampleResponses", fileName);
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

        [DataTestMethod]
        [DataRow("FlatbuffersResponse_UTC", "GMT", 0)]
        [DataRow("FlatbuffersResponse_NonUTC", "America/Vancouver", -8)]
        public async Task ConvertFlatBuffersAsync_FlatBuffersFile_ProducesCorrectHourlyTime(string fileName, string timezone, int expectedOffsetHours)
        {
            var filePath = Path.Combine("Weather", "ExampleResponses", fileName);
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

        [DataTestMethod]
        [DataRow("forecast_hrrr_nyc_20251111_021159.json", "forecast_hrrr_nyc_20251111_021158.bin", false)]
        [DataRow("forecast_hrrr_nyc_all_20251111_032924.json", "forecast_hrrr_nyc_all_20251111_032915.bin", true)]
        public async Task FlatbufferAndJsonProduceIdenticalWeatherForecastObjects(string jsonFile, string binFile, bool isAllOptions)
        {
            var jsonPath = Path.Combine("Weather", "ExampleResponses", jsonFile);
            var binPath = Path.Combine("Weather", "ExampleResponses", binFile);

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
