using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Ensemble.ResponseModel;

namespace OpenMeteoTests.Weather.Ensemble;

[TestClass]
public class WeatherEnsembleResponseParserTests
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    [TestMethod]
    public async Task DeserializeJsonAsync_SimpleJson_Success()
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

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };
        var options = new WeatherEnsembleOptions
        {
            Timezone = "GMT",
            Hourly = new WeatherEnsembleHourlyOptions([WeatherEnsembleHourlyOptionsParameter.temperature_2m])
        };
        var parser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);

        var ensemble = await parser.DeserializeJsonAsync(response, options);

        Assert.IsNotNull(ensemble);
        Assert.AreEqual(50.0f, ensemble.Latitude);
        Assert.AreEqual(-123.0f, ensemble.Longitude);
        Assert.AreEqual(1643.0f, ensemble.Elevation);
        Assert.AreEqual("GMT", ensemble.Timezone);
        Assert.IsNotNull(ensemble.Hourly);
        Assert.IsNotNull(ensemble.Hourly.Time);
        Assert.AreEqual(2, ensemble.Hourly.Time.Length);
    }

    [TestMethod]
    public async Task DeserializeJsonAsync_WithDailyData_Success()
    {
        var json = @"{
            ""latitude"": 52.52,
            ""longitude"": 13.41,
            ""generationtime_ms"": 0.5,
            ""utc_offset_seconds"": 3600,
            ""timezone"": ""Europe/Berlin"",
            ""timezone_abbreviation"": ""CET"",
            ""elevation"": 38.0,
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

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };
        var options = new WeatherEnsembleOptions
        {
            Timezone = "Europe/Berlin",
            Daily = new WeatherEnsembleDailyOptions([WeatherEnsembleDailyOptionsParameter.temperature_2m_max])
        };
        var parser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);

        var ensemble = await parser.DeserializeJsonAsync(response, options);

        Assert.IsNotNull(ensemble);
        Assert.AreEqual(52.52f, ensemble.Latitude);
        Assert.AreEqual(13.41f, ensemble.Longitude);
        Assert.AreEqual("Europe/Berlin", ensemble.Timezone);
        Assert.IsNotNull(ensemble.Daily);
        Assert.IsNotNull(ensemble.Daily.Time);
        Assert.AreEqual(2, ensemble.Daily.Time.Length);
        Assert.AreEqual(new DateOnly(2025, 12, 14), ensemble.Daily.Time[0]);
        Assert.AreEqual(new DateOnly(2025, 12, 15), ensemble.Daily.Time[1]);
    }

    [TestMethod]
    public async Task DeserializeJsonAsync_WithMultipleEnsembleMembers_CapturesAllMembers()
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
                ""temperature_2m_member01"": ""°C"",
                ""temperature_2m_member02"": ""°C"",
                ""temperature_2m_member03"": ""°C""
            },
            ""hourly"": {
                ""time"": [""2025-12-14T00:00""],
                ""temperature_2m"": [0.9],
                ""temperature_2m_member01"": [0.9],
                ""temperature_2m_member02"": [0.8],
                ""temperature_2m_member03"": [1.0]
            }
        }";

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };
        var options = new WeatherEnsembleOptions
        {
            Hourly = new WeatherEnsembleHourlyOptions([WeatherEnsembleHourlyOptionsParameter.temperature_2m])
        };
        var parser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);

        var ensemble = await parser.DeserializeJsonAsync(response, options);

        Assert.IsNotNull(ensemble);
        Assert.IsNotNull(ensemble.Hourly);
        Assert.IsNotNull(ensemble.Hourly.AdditionalData);
        Assert.IsTrue(ensemble.Hourly.AdditionalData.ContainsKey("temperature_2m"));
        Assert.IsTrue(ensemble.Hourly.AdditionalData.ContainsKey("temperature_2m_member01"));
        Assert.IsTrue(ensemble.Hourly.AdditionalData.ContainsKey("temperature_2m_member02"));
        Assert.IsTrue(ensemble.Hourly.AdditionalData.ContainsKey("temperature_2m_member03"));
    }

    [TestMethod]
    public async Task DeserializeJsonAsync_NullResponse_ReturnsNull()
    {
        var parser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);
        var options = new WeatherEnsembleOptions();

        var result = await parser.DeserializeJsonAsync(null, options);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task DeserializeJsonAsync_FailedResponse_ReturnsNull()
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var parser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);
        var options = new WeatherEnsembleOptions();

        var result = await parser.DeserializeJsonAsync(response, options);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task ConvertFlatBuffersAsync_NullResponse_ReturnsNull()
    {
        var parser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);
        var options = new WeatherEnsembleOptions();

        var result = await parser.ConvertFlatBuffersAsync(null, options);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task ConvertFlatBuffersAsync_FailedResponse_ReturnsNull()
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var parser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);
        var options = new WeatherEnsembleOptions();

        var result = await parser.ConvertFlatBuffersAsync(response, options);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task ConvertFlatBuffersAsync_EmptyBytes_ReturnsNull()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent([])
        };
        var parser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);
        var options = new WeatherEnsembleOptions();

        var result = await parser.ConvertFlatBuffersAsync(response, options);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task ConvertFlatBuffersAsync_InvalidBytes_ThrowsInvalidOperationException()
    {
        var invalidBytes = new byte[] { 0x00, 0x00, 0x00, 0x08, 0xFF, 0xFF, 0xFF, 0xFF };
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(invalidBytes)
        };
        var parser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);
        var options = new WeatherEnsembleOptions();

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            async () => await parser.ConvertFlatBuffersAsync(response, options));
    }
}
