using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Ensemble.ResponseModel;
using System.Text.Json;

namespace OpenMeteoTests.Weather.Ensemble;

[TestClass]
public class EnsemblePublicApiTests
{
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    [TestMethod]
    public async Task DeserializeJson_PopulatesMemberDictionaries()
    {
        var json = @"{
            ""latitude"": 50.0,
            ""longitude"": -123.0,
            ""generationtime_ms"": 0.349,
            ""utc_offset_seconds"": 0,
            ""timezone"": ""GMT"",
            ""timezone_abbreviation"": ""GMT"",
            ""elevation"": 1643.0,
            ""hourly"": {
                ""time"": [""2025-12-14T00:00"", ""2025-12-14T01:00""],
                ""temperature_2m"": [1.0, 1.1],
                ""temperature_2m_member01"": [0.9, 0.6],
                ""temperature_2m_member02"": [0.8, 0.7],
                ""precipitation"": [0.0, 0.1],
                ""precipitation_member01"": [0.1, 0.2],
                ""windspeed_10m"": [5.0, 5.5],
                ""windspeed_10m_member01"": [5.5, 6.0]
            }
        }";

        var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
        var options = new WeatherEnsembleOptions { Timezone = "GMT" };
        var parser = new WeatherEnsembleResponseParser(_jsonOptions);

        var result = await parser.DeserializeJsonAsync(response, options);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Hourly);
        
        // Verify Temperature_2m members (should have member 0, 1, and 2)
        Assert.IsNotNull(result.Hourly.Temperature_2m);
        Assert.AreEqual(3, result.Hourly.Temperature_2m.Count, "Should have 3 members (0, 1, 2)");
        Assert.IsTrue(result.Hourly.Temperature_2m.ContainsKey(0), "Should contain member 0");
        Assert.IsTrue(result.Hourly.Temperature_2m.ContainsKey(1), "Should contain member 1");
        Assert.IsTrue(result.Hourly.Temperature_2m.ContainsKey(2), "Should contain member 2");
        Assert.AreEqual(1.0f, result.Hourly.Temperature_2m[0]![0], "Member 0 first value");
        Assert.AreEqual(1.1f, result.Hourly.Temperature_2m[0]![1], "Member 0 second value");
        Assert.AreEqual(0.9f, result.Hourly.Temperature_2m[1]![0], "Member 1 first value");
        Assert.AreEqual(0.6f, result.Hourly.Temperature_2m[1]![1], "Member 1 second value");
        Assert.AreEqual(0.8f, result.Hourly.Temperature_2m[2]![0], "Member 2 first value");
        
        // Verify Precipitation members (should have member 0 and 1)
        Assert.IsNotNull(result.Hourly.Precipitation);
        Assert.AreEqual(2, result.Hourly.Precipitation.Count, "Should have 2 precipitation members (0, 1)");
        Assert.IsTrue(result.Hourly.Precipitation.ContainsKey(0), "Should contain precipitation member 0");
        Assert.AreEqual(0.0f, result.Hourly.Precipitation[0]![0], "Precipitation member 0 first value");
        Assert.AreEqual(0.1f, result.Hourly.Precipitation[1]![0], "Precipitation member 1 first value");
        
        // Verify Windspeed_10m members (should have member 0 and 1)
        Assert.IsNotNull(result.Hourly.Windspeed_10m);
        Assert.AreEqual(2, result.Hourly.Windspeed_10m.Count, "Should have 2 windspeed members (0, 1)");
        Assert.IsTrue(result.Hourly.Windspeed_10m.ContainsKey(0), "Should contain windspeed member 0");
        Assert.AreEqual(5.0f, result.Hourly.Windspeed_10m[0]![0], "Windspeed member 0 first value");
        Assert.AreEqual(5.5f, result.Hourly.Windspeed_10m[1]![0], "Windspeed member 1 first value");
    }

    [TestMethod]
    public async Task DeserializeJson_Daily_PopulatesMemberDictionaries()
    {
        var json = @"{
            ""latitude"": 52.52,
            ""longitude"": 13.41,
            ""generationtime_ms"": 0.5,
            ""utc_offset_seconds"": 3600,
            ""timezone"": ""Europe/Berlin"",
            ""timezone_abbreviation"": ""CET"",
            ""elevation"": 38.0,
            ""daily"": {
                ""time"": [""2025-12-14"", ""2025-12-15""],
                ""temperature_2m_max"": [5.5, 6.2],
                ""temperature_2m_max_member01"": [5.3, 6.0],
                ""temperature_2m_max_member02"": [5.1, 5.9],
                ""precipitation_sum"": [1.0, 0.5],
                ""precipitation_sum_member01"": [1.2, 0.8],
                ""cloudcover_mean"": [70, 75],
                ""cloudcover_mean_member01"": [75, 80],
                ""winddirection_10m_dominant"": [170, 180],
                ""winddirection_10m_dominant_member01"": [180, 190]
            }
        }";

        var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
        var options = new WeatherEnsembleOptions { Timezone = "Europe/Berlin" };
        var parser = new WeatherEnsembleResponseParser(_jsonOptions);

        var result = await parser.DeserializeJsonAsync(response, options);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Daily);
        Assert.IsNotNull(result.Daily.Time);
        Assert.AreEqual(2, result.Daily.Time.Length);
        Assert.AreEqual(new DateOnly(2025, 12, 14), result.Daily.Time[0]);
        Assert.AreEqual(new DateOnly(2025, 12, 15), result.Daily.Time[1]);
        
        // Verify Temperature_2m_max members (should have 0, 1, 2)
        Assert.IsNotNull(result.Daily.Temperature_2m_max);
        Assert.AreEqual(3, result.Daily.Temperature_2m_max.Count, "Should have 3 members (0, 1, 2)");
        Assert.IsTrue(result.Daily.Temperature_2m_max.ContainsKey(0), "Should contain member 0");
        Assert.AreEqual(5.5f, result.Daily.Temperature_2m_max[0]![0], "Member 0 first value");
        Assert.AreEqual(6.2f, result.Daily.Temperature_2m_max[0]![1], "Member 0 second value");
        Assert.AreEqual(5.3f, result.Daily.Temperature_2m_max[1]![0], "Member 1 first value");
        Assert.AreEqual(6.0f, result.Daily.Temperature_2m_max[1]![1], "Member 1 second value");
        Assert.AreEqual(5.1f, result.Daily.Temperature_2m_max[2]![0], "Member 2 first value");
        
        // Verify Cloudcover_mean members (int) - should have 0 and 1
        Assert.IsNotNull(result.Daily.Cloudcover_mean);
        Assert.AreEqual(2, result.Daily.Cloudcover_mean.Count, "Should have 2 cloudcover members");
        Assert.IsTrue(result.Daily.Cloudcover_mean.ContainsKey(0), "Should contain cloudcover member 0");
        Assert.AreEqual(70, result.Daily.Cloudcover_mean[0]![0], "Member 0 cloudcover");
        Assert.AreEqual(75, result.Daily.Cloudcover_mean[1]![0], "Member 1 cloudcover");
        Assert.AreEqual(80, result.Daily.Cloudcover_mean[1]![1], "Member 1 cloudcover second value");
        
        // Verify Winddirection_10m_dominant members (int) - should have 0 and 1
        Assert.IsNotNull(result.Daily.Winddirection_10m_dominant);
        Assert.AreEqual(2, result.Daily.Winddirection_10m_dominant.Count, "Should have 2 winddirection members");
        Assert.IsTrue(result.Daily.Winddirection_10m_dominant.ContainsKey(0), "Should contain winddirection member 0");
        Assert.AreEqual(170, result.Daily.Winddirection_10m_dominant[0]![0], "Member 0 wind direction");
        Assert.AreEqual(180, result.Daily.Winddirection_10m_dominant[1]![0], "Member 1 wind direction");
    }

    [TestMethod]
    public async Task EasyAccessToSpecificMembers()
    {
        var json = @"{
            ""latitude"": 50.0,
            ""longitude"": -123.0,
            ""hourly"": {
                ""time"": [""2025-12-14T00:00""],
                ""temperature_2m"": [1.1],
                ""temperature_2m_member01"": [0.9],
                ""temperature_2m_member05"": [1.2],
                ""temperature_2m_member10"": [0.7]
            }
        }";

        var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
        var options = new WeatherEnsembleOptions();
        var parser = new WeatherEnsembleResponseParser(_jsonOptions);

        var result = await parser.DeserializeJsonAsync(response, options);

        Assert.IsNotNull(result?.Hourly?.Temperature_2m);
        
        // Can access specific members directly including member 0
        var member0Forecast = result.Hourly.Temperature_2m[0];
        Assert.IsNotNull(member0Forecast);
        Assert.AreEqual(1.1f, member0Forecast[0]);
        
        var member5Forecast = result.Hourly.Temperature_2m[5];
        Assert.IsNotNull(member5Forecast);
        Assert.AreEqual(1.2f, member5Forecast[0]);
        
        // Can check if member exists
        Assert.IsTrue(result.Hourly.Temperature_2m.ContainsKey(0));
        Assert.IsTrue(result.Hourly.Temperature_2m.ContainsKey(1));
        Assert.IsTrue(result.Hourly.Temperature_2m.ContainsKey(5));
        Assert.IsTrue(result.Hourly.Temperature_2m.ContainsKey(10));
        Assert.IsFalse(result.Hourly.Temperature_2m.ContainsKey(3));
    }

    [TestMethod]
    public async Task CanIterateOverAllMembers()
    {
        var json = @"{
            ""latitude"": 50.0,
            ""longitude"": -123.0,
            ""hourly"": {
                ""time"": [""2025-12-14T00:00""],
                ""temperature_2m"": [1.0],
                ""temperature_2m_member01"": [0.9],
                ""temperature_2m_member02"": [0.8],
                ""temperature_2m_member03"": [1.0]
            }
        }";

        var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
        var options = new WeatherEnsembleOptions();
        var parser = new WeatherEnsembleResponseParser(_jsonOptions);

        var result = await parser.DeserializeJsonAsync(response, options);

        Assert.IsNotNull(result?.Hourly?.Temperature_2m);
        
        var temperatures = new System.Collections.Generic.List<float?>();
        foreach (var (memberNum, forecast) in result.Hourly.Temperature_2m)
        {
            Console.WriteLine($"Member {memberNum}: {forecast![0]}°C");
            temperatures.Add(forecast[0]);
        }
        
        Assert.AreEqual(4, temperatures.Count, "Should have 4 members including member 0");
        CollectionAssert.Contains(temperatures, 1.0f); // Member 0 or 3
        CollectionAssert.Contains(temperatures, 0.9f); // Member 1
        CollectionAssert.Contains(temperatures, 0.8f); // Member 2
    }
}
