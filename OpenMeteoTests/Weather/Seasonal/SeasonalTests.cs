using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Seasonal;
using OpenMeteo.Weather.Seasonal.Options;
using OpenMeteo.Weather.Seasonal.ResponseModel;

namespace OpenMeteoTests.Weather.Seasonal;

[TestClass]
public class SeasonalUrlBuilderTests
{
    private static SeasonalOptions GetOptions() => new(52.52f, 13.41f)
    {
        Daily = [SeasonalDailyOptionsParameter.temperature_2m_max, SeasonalDailyOptionsParameter.precipitation_sum],
        Models = [SeasonalModelOptionsParameter.ecmwf_seas5],
        Forecast_Days = 30
    };

    [TestMethod]
    public void Build_WithOptions_Test()
    {
        var url = new SeasonalUrlBuilder().WithOptions(GetOptions()).Build();

        Assert.AreEqual(
            "https://seasonal-api.open-meteo.com/v1/seasonal?latitude=52.52&longitude=13.41&temperature_unit=celsius&windspeed_unit=kmh&precipitation_unit=mm&timezone=GMT&timeformat=iso8601&forecast_days=30&cell_selection=land&daily=temperature_2m_max,precipitation_sum&models=ecmwf_seas5",
            url);
    }

    [TestMethod]
    public void Build_WithApiKey_UsesCustomerSubdomain_Test()
    {
        var url = new SeasonalUrlBuilder("testApiKey").WithOptions(GetOptions()).Build();

        StringAssert.StartsWith(url, "https://customer-seasonal-api.open-meteo.com/v1/seasonal?");
        StringAssert.EndsWith(url, "&apikey=testApiKey");
    }

    [TestMethod]
    public void Build_WithCustomBaseUri_Test()
    {
        var url = new SeasonalUrlBuilder(new Uri("https://custom.example.com")).WithOptions(GetOptions()).Build();

        StringAssert.StartsWith(url, "https://custom.example.com/v1/seasonal?");
    }

    [TestMethod]
    public void Build_WithWeeklyAndMonthly_Test()
    {
        var options = new SeasonalOptions(52.52f, 13.41f)
        {
            Weekly = [SeasonalWeeklyOptionsParameter.temperature_2m_anomaly],
            Monthly = [SeasonalMonthlyOptionsParameter.precipitation_mean],
            Hourly = [SeasonalHourlyOptionsParameter.temperature_2m]
        };

        var url = new SeasonalUrlBuilder().WithOptions(options).Build();

        StringAssert.Contains(url, "weekly=temperature_2m_anomaly");
        StringAssert.Contains(url, "monthly=precipitation_mean");
        StringAssert.Contains(url, "hourly=temperature_2m");
        Assert.IsFalse(url.Contains("models="));
    }

    [TestMethod]
    public void Build_WithoutVariables_Throws_Test()
    {
        Assert.Throws<ArgumentException>(() => new SeasonalUrlBuilder().WithOptions(new SeasonalOptions(1f, 2f)));
    }

    [TestMethod]
    public void VariableOptions_IgnoresDuplicates_Test()
    {
        var options = new SeasonalVariableOptions<SeasonalDailyOptionsParameter>
        {
            SeasonalDailyOptionsParameter.temperature_2m_max,
            SeasonalDailyOptionsParameter.temperature_2m_max
        };

        Assert.AreEqual(1, options.Count);
        Assert.AreEqual(Enum.GetValues<SeasonalDailyOptionsParameter>().Length, SeasonalVariableOptions<SeasonalDailyOptionsParameter>.All.Count);
    }
}

[TestClass]
public class SeasonalResponseParserTests
{
    private const string Json = """
    {
      "latitude": 52.41, "longitude": 13.69, "generationtime_ms": 0.5, "utc_offset_seconds": 0,
      "timezone": "GMT", "timezone_abbreviation": "GMT", "elevation": 38.0,
      "daily_units": { "time": "iso8601", "temperature_2m_max": "°C", "temperature_2m_max_member01": "°C", "temperature_2m_max_member02": "°C" },
      "daily": {
        "time": ["2026-11-15", "2026-11-16"],
        "temperature_2m_max": [8.9, 8.5],
        "temperature_2m_max_member01": [9.1, null],
        "temperature_2m_max_member02": [8.7, 8.0]
      },
      "monthly_units": { "time": "iso8601", "temperature_2m_anomaly": "K" },
      "monthly": { "time": ["2026-11-01"], "temperature_2m_anomaly": [0.2] }
    }
    """;

    [TestMethod]
    public void Parse_ReadsMetadataAndSeries_Test()
    {
        var result = new SeasonalResponseParser().Parse(Json);

        Assert.IsNotNull(result);
        Assert.AreEqual(38f, result.Elevation);
        Assert.AreEqual("GMT", result.Timezone);
        Assert.IsNull(result.Hourly);
        Assert.IsNull(result.Weekly);

        Assert.IsNotNull(result.Daily);
        CollectionAssert.AreEqual(new[] { new DateTime(2026, 11, 15), new DateTime(2026, 11, 16) }, result.Daily.Time);
        CollectionAssert.AreEqual(new float?[] { 8.9f, 8.5f }, result.Daily.Get("temperature_2m_max"));
        Assert.AreEqual("°C", result.Daily.Units["temperature_2m_max"]);
        Assert.IsFalse(result.Daily.Units.ContainsKey("time"));

        Assert.IsNotNull(result.Monthly);
        Assert.AreEqual(0.2f, result.Monthly.Get("temperature_2m_anomaly")![0]);
    }

    [TestMethod]
    public void GetMembers_ReturnsOrderedMembersWithNulls_Test()
    {
        var daily = new SeasonalResponseParser().Parse(Json)!.Daily!;

        var members = daily.GetMembers("temperature_2m_max");

        Assert.AreEqual(2, members.Count);
        Assert.IsNull(members[0][1]);
        Assert.AreEqual(8.7f, members[1][0]);
        Assert.AreEqual(0, daily.GetMembers("precipitation_sum").Count);
        Assert.IsNull(daily.Get("precipitation_sum"));
    }

    [TestMethod]
    public void Parse_UnixTime_Test()
    {
        const string json = """{ "daily": { "time": [1794697200] , "temperature_2m_max": [1.5] } }""";

        var daily = new SeasonalResponseParser().Parse(json)!.Daily!;

        Assert.AreEqual(DateTimeOffset.FromUnixTimeSeconds(1794697200).UtcDateTime, daily.Time.Single());
    }
}
