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

            Assert.AreEqual(1.0896308f, weatherData.Latitude);
            Assert.AreEqual(2.2695036f, weatherData.Longitude);
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
            Assert.AreEqual(49.1f, result.Latitude, 0.1f);
            Assert.AreEqual(-122.6f, result.Longitude, 0.1f);
            Assert.IsNotNull(result.Hourly);
            Assert.IsNotNull(result.Daily);
            Assert.IsNotNull(result.Current);
            Assert.IsNotNull(result.Minutely15);
        }

        [DataTestMethod]
        [DataRow(WeatherModelOptionsParameter.best_match, 52.52, 13.405)]
        [DataRow(WeatherModelOptionsParameter.ecmwf_ifs, 48.8566, 2.3522)] // Paris
        [DataRow(WeatherModelOptionsParameter.ecmwf_ifs025, 48.8566, 2.3522)] // Paris
        [DataRow(WeatherModelOptionsParameter.icon_global, 52.52, 13.405)] // Berlin
        [DataRow(WeatherModelOptionsParameter.geosphere_arome_austria, 48.201, 16.362)] // Vienna
        [DataRow(WeatherModelOptionsParameter.icon_eu, 52.52, 13.405)] // Berlin
        [DataRow(WeatherModelOptionsParameter.icon_d2, 51.5074, -0.1278)] // London
        [DataRow(WeatherModelOptionsParameter.gfs_seamless, 40.7128, -74.0060)] // New York
        [DataRow(WeatherModelOptionsParameter.gfs_global, 40.7128, -74.0060)] // New York
        [DataRow(WeatherModelOptionsParameter.gfs_graphcast025, 52.52, 13.405)] // Berlin
        [DataRow(WeatherModelOptionsParameter.gem_global, 45.4215, -75.6997)] // Ottawa
        [DataRow(WeatherModelOptionsParameter.metno_nordic, 59.9139, 10.7522)] // Oslo
        [DataRow(WeatherModelOptionsParameter.jma_gsm, 35.6762, 139.6503)] // Tokyo
        [DataRow(WeatherModelOptionsParameter.bom_access_global, -33.8688, 151.2093)] // Sydney
        [DataRow(WeatherModelOptionsParameter.ukmo_global_deterministic_10km, 51.5074, -0.1278)] // London
        [DataRow(WeatherModelOptionsParameter.ncep_aigfs025, 40.7128, -74.0060)] // New York
        [DataRow(WeatherModelOptionsParameter.ncep_hgefs025_ensemble_mean, 40.7128, -74.0060)] // New York
        [DataRow(WeatherModelOptionsParameter.ncep_nam_conus, 40.7128, -74.0060)] // New York
        public async Task QueryWeatherApiAsync_WithModel_ReturnsWeatherForecast(WeatherModelOptionsParameter model, double lat, double lon)
        {
            var client = new OpenMeteoClient();
            var options = new WeatherForecastOptions
            {
                Latitude = (float)lat,
                Longitude = (float)lon,
                Hourly = new HourlyOptions([
                    HourlyOptionsParameter.temperature_2m,
                    HourlyOptionsParameter.precipitation
                ]),
                Models = new WeatherModelOptions(model)
            };
            
            var result = await client.QueryWeatherApiAsync(options);
            
            Assert.IsNotNull(result);
            Assert.AreEqual((float)lat, result.Latitude, 0.5f);
            Assert.AreEqual((float)lon, result.Longitude, 0.5f);
            Assert.IsNotNull(result.Hourly);
        }
    }
}
