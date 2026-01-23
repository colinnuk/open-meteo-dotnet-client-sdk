using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;
using OpenMeteo.Geocoding;
using OpenMeteo.Weather.Forecast.Options;

namespace OpenMeteoTests.Weather.Forecast
{
    [TestClass]
    [TestCategory(TestCategoryConstants.Integration)]
    public class WeatherForecastApiTests
    {
        [TestMethod]
        public async Task Only_Location_Name_Test()
        {
            OpenMeteoClient client = new();
            string location = "Tokyo";
            var weatherData = await client.QueryWeatherApiAsync(location);

            Assert.IsNotNull(weatherData);
            Assert.IsNotNull(weatherData.Longitude);
            Assert.IsNotNull(weatherData.Latitude);
        }

        [TestMethod]
        public async Task Latitude_Longitude_Test()
        {
            OpenMeteoClient client = new();
            
            var weatherData = await client.QueryWeatherApiAsync(1.125f, 2.25f);

            Assert.IsNotNull(weatherData);
            Assert.IsNotNull(weatherData.Longitude);
            Assert.IsNotNull(weatherData.Latitude);

            Assert.AreEqual(1.125f, weatherData.Latitude);
            Assert.AreEqual(2.25f, weatherData.Longitude);
        }

        [TestMethod]
        public async Task GeocodingOptions_Test()
        {
            OpenMeteoClient client = new();
            GeocodingOptions options = new("Tokyo");
            var weatherData = await client.QueryWeatherApiAsync(options);

            Assert.IsNotNull(weatherData);
            Assert.IsNotNull(weatherData.Longitude);
            Assert.IsNotNull(weatherData.Latitude);
        }

        [TestMethod]
        public async Task WeatherForecast_With_WeatherForecastOptions_Test()
        {
            OpenMeteoClient client = new();
            WeatherForecastOptions weatherForecast = new();

            var res = await client.QueryWeatherApiAsync(weatherForecast);

            Assert.IsNotNull(res);
            Assert.AreEqual(0f, res.Latitude);
            Assert.AreEqual(0f, res.Longitude);
        }

        [TestMethod]
        public async Task WeatherForecast_With_String_And_Options_Test()
        {
            OpenMeteoClient client = new() { RethrowExceptions = true };
            var options = new WeatherForecastOptions(
                0f, 
                0f, 
                TemperatureUnitType.celsius, 
                WindspeedUnitType.kmh, 
                PrecipitationUnitType.mm, 
                "GMT", 
                null!, 
                null!, 
                null!,
                null!,
                TimeformatType.iso8601, 
                0,
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-2),
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
                null!,
                CellSelectionType.nearest
                );

            var res = await client.QueryWeatherApiAsync("Tokyo", options);

            Assert.IsNotNull(res);
        }

        [TestMethod]
        public async Task Latitude_Longitude_No_Data_For_Selected_Forecast_Rethrows_Test()
        {
            OpenMeteoClient client = new()
            {
                RethrowExceptions = true
            };

            WeatherForecastOptions options = new()
            {
                Latitude = 1,
                Longitude = 1,
                Models = new WeatherModelOptions(WeatherModelOptionsParameter.gfs_hrrr),
            };

            var ex = await Assert.ThrowsExceptionAsync<OpenMeteoClientException>(async () => await client.QueryWeatherApiAsync(options));
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, ex.StatusCode);
            Assert.AreEqual("No data is available for this location", ex.Message);
        }

        [TestMethod]
        public async Task FlatBuffers_Enabled_Returns_WeatherForecast()
        {
            var client = new OpenMeteoClient() { UseFlatbuffers = true };
            var options = new WeatherForecastOptions(52.52f, 13.41f);
            var result = await client.QueryWeatherApiAsync(options);
            Assert.IsNotNull(result);
            Assert.AreEqual(52.52f, result.Latitude, 0.01f);
            Assert.AreEqual(13.41f, result.Longitude, 0.01f);
        }

        [TestMethod]
        public async Task FlatBuffers_ComplexOptions_Returns_WeatherForecast()
        {
            var client = new OpenMeteoClient() { UseFlatbuffers = true };
            var options = new WeatherForecastOptions
            {
                Latitude = 52.52f,
                Longitude = 13.41f,
                Hourly = HourlyOptions.All,
                Daily = DailyOptions.All,
                Current = CurrentOptions.All,
                Minutely_15 = Minutely15Options.All,
                Models = new WeatherModelOptions(WeatherModelOptionsParameter.best_match)
            };
            var result = await client.QueryWeatherApiAsync(options);
            Assert.IsNotNull(result);
            Assert.AreEqual(52.52f, result.Latitude, 0.01f);
            Assert.AreEqual(13.41f, result.Longitude, 0.01f);
            Assert.IsNotNull(result.Hourly);
            Assert.IsNotNull(result.Daily);
            Assert.IsNotNull(result.Current);
            Assert.IsNotNull(result.Minutely15);
        }

        [TestMethod]
        public async Task FlatBuffers_ComplexOptions_WithTimeZone_Returns_WeatherForecast()
        {
            var client = new OpenMeteoClient() { UseFlatbuffers = true };
            var options = new WeatherForecastOptions
            {
                Latitude = 49.1f,
                Longitude = -122.6f,
                Timezone = "America/Vancouver",
                Hourly = HourlyOptions.All,
                Daily = DailyOptions.All,
                Current = CurrentOptions.All,
                Minutely_15 = Minutely15Options.All,
                Models = new WeatherModelOptions(WeatherModelOptionsParameter.best_match)
            };
            var result = await client.QueryWeatherApiAsync(options);
            Assert.IsNotNull(result);
            Assert.AreEqual(49.1f, result.Latitude, 0.01f);
            Assert.AreEqual(-122.6f, result.Longitude, 0.01f);
            Assert.IsNotNull(result.Hourly);
            Assert.IsNotNull(result.Daily);
            Assert.IsNotNull(result.Current);
            Assert.IsNotNull(result.Minutely15);
        }
    }
}
