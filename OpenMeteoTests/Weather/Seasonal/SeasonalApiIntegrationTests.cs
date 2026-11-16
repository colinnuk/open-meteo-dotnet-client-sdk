using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;
using OpenMeteo.Weather.Seasonal.Options;

namespace OpenMeteoTests.Weather.Seasonal;

[TestClass]
[TestCategory(TestCategoryConstants.Integration)]
public class SeasonalApiIntegrationTests
{
    [TestMethod]
    public async Task QuerySeasonalApi_DailyWithMembers_Async_Test()
    {
        OpenMeteoClient client = new() { RethrowExceptions = true };
        var options = new SeasonalOptions(52.52f, 13.41f)
        {
            Daily = [SeasonalDailyOptionsParameter.temperature_2m_max],
            Forecast_Days = 10
        };

        var res = await client.QuerySeasonalApiAsync(options);

        Assert.IsNotNull(res?.Daily);
        Assert.AreEqual(10, res.Daily.Time.Length);
        Assert.IsNotNull(res.Daily.Get("temperature_2m_max"));
        Assert.IsTrue(res.Daily.GetMembers("temperature_2m_max").Count > 1);
    }

    [TestMethod]
    public async Task QuerySeasonalApi_WeeklyAndMonthly_Async_Test()
    {
        OpenMeteoClient client = new() { RethrowExceptions = true };
        var options = new SeasonalOptions(52.52f, 13.41f)
        {
            Weekly = [SeasonalWeeklyOptionsParameter.temperature_2m_mean, SeasonalWeeklyOptionsParameter.temperature_2m_anomaly],
            Monthly = [SeasonalMonthlyOptionsParameter.temperature_2m_anomaly]
        };

        var res = await client.QuerySeasonalApiAsync(options);

        Assert.IsNotNull(res?.Weekly);
        Assert.IsNotNull(res.Monthly);
        Assert.IsTrue(res.Weekly.Time.Length > 0);
        Assert.AreEqual("K", res.Monthly.Units["temperature_2m_anomaly"]);
    }

    [TestMethod]
    public async Task QuerySeasonalApi_AllAnomalies_Async_Test()
    {
        OpenMeteoClient client = new() { RethrowExceptions = true };
        var options = new SeasonalOptions(52.52f, 13.41f)
        {
            Weekly =
            [
                SeasonalWeeklyOptionsParameter.temperature_2m_anomaly,
                SeasonalWeeklyOptionsParameter.precipitation_anomaly,
                SeasonalWeeklyOptionsParameter.snowfall_anomaly,
                SeasonalWeeklyOptionsParameter.cloud_cover_anomaly,
                SeasonalWeeklyOptionsParameter.sunshine_duration_anomaly
            ],
            Monthly =
            [
                SeasonalMonthlyOptionsParameter.temperature_2m_anomaly,
                SeasonalMonthlyOptionsParameter.precipitation_anomaly,
                SeasonalMonthlyOptionsParameter.snowfall_anomaly,
                SeasonalMonthlyOptionsParameter.cloud_cover_anomaly,
                SeasonalMonthlyOptionsParameter.sunshine_duration_anomaly
            ]
        };

        var res = await client.QuerySeasonalApiAsync(options);

        Assert.IsNotNull(res?.Weekly);
        Assert.IsNotNull(res.Monthly);
        foreach (var name in new[] { "temperature_2m_anomaly", "precipitation_anomaly", "snowfall_anomaly", "cloud_cover_anomaly", "sunshine_duration_anomaly" })
        {
            Assert.IsTrue(res.Weekly.Variables.TryGetValue(name, out var weekly) && weekly.Length > 0, $"weekly {name}");
            Assert.IsTrue(res.Monthly.Variables.TryGetValue(name, out var monthly) && monthly.Length > 0, $"monthly {name}");
        }
    }

    [TestMethod]
    public async Task QuerySeasonalApi_EnsembleMeanModel_Async_Test()
    {
        OpenMeteoClient client = new() { RethrowExceptions = true };
        var options = new SeasonalOptions(52.52f, 13.41f)
        {
            Hourly = [SeasonalHourlyOptionsParameter.temperature_2m],
            Models = [SeasonalModelOptionsParameter.ecmwf_seas5_ensemble_mean],
            Forecast_Days = 2
        };

        var res = await client.QuerySeasonalApiAsync(options);

        Assert.IsNotNull(res?.Hourly);
        Assert.AreEqual(8, res.Hourly.Time.Length); // 6-hourly resolution
        Assert.AreEqual(0, res.Hourly.GetMembers("temperature_2m").Count);
    }
}
