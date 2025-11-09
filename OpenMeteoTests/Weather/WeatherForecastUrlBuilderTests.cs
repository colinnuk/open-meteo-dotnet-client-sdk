using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather;
using OpenMeteo.Weather.Options;

namespace OpenMeteoTests.Weather
{
    [TestClass]
    public class WeatherForecastUrlBuilderTests
    {
        [TestMethod]
        public void Build_WithOptionsStartDateEndDate_Test()
        {
            var options = GetWeatherForecastOptions();
            options.Past_Days = null;
            options.Start_date = DateOnly.Parse("2023-01-01");
            options.End_date = DateOnly.Parse("2023-01-02");
            var url = new WeatherForecastUrlBuilder()
                .WithOptions(options)
                .Build();

            var expectedUrl = "https://api.open-meteo.com/v1/forecast?latitude=40.7128&longitude=-74.006&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=America%2FNew_York&timeformat=iso8601&start_date=2023-01-01&end_date=2023-01-02&cell_selection=nearest&hourly=temperature_2m,windspeed_10m&daily=temperature_2m_max,temperature_2m_min&models=gfs_hrrr,gfs_global&current=temperature_2m&minutely_15=precipitation";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithOptionsPastDays_Test()
        {
            var options = GetWeatherForecastOptions();
            var url = new WeatherForecastUrlBuilder()
                .WithOptions(options)
                .Build();

            var expectedUrl = "https://api.open-meteo.com/v1/forecast?latitude=40.7128&longitude=-74.006&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=America%2FNew_York&timeformat=iso8601&past_days=3&cell_selection=nearest&hourly=temperature_2m,windspeed_10m&daily=temperature_2m_max,temperature_2m_min&models=gfs_hrrr,gfs_global&current=temperature_2m&minutely_15=precipitation";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithOptions_WithApiKey_Test()
        {
            var url = new WeatherForecastUrlBuilder("testApiKey")
                .WithOptions(GetWeatherForecastOptions())
                .Build();

            var expectedUrl = "https://customer-api.open-meteo.com/v1/forecast?latitude=40.7128&longitude=-74.006&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=America%2FNew_York&timeformat=iso8601&past_days=3&cell_selection=nearest&hourly=temperature_2m,windspeed_10m&daily=temperature_2m_max,temperature_2m_min&models=gfs_hrrr,gfs_global&current=temperature_2m&minutely_15=precipitation&apikey=testApiKey";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithCustomBaseUri_Test()
        {
            var customUri = new Uri("https://custom.example.com");
            var url = new WeatherForecastUrlBuilder(customUri)
                .WithOptions(GetWeatherForecastOptions())
                .Build();

            var expectedUrl = "https://custom.example.com/v1/forecast?latitude=40.7128&longitude=-74.006&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=America%2FNew_York&timeformat=iso8601&past_days=3&cell_selection=nearest&hourly=temperature_2m,windspeed_10m&daily=temperature_2m_max,temperature_2m_min&models=gfs_hrrr,gfs_global&current=temperature_2m&minutely_15=precipitation";
            Assert.AreEqual(expectedUrl, url);
        }

        [TestMethod]
        public void Build_WithCustomBaseUri_WithApiKey_Test()
        {
            var customUri = new Uri("https://custom.example.com");
            var url = new WeatherForecastUrlBuilder(customUri, "testApiKey")
                .WithOptions(GetWeatherForecastOptions())
                .Build();

            var expectedUrl = "https://custom.example.com/v1/forecast?latitude=40.7128&longitude=-74.006&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=America%2FNew_York&timeformat=iso8601&past_days=3&cell_selection=nearest&hourly=temperature_2m,windspeed_10m&daily=temperature_2m_max,temperature_2m_min&models=gfs_hrrr,gfs_global&current=temperature_2m&minutely_15=precipitation&apikey=testApiKey";
            Assert.AreEqual(expectedUrl, url);
        }

        private static WeatherForecastOptions GetWeatherForecastOptions() => new()
        {
            Latitude = 40.7128f,
            Longitude = -74.006f,
            Temperature_Unit = TemperatureUnitType.celsius,
            Windspeed_Unit = WindspeedUnitType.kmh,
            Precipitation_Unit = PrecipitationUnitType.mm,
            Timezone = "America/New_York",
            Timeformat = TimeformatType.iso8601,
            Past_Days = 3,
            Hourly = new HourlyOptions([HourlyOptionsParameter.temperature_2m, HourlyOptionsParameter.windspeed_10m]),
            Daily = new DailyOptions([DailyOptionsParameter.temperature_2m_max, DailyOptionsParameter.temperature_2m_min]),
            Cell_Selection = CellSelectionType.nearest,
            Models = new WeatherModelOptions([WeatherModelOptionsParameter.gfs_hrrr, WeatherModelOptionsParameter.gfs_global]),
            Current = new CurrentOptions([CurrentOptionsParameter.temperature_2m]),
            Minutely_15 = new Minutely15Options([Minutely15OptionsParameter.precipitation])
        };
    }
}
