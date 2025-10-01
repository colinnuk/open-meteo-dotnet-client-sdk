using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;

namespace OpenMeteoTests
{
    [TestClass]
    public class WeatherForecastTests
    {
        [TestMethod]
        public async Task Only_Location_Name_Test()
        {
            OpenMeteoClient client = new OpenMeteoClient();
            string location = "Tokyo";
            WeatherForecast weatherData = await client.QueryAsync(location);

            Assert.IsNotNull(weatherData);
            Assert.IsNotNull(weatherData.Longitude);
            Assert.IsNotNull(weatherData.Latitude);
        }

        [TestMethod]
        public async Task Latitude_Longitude_Test()
        {
            OpenMeteoClient client = new OpenMeteoClient();
            
            WeatherForecast weatherData = await client.QueryAsync(1.125f, 2.25f);

            Assert.IsNotNull(weatherData);
            Assert.IsNotNull(weatherData.Longitude);
            Assert.IsNotNull(weatherData.Latitude);

            Assert.AreEqual(1.125f, weatherData.Latitude);
            Assert.AreEqual(2.25f, weatherData.Longitude);
        }

        [TestMethod]
        public async Task Latitude_Longitude_Test_With_French_Culture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

            OpenMeteoClient client = new OpenMeteoClient();
            
            WeatherForecast weatherData = await client.QueryAsync(1.125f, 2.25f);

            Assert.IsNotNull(weatherData);
            Assert.IsNotNull(weatherData.Longitude);
            Assert.IsNotNull(weatherData.Latitude);

            Assert.AreEqual(1.125f, weatherData.Latitude);
            Assert.AreEqual(2.25f, weatherData.Longitude);
        }

        [TestMethod]
        public async Task GeocodingOptions_Test()
        {
            OpenMeteoClient client = new OpenMeteoClient();
            GeocodingOptions options = new GeocodingOptions("Tokyo");
            WeatherForecast weatherData = await client.QueryAsync(options);

            Assert.IsNotNull(weatherData);
            Assert.IsNotNull(weatherData.Longitude);
            Assert.IsNotNull(weatherData.Latitude);
        }

        [TestMethod]
        public async Task WeatherForecast_With_WeatherForecastOptions_Test()
        {
            OpenMeteoClient client = new();
            WeatherForecastOptions weatherForecast = new();

            var res = await client.QueryAsync(weatherForecast);

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
                DateTime.UtcNow.Date.AddDays(-2).ToString("yyyy-MM-dd"),
                DateTime.UtcNow.Date.AddDays(-1).ToString("yyyy-MM-dd"),
                null,
                CellSelectionType.nearest
                );

            var res = await client.QueryAsync("Tokyo", options);

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
                Minutely15 = Minutely15Options.All
            };

            Assert.IsTrue(HourlyOptions.All.Parameter.All(p => options.Hourly.Parameter.Contains(p)));
            Assert.IsTrue(DailyOptions.All.Parameter.All(p => options.Daily.Parameter.Contains(p)));
            Assert.IsTrue(WeatherModelOptions.All.Parameter.All(p => options.Models.Parameter.Contains(p)));
            Assert.IsTrue(CurrentOptions.All.Parameter.All(p => options.Current.Parameter.Contains(p)));
            Assert.IsTrue(Minutely15Options.All.Parameter.All(p => options.Minutely15.Parameter.Contains(p)));
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

            var ex = await Assert.ThrowsExceptionAsync<OpenMeteoClientException>(async () => await client.QueryAsync(options));
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, ex.StatusCode);
            Assert.AreEqual("No data is available for this location", ex.Message);
        }

        [TestMethod]
        [Ignore]
        public async Task MeteoSwissCH1_Null_Daily_Values_Test()
        {
            // Test for a location in Switzerland where MeteoSwiss CH1 model returns null daily values
            OpenMeteoClient client = new()
            {
                RethrowExceptions = true
            };

            var utcDateNow = DateTime.UtcNow.Date;
            var utcDateEnd = utcDateNow.AddDays(1);

            var options = new WeatherForecastOptions()
            {
                Latitude = 46.21f,
                Longitude = 7.31f,
                Daily = new DailyOptions(DailyOptionsParameter.temperature_2m_max),
                Models = new WeatherModelOptions(WeatherModelOptionsParameter.meteoswiss_icon_ch1),

                Temperature_Unit = TemperatureUnitType.celsius,
                Windspeed_Unit = WindspeedUnitType.kmh,
                Precipitation_Unit = PrecipitationUnitType.mm,
                Timeformat = TimeformatType.iso8601,
                Cell_Selection = CellSelectionType.land,
                Timezone = "GMT",

                Start_date = utcDateNow.ToString("yyyy-MM-dd"),
                End_date = utcDateEnd.ToString("yyyy-MM-dd")
            };


            var res = await client.QueryAsync(options);
            Assert.IsNotNull(res);
        }
    }
}
