using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using OpenMeteo.Url;

namespace OpenMeteoTests.Utilities
{
    [TestClass]
    [Ignore]
    public class RawForecastDownloader
    {
        private readonly float _latitude = 40.7128f; // New York City
        private readonly float _longitude = -74.0060f;

        [TestMethod]
        public async Task DownloadRawForecastData_SavesToFile()
        {
            // Arrange
            var options = ForecastOptionsHelper.GetOptions(_latitude, _longitude);
            var url = UrlBuilderFactory.Create<OpenMeteo.Weather.WeatherForecastUrlBuilder>()
                .WithOptions(options)
                .WithFlatbuffers(true)
                .Build();

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var bytes = await response.Content.ReadAsByteArrayAsync();

            // Save to file next to test exe
            var testDir = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(testDir, $"forecast_raw_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bin");
            await File.WriteAllBytesAsync(filePath, bytes);

            Console.WriteLine($"Raw forecast binary saved to: {filePath}");

            Assert.IsTrue(File.Exists(filePath), $"File not found: {filePath}");
            Assert.IsTrue(new FileInfo(filePath).Length >0, "Downloaded file is empty");
        }

        [TestMethod]
        public async Task DownloadRawForecastJson_SavesToFile()
        {
            // Arrange
            var options = ForecastOptionsHelper.GetOptions(_latitude, _longitude);
            var url = UrlBuilderFactory.Create<OpenMeteo.Weather.WeatherForecastUrlBuilder>()
                .WithOptions(options)
                .Build();

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            // Save to file next to test exe
            var testDir = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(testDir, $"forecast_raw_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json");
            await File.WriteAllTextAsync(filePath, json);

            Console.WriteLine($"Raw forecast JSON saved to: {filePath}");

            Assert.IsTrue(File.Exists(filePath), $"File not found: {filePath}");
            Assert.IsTrue(new FileInfo(filePath).Length >0, "Downloaded file is empty");
        }

        [TestMethod]
        public async Task DownloadRawForecastData_AllOptions_SavesToFile()
        {
            // Arrange
            var options = ForecastOptionsHelper.GetAllOptions(_latitude, _longitude);
            var url = UrlBuilderFactory.Create<OpenMeteo.Weather.WeatherForecastUrlBuilder>()
                .WithOptions(options)
                .WithFlatbuffers(true)
                .Build();

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var bytes = await response.Content.ReadAsByteArrayAsync();

            // Save to file next to test exe
            var testDir = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(testDir, $"forecast_raw_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bin");
            await File.WriteAllBytesAsync(filePath, bytes);

            Console.WriteLine($"Raw forecast binary saved to: {filePath}");

            Assert.IsTrue(File.Exists(filePath), $"File not found: {filePath}");
            Assert.IsTrue(new FileInfo(filePath).Length > 0, "Downloaded file is empty");
        }

        [TestMethod]
        public async Task DownloadRawForecastJson_AllOptions_SavesToFile()
        {
            // Arrange
            var options = ForecastOptionsHelper.GetAllOptions(_latitude, _longitude);
            var url = UrlBuilderFactory.Create<OpenMeteo.Weather.WeatherForecastUrlBuilder>()
                .WithOptions(options)
                .Build();

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            // Save to file next to test exe
            var testDir = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(testDir, $"forecast_raw_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json");
            await File.WriteAllTextAsync(filePath, json);

            Console.WriteLine($"Raw forecast JSON saved to: {filePath}");

            Assert.IsTrue(File.Exists(filePath), $"File not found: {filePath}");
            Assert.IsTrue(new FileInfo(filePath).Length > 0, "Downloaded file is empty");
        }
    }
}
