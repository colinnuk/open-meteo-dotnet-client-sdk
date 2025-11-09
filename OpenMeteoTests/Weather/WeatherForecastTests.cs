using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;
using OpenMeteo.Geocoding;
using OpenMeteo.Weather.Options;
using OpenMeteo.Weather.ResponseModel;

namespace OpenMeteoTests.Weather
{
    [TestClass]
    public class WeatherForecastTests
    {
        [TestMethod]
        [Ignore] // Ignored to reduce the number of API calls during testing
        public async Task Only_Location_Name_Test()
        {
            OpenMeteoClient client = new();
            string location = "Tokyo";
            WeatherForecast weatherData = await client.QueryWeatherApiAsync(location);

            Assert.IsNotNull(weatherData);
            Assert.IsNotNull(weatherData.Longitude);
            Assert.IsNotNull(weatherData.Latitude);
        }

        [TestMethod]
        [Ignore] // Ignored to reduce the number of API calls during testing
        public async Task Latitude_Longitude_Test()
        {
            OpenMeteoClient client = new();
            
            WeatherForecast weatherData = await client.QueryWeatherApiAsync(1.125f, 2.25f);

            Assert.IsNotNull(weatherData);
            Assert.IsNotNull(weatherData.Longitude);
            Assert.IsNotNull(weatherData.Latitude);

            Assert.AreEqual(1.125f, weatherData.Latitude);
            Assert.AreEqual(2.25f, weatherData.Longitude);
        }

        [TestMethod]
        [Ignore] // Ignored to reduce the number of API calls during testing
        public async Task GeocodingOptions_Test()
        {
            OpenMeteoClient client = new();
            GeocodingOptions options = new("Tokyo");
            WeatherForecast weatherData = await client.QueryWeatherApiAsync(options);

            Assert.IsNotNull(weatherData);
            Assert.IsNotNull(weatherData.Longitude);
            Assert.IsNotNull(weatherData.Latitude);
        }

        [TestMethod]
        [Ignore] // Ignored to reduce the number of API calls during testing
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
            OpenMeteoClient client = new();
            var options = new WeatherForecastOptions(
                0f, 
                0f, 
                TemperatureUnitType.celsius, 
                WindspeedUnitType.kmh, 
                PrecipitationUnitType.mm, 
                "GMT", 
                null, 
                null, 
                null,
                null,
                TimeformatType.iso8601, 
                0,
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-2),
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
                null,
                CellSelectionType.nearest
                );

            var res = await client.QueryWeatherApiAsync("Tokyo", options);

            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void WeatherForecast_With_All_Options_Test()
        {
            WeatherForecastOptions options = new()
            {
                Hourly = HourlyOptions.All,
                Daily = DailyOptions.All,
                Models = WeatherModelOptions.All,
                Current = CurrentOptions.All,
                Minutely_15 = Minutely15Options.All
            };

            Assert.IsTrue(HourlyOptions.All.Parameter.All(p => options.Hourly.Parameter.Contains(p)));
            Assert.IsTrue(DailyOptions.All.Parameter.All(p => options.Daily.Parameter.Contains(p)));
            Assert.IsTrue(WeatherModelOptions.All.Parameter.All(p => options.Models.Parameter.Contains(p)));
            Assert.IsTrue(CurrentOptions.All.Parameter.All(p => options.Current.Parameter.Contains(p)));
            Assert.IsTrue(Minutely15Options.All.Parameter.All(p => options.Minutely_15.Parameter.Contains(p)));
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
        [Ignore]
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
    }
}
