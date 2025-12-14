using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Ensemble.ResponseModel;

namespace OpenMeteoTests.Weather.Ensemble;

[TestClass]
public class WeatherEnsembleResponseTests
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [TestMethod]
    public void Deserialize_EnsembleApiResponse_Success()
    {
        var json = @"{
            ""latitude"": 50.0,
            ""longitude"": -123.0,
            ""generationtime_ms"": 0.349,
            ""utc_offset_seconds"": 0,
            ""timezone"": ""GMT"",
            ""timezone_abbreviation"": ""GMT"",
            ""elevation"": 1643.0,
            ""hourly_units"": {
                ""time"": ""iso8601"",
                ""temperature_2m"": ""°C"",
                ""temperature_2m_member01"": ""°C""
            },
            ""hourly"": {
                ""time"": [""2025-12-14T00:00"", ""2025-12-14T01:00""],
                ""temperature_2m"": [0.9, 0.7],
                ""temperature_2m_member01"": [0.9, 0.6]
            }
        }";

        var result = JsonSerializer.Deserialize<WeatherEnsemble>(json, _jsonOptions);

        Assert.IsNotNull(result);
        Assert.AreEqual(50.0f, result.Latitude);
        Assert.AreEqual(-123.0f, result.Longitude);
        Assert.AreEqual(1643.0f, result.Elevation);
        Assert.AreEqual("GMT", result.Timezone);
        
        Assert.IsNotNull(result.Hourly);
        Assert.IsNotNull(result.Hourly.Time);
        Assert.AreEqual(2, result.Hourly.Time.Length);
        Assert.AreEqual("2025-12-14T00:00", result.Hourly.Time[0]);
        
        Assert.IsNotNull(result.Hourly.AdditionalData);
        Assert.IsTrue(result.Hourly.AdditionalData.ContainsKey("temperature_2m"));
        Assert.IsTrue(result.Hourly.AdditionalData.ContainsKey("temperature_2m_member01"));
        
        Assert.IsNotNull(result.HourlyUnits);
        Assert.AreEqual("iso8601", result.HourlyUnits.Time);
        Assert.IsNotNull(result.HourlyUnits.AdditionalData);
        Assert.IsTrue(result.HourlyUnits.AdditionalData.ContainsKey("temperature_2m"));
    }

    [TestMethod]
    public void Deserialize_EnsembleApiResponse_WithDaily_Success()
    {
        var json = @"{
            ""latitude"": 50.0,
            ""longitude"": -123.0,
            ""generationtime_ms"": 0.349,
            ""utc_offset_seconds"": 0,
            ""timezone"": ""GMT"",
            ""timezone_abbreviation"": ""GMT"",
            ""elevation"": 1643.0,
            ""daily_units"": {
                ""time"": ""iso8601"",
                ""temperature_2m_max"": ""°C"",
                ""temperature_2m_max_member01"": ""°C""
            },
            ""daily"": {
                ""time"": [""2025-12-14"", ""2025-12-15""],
                ""temperature_2m_max"": [5.5, 6.2],
                ""temperature_2m_max_member01"": [5.3, 6.0]
            }
        }";

        var result = JsonSerializer.Deserialize<WeatherEnsemble>(json, _jsonOptions);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Daily);
        Assert.IsNotNull(result.Daily.Time);
        Assert.AreEqual(2, result.Daily.Time.Length);
        
        Assert.IsNotNull(result.Daily.AdditionalData);
        Assert.IsTrue(result.Daily.AdditionalData.ContainsKey("temperature_2m_max"));
        Assert.IsTrue(result.Daily.AdditionalData.ContainsKey("temperature_2m_max_member01"));
        
        Assert.IsNotNull(result.DailyUnits);
        Assert.IsNotNull(result.DailyUnits.AdditionalData);
        Assert.IsTrue(result.DailyUnits.AdditionalData.ContainsKey("temperature_2m_max"));
    }
}
