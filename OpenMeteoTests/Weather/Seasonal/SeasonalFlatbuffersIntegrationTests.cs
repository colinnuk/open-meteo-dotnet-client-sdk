using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;
using OpenMeteo.Weather.Seasonal.Options;
using OpenMeteo.Weather.Seasonal.ResponseModel;

namespace OpenMeteoTests.Weather.Seasonal;

/// <summary>
/// Requests the same data as JSON and as FlatBuffers and checks that both produce the same result.
/// </summary>
[TestClass]
[TestCategory(TestCategoryConstants.Integration)]
public class SeasonalFlatbuffersIntegrationTests
{
    private static async Task<(WeatherSeasonal json, WeatherSeasonal fb)> QueryBothAsync(SeasonalOptions options)
    {
        var json = await new OpenMeteoClient { RethrowExceptions = true, UseFlatbuffers = false }.QuerySeasonalApiAsync(options);
        var fb = await new OpenMeteoClient { RethrowExceptions = true, UseFlatbuffers = true }.QuerySeasonalApiAsync(options);
        Assert.IsNotNull(json);
        Assert.IsNotNull(fb);
        return (json, fb);
    }

    private static void AssertSeriesEqual(SeasonalSeries? expected, SeasonalSeries? actual)
    {
        Assert.IsNotNull(expected);
        Assert.IsNotNull(actual);

        CollectionAssert.AreEqual(expected.Time, actual.Time, "time");
        var missing = expected.Variables.Keys.Except(actual.Variables.Keys).ToList();
        var unexpected = actual.Variables.Keys.Except(expected.Variables.Keys).ToList();
        Assert.IsTrue(missing.Count == 0 && unexpected.Count == 0,
            $"variable names differ. Missing: [{string.Join(", ", missing)}] Unexpected: [{string.Join(", ", unexpected)}]");

        foreach (var (name, expectedValues) in expected.Variables)
        {
            var actualValues = actual.Variables[name];
            Assert.AreEqual(expectedValues.Length, actualValues.Length, $"{name} length");
            for (int i = 0; i < expectedValues.Length; i++)
            {
                if (expectedValues[i] == null || actualValues[i] == null)
                    Assert.AreEqual(expectedValues[i], actualValues[i], $"{name}[{i}]");
                else // the JSON format rounds to 0 to 2 decimals depending on the variable, the binary format does not
                    Assert.AreEqual(expectedValues[i]!.Value, actualValues[i]!.Value, Math.Abs(expectedValues[i]!.Value) * 0.001 + 0.51, $"{name}[{i}]");
            }
            Assert.AreEqual(expected.Units[name], actual.Units[name], $"{name} unit");
        }
    }

    private static void AssertMetadataEqual(WeatherSeasonal expected, WeatherSeasonal actual)
    {
        Assert.AreEqual(expected.Latitude, actual.Latitude, 0.001);
        Assert.AreEqual(expected.Longitude, actual.Longitude, 0.001);
        Assert.AreEqual(expected.Elevation, actual.Elevation);
        Assert.AreEqual(expected.UtcOffset, actual.UtcOffset);
        Assert.AreEqual(expected.Timezone, actual.Timezone);
    }

    [TestMethod]
    public async Task Hourly_AllVariables_MatchJson_Async_Test()
    {
        var options = new SeasonalOptions(52.52f, 13.41f)
        {
            Hourly = SeasonalVariableOptions<SeasonalHourlyOptionsParameter>.All,
            Models = [SeasonalModelOptionsParameter.ecmwf_seas5_ensemble_mean],
            Forecast_Days = 20
        };

        var (json, fb) = await QueryBothAsync(options);

        AssertMetadataEqual(json, fb);
        AssertSeriesEqual(json.Hourly, fb.Hourly);
    }

    [TestMethod]
    public async Task Daily_AllVariables_MatchJson_Async_Test()
    {
        var options = new SeasonalOptions(52.52f, 13.41f)
        {
            Daily = SeasonalVariableOptions<SeasonalDailyOptionsParameter>.All,
            Models = [SeasonalModelOptionsParameter.ecmwf_seas5_ensemble_mean],
            Forecast_Days = 200
        };

        var (json, fb) = await QueryBothAsync(options);

        AssertSeriesEqual(json.Daily, fb.Daily);
    }

    [TestMethod]
    public async Task Daily_EnsembleMembers_MatchJson_Async_Test()
    {
        var options = new SeasonalOptions(52.52f, 13.41f)
        {
            Daily = [SeasonalDailyOptionsParameter.temperature_2m_max, SeasonalDailyOptionsParameter.precipitation_sum],
            Forecast_Days = 30
        };

        var (json, fb) = await QueryBothAsync(options);

        AssertSeriesEqual(json.Daily, fb.Daily);
        Assert.IsTrue(fb.Daily!.GetMembers("temperature_2m_max").Count > 1);
    }

    [TestMethod]
    public async Task Monthly_AllVariables_MatchJson_Async_Test()
    {
        var options = new SeasonalOptions(52.52f, 13.41f)
        {
            Monthly = SeasonalVariableOptions<SeasonalMonthlyOptionsParameter>.All
        };

        var (json, fb) = await QueryBothAsync(options);

        AssertSeriesEqual(json.Monthly, fb.Monthly);
    }

    [TestMethod]
    public async Task Monthly_AllAnomalies_MatchJson_Async_Test()
    {
        var options = new SeasonalOptions(52.52f, 13.41f)
        {
            Monthly =
            [
                SeasonalMonthlyOptionsParameter.temperature_2m_anomaly,
                SeasonalMonthlyOptionsParameter.precipitation_anomaly,
                SeasonalMonthlyOptionsParameter.snowfall_anomaly,
                SeasonalMonthlyOptionsParameter.cloud_cover_anomaly,
                SeasonalMonthlyOptionsParameter.sunshine_duration_anomaly
            ]
        };

        var (json, fb) = await QueryBothAsync(options);

        AssertSeriesEqual(json.Monthly, fb.Monthly);
        foreach (var name in new[] { "temperature_2m_anomaly", "precipitation_anomaly", "snowfall_anomaly", "cloud_cover_anomaly", "sunshine_duration_anomaly" })
            Assert.IsTrue(fb.Monthly!.Variables.TryGetValue(name, out var values) && values.Length > 0, name);
    }

    [TestMethod]
    public async Task Weekly_AllAnomalies_Async_Test()
    {
        var options = new SeasonalOptions(52.52f, 13.41f)
        {
            Weekly =
            [
                SeasonalWeeklyOptionsParameter.temperature_2m_anomaly,
                SeasonalWeeklyOptionsParameter.precipitation_anomaly,
                SeasonalWeeklyOptionsParameter.snowfall_anomaly,
                SeasonalWeeklyOptionsParameter.cloud_cover_anomaly,
                SeasonalWeeklyOptionsParameter.sunshine_duration_anomaly
            ]
        };

        var (json, fb) = await QueryBothAsync(options);

        AssertSeriesEqual(json.Weekly, fb.Weekly);
        foreach (var name in new[] { "temperature_2m_anomaly", "precipitation_anomaly", "snowfall_anomaly", "cloud_cover_anomaly", "sunshine_duration_anomaly" })
            Assert.IsTrue(fb.Weekly!.Variables.TryGetValue(name, out var values) && values.Length > 0, name);
    }

    [TestMethod]
    public async Task Daily_WithTimezoneAcrossDst_MatchJson_Async_Test()
    {
        var options = new SeasonalOptions(52.52f, 13.41f)
        {
            Daily = [SeasonalDailyOptionsParameter.temperature_2m_max],
            Hourly = [SeasonalHourlyOptionsParameter.temperature_2m],
            Models = [SeasonalModelOptionsParameter.ecmwf_seas5_ensemble_mean],
            Timezone = "Europe/Berlin",
            Forecast_Days = 150
        };

        var (json, fb) = await QueryBothAsync(options);

        AssertMetadataEqual(json, fb);
        AssertSeriesEqual(json.Daily, fb.Daily);
        AssertSeriesEqual(json.Hourly, fb.Hourly);
    }

    [TestMethod]
    public async Task Units_MatchJson_Async_Test()
    {
        var options = new SeasonalOptions(52.52f, 13.41f)
        {
            Daily = [SeasonalDailyOptionsParameter.temperature_2m_max, SeasonalDailyOptionsParameter.precipitation_sum, SeasonalDailyOptionsParameter.wind_speed_10m_max],
            Models = [SeasonalModelOptionsParameter.ecmwf_seas5_ensemble_mean],
            Temperature_Unit = OpenMeteo.Weather.Forecast.Options.TemperatureUnitType.fahrenheit,
            Windspeed_Unit = OpenMeteo.Weather.Forecast.Options.WindspeedUnitType.mph,
            Precipitation_Unit = OpenMeteo.Weather.Forecast.Options.PrecipitationUnitType.inch,
            Forecast_Days = 5
        };

        var (json, fb) = await QueryBothAsync(options);

        AssertSeriesEqual(json.Daily, fb.Daily);
    }

    [TestMethod]
    public async Task Weekly_FallsBackToJson_Async_Test()
    {
        var options = new SeasonalOptions(52.52f, 13.41f)
        {
            Weekly = [SeasonalWeeklyOptionsParameter.temperature_2m_mean, SeasonalWeeklyOptionsParameter.temperature_2m_anomaly]
        };

        var (json, fb) = await QueryBothAsync(options);

        AssertSeriesEqual(json.Weekly, fb.Weekly);
    }

    [TestMethod]
    public async Task MixedWeeklyAndDaily_WithFlatbuffers_Async_Test()
    {
        var options = new SeasonalOptions(52.52f, 13.41f)
        {
            Weekly = [SeasonalWeeklyOptionsParameter.temperature_2m_mean],
            Daily = [SeasonalDailyOptionsParameter.temperature_2m_max],
            Forecast_Days = 5
        };

        var (json, fb) = await QueryBothAsync(options);

        AssertSeriesEqual(json.Daily, fb.Daily);
        AssertSeriesEqual(json.Weekly, fb.Weekly);
    }
}
