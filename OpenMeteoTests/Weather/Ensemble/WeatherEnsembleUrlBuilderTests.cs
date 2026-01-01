using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Ensemble;
using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Forecast.Options;

namespace OpenMeteoTests.Weather.Ensemble;

[TestClass]
public class WeatherEnsembleUrlBuilderTests
{
    [TestMethod]
    public void Build_WithOptionsStartDateEndDate_Test()
    {
        var options = GetWeatherEnsembleOptions();
        options.Past_Days = null;
        options.Forecast_Days = null;
        options.Start_date = DateOnly.Parse("2023-01-01");
        options.End_date = DateOnly.Parse("2023-01-02");
        var url = new WeatherEnsembleUrlBuilder()
            .WithOptions(options)
            .Build();

        var expectedUrl = "https://ensemble-api.open-meteo.com/v1/ensemble?latitude=52.52&longitude=13.41&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=Europe%2FBerlin&timeformat=iso8601&start_date=2023-01-01&end_date=2023-01-02&cell_selection=land&hourly=temperature_2m,precipitation&daily=temperature_2m_max,temperature_2m_min&models=icon_seamless,gfs_seamless";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_WithOptionsPastDays_Test()
    {
        var options = GetWeatherEnsembleOptions();
        var url = new WeatherEnsembleUrlBuilder()
            .WithOptions(options)
            .Build();

        var expectedUrl = "https://ensemble-api.open-meteo.com/v1/ensemble?latitude=52.52&longitude=13.41&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=Europe%2FBerlin&timeformat=iso8601&past_days=2&forecast_days=10&cell_selection=land&hourly=temperature_2m,precipitation&daily=temperature_2m_max,temperature_2m_min&models=icon_seamless,gfs_seamless";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_WithOptions_WithApiKey_Test()
    {
        var url = new WeatherEnsembleUrlBuilder("testApiKey")
            .WithOptions(GetWeatherEnsembleOptions())
            .Build();

        var expectedUrl = "https://customer-ensemble-api.open-meteo.com/v1/ensemble?latitude=52.52&longitude=13.41&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=Europe%2FBerlin&timeformat=iso8601&past_days=2&forecast_days=10&cell_selection=land&hourly=temperature_2m,precipitation&daily=temperature_2m_max,temperature_2m_min&models=icon_seamless,gfs_seamless&apikey=testApiKey";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_WithCustomBaseUri_Test()
    {
        var customUri = new Uri("https://custom.example.com");
        var url = new WeatherEnsembleUrlBuilder(customUri)
            .WithOptions(GetWeatherEnsembleOptions())
            .Build();

        var expectedUrl = "https://custom.example.com/v1/ensemble?latitude=52.52&longitude=13.41&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=Europe%2FBerlin&timeformat=iso8601&past_days=2&forecast_days=10&cell_selection=land&hourly=temperature_2m,precipitation&daily=temperature_2m_max,temperature_2m_min&models=icon_seamless,gfs_seamless";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_WithCustomBaseUri_WithApiKey_Test()
    {
        var customUri = new Uri("https://custom.example.com");
        var url = new WeatherEnsembleUrlBuilder(customUri, "testApiKey")
            .WithOptions(GetWeatherEnsembleOptions())
            .Build();

        var expectedUrl = "https://custom.example.com/v1/ensemble?latitude=52.52&longitude=13.41&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=Europe%2FBerlin&timeformat=iso8601&past_days=2&forecast_days=10&cell_selection=land&hourly=temperature_2m,precipitation&daily=temperature_2m_max,temperature_2m_min&models=icon_seamless,gfs_seamless&apikey=testApiKey";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_WithFlatbuffers_Test()
    {
        var url = new WeatherEnsembleUrlBuilder()
            .WithOptions(GetWeatherEnsembleOptions())
            .WithFlatbuffers(true)
            .Build();

        var expectedUrl = "https://ensemble-api.open-meteo.com/v1/ensemble?latitude=52.52&longitude=13.41&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=Europe%2FBerlin&timeformat=iso8601&past_days=2&forecast_days=10&cell_selection=land&hourly=temperature_2m,precipitation&daily=temperature_2m_max,temperature_2m_min&models=icon_seamless,gfs_seamless&format=flatbuffers";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_WithMinimalOptions_Test()
    {
        var options = new WeatherEnsembleOptions
        {
            Latitude = 40.7128f,
            Longitude = -74.006f
        };
        
        var url = new WeatherEnsembleUrlBuilder()
            .WithOptions(options)
            .Build();

        var expectedUrl = "https://ensemble-api.open-meteo.com/v1/ensemble?latitude=40.7128&longitude=-74.006&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=GMT&timeformat=iso8601&cell_selection=land";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_WithHourlyOptionsOnly_Test()
    {
        var options = new WeatherEnsembleOptions(52.52f, 13.41f)
        {
            Hourly = new WeatherEnsembleHourlyOptions([
                WeatherEnsembleHourlyOptionsParameter.temperature_2m,
                WeatherEnsembleHourlyOptionsParameter.windspeed_10m,
                WeatherEnsembleHourlyOptionsParameter.precipitation
            ])
        };

        var url = new WeatherEnsembleUrlBuilder()
            .WithOptions(options)
            .Build();

        var expectedUrl = "https://ensemble-api.open-meteo.com/v1/ensemble?latitude=52.52&longitude=13.41&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=GMT&timeformat=iso8601&cell_selection=land&hourly=temperature_2m,windspeed_10m,precipitation";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_WithDailyOptionsOnly_Test()
    {
        var options = new WeatherEnsembleOptions(52.52f, 13.41f)
        {
            Daily = new WeatherEnsembleDailyOptions([
                WeatherEnsembleDailyOptionsParameter.temperature_2m_max,
                WeatherEnsembleDailyOptionsParameter.temperature_2m_min,
                WeatherEnsembleDailyOptionsParameter.precipitation_sum
            ])
        };

        var url = new WeatherEnsembleUrlBuilder()
            .WithOptions(options)
            .Build();

        var expectedUrl = "https://ensemble-api.open-meteo.com/v1/ensemble?latitude=52.52&longitude=13.41&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=GMT&timeformat=iso8601&cell_selection=land&daily=temperature_2m_max,temperature_2m_min,precipitation_sum";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_WithModelOptionsOnly_Test()
    {
        var options = new WeatherEnsembleOptions(52.52f, 13.41f)
        {
            Models = new EnsembleModelOptions([
                EnsembleModelOptionsParameter.icon_global,
                EnsembleModelOptionsParameter.ecmwf_ifs025
            ])
        };

        var url = new WeatherEnsembleUrlBuilder()
            .WithOptions(options)
            .Build();

        var expectedUrl = "https://ensemble-api.open-meteo.com/v1/ensemble?latitude=52.52&longitude=13.41&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=GMT&timeformat=iso8601&cell_selection=land&models=icon_global,ecmwf_ifs025";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_WithFahrenheitAndMph_Test()
    {
        var options = new WeatherEnsembleOptions
        {
            Latitude = 40.7128f,
            Longitude = -74.006f,
            Temperature_Unit = TemperatureUnitType.fahrenheit,
            Windspeed_Unit = WindspeedUnitType.mph,
            Precipitation_Unit = PrecipitationUnitType.inch
        };

        var url = new WeatherEnsembleUrlBuilder()
            .WithOptions(options)
            .Build();

        var expectedUrl = "https://ensemble-api.open-meteo.com/v1/ensemble?latitude=40.7128&longitude=-74.006&temperature_unit=fahrenheit&windspeed_unit=mph&precipitation_unit=inch&timezone=GMT&timeformat=iso8601&cell_selection=land";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_WithAllEnsembleModels_Test()
    {
        var options = new WeatherEnsembleOptions(52.52f, 13.41f)
        {
            Models = new EnsembleModelOptions([
                EnsembleModelOptionsParameter.icon_seamless,
                EnsembleModelOptionsParameter.icon_global,
                EnsembleModelOptionsParameter.icon_eu,
                EnsembleModelOptionsParameter.icon_d2,
                EnsembleModelOptionsParameter.gfs_seamless,
                EnsembleModelOptionsParameter.gfs025,
                EnsembleModelOptionsParameter.ecmwf_ifs025
            ])
        };

        var url = new WeatherEnsembleUrlBuilder()
            .WithOptions(options)
            .Build();

        Assert.IsTrue(url.Contains("models=icon_seamless,icon_global,icon_eu,icon_d2,gfs_seamless,gfs025,ecmwf_ifs025"));
    }

    [TestMethod]
    public void Build_WithPressureLevelVariables_Test()
    {
        var options = new WeatherEnsembleOptions(52.52f, 13.41f)
        {
            Hourly = new WeatherEnsembleHourlyOptions([
                WeatherEnsembleHourlyOptionsParameter.temperature_850hPa,
                WeatherEnsembleHourlyOptionsParameter.geopotential_height_500hPa,
                WeatherEnsembleHourlyOptionsParameter.windspeed_250hPa
            ])
        };

        var url = new WeatherEnsembleUrlBuilder()
            .WithOptions(options)
            .Build();

        Assert.IsTrue(url.Contains("hourly=temperature_850hPa,geopotential_height_500hPa,windspeed_250hPa"));
    }

    private static WeatherEnsembleOptions GetWeatherEnsembleOptions() => new()
    {
        Latitude = 52.52f,
        Longitude = 13.41f,
        Temperature_Unit = TemperatureUnitType.celsius,
        Windspeed_Unit = WindspeedUnitType.kmh,
        Precipitation_Unit = PrecipitationUnitType.mm,
        Timezone = "Europe/Berlin",
        Timeformat = TimeformatType.iso8601,
        Past_Days = 2,
        Forecast_Days = 10,
        Cell_Selection = CellSelectionType.land,
        Hourly = new WeatherEnsembleHourlyOptions([
            WeatherEnsembleHourlyOptionsParameter.temperature_2m,
            WeatherEnsembleHourlyOptionsParameter.precipitation
        ]),
        Daily = new WeatherEnsembleDailyOptions([
            WeatherEnsembleDailyOptionsParameter.temperature_2m_max,
            WeatherEnsembleDailyOptionsParameter.temperature_2m_min
        ]),
        Models = new EnsembleModelOptions([
            EnsembleModelOptionsParameter.icon_seamless,
            EnsembleModelOptionsParameter.gfs_seamless
        ])
    };
}
