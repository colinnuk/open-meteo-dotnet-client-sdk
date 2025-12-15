using System;
using System.IO;
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
        Assert.IsNotNull(ensemble.Hourly.Temperature_2m);
        
        // Should have 4 members: 0, 1, 2, and 3
        Assert.AreEqual(4, ensemble.Hourly.Temperature_2m.Count, "Should have 4 members (0, 1, 2, 3)");
        
        // Verify member 0 (base variable without suffix)
        Assert.IsTrue(ensemble.Hourly.Temperature_2m.ContainsKey(0), "Should contain member 0");
        Assert.IsNotNull(ensemble.Hourly.Temperature_2m[0]);
        Assert.AreEqual(0.9f, ensemble.Hourly.Temperature_2m[0]![0], "Member 0 value should be 0.9");
        
        // Verify member 1
        Assert.IsTrue(ensemble.Hourly.Temperature_2m.ContainsKey(1), "Should contain member 1");
        Assert.IsNotNull(ensemble.Hourly.Temperature_2m[1]);
        Assert.AreEqual(0.9f, ensemble.Hourly.Temperature_2m[1]![0], "Member 1 value should be 0.9");
        
        // Verify member 2
        Assert.IsTrue(ensemble.Hourly.Temperature_2m.ContainsKey(2), "Should contain member 2");
        Assert.IsNotNull(ensemble.Hourly.Temperature_2m[2]);
        Assert.AreEqual(0.8f, ensemble.Hourly.Temperature_2m[2]![0], "Member 2 value should be 0.8");
        
        // Verify member 3
        Assert.IsTrue(ensemble.Hourly.Temperature_2m.ContainsKey(3), "Should contain member 3");
        Assert.IsNotNull(ensemble.Hourly.Temperature_2m[3]);
        Assert.AreEqual(1.0f, ensemble.Hourly.Temperature_2m[3]![0], "Member 3 value should be 1.0");
    }

    [TestMethod]
    public async Task DeserializeJsonAsync_NullResponse_ReturnsNull()
    {
        var parser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);
        var options = new WeatherEnsembleOptions();

        var result = await parser.DeserializeJsonAsync(null!, options);

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

        var result = await parser.ConvertFlatBuffersAsync(null!, options);

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
            Content = new ByteArrayContent(Array.Empty<byte>())
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

    [TestMethod]
    public async Task ConvertFlatBuffersAsync_RealData_Success()
    {
        var filePath = Path.Combine("Weather", "Ensemble", "ExampleResponses", "Gem_GEPS_Whistler_20251214");
        var bytes = await File.ReadAllBytesAsync(filePath);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(bytes)
        };
        var options = new WeatherEnsembleOptions
        {
            Latitude = 50.0f,
            Longitude = -123.0f,
            Timezone = "GMT",
            Hourly = new WeatherEnsembleHourlyOptions([
                WeatherEnsembleHourlyOptionsParameter.temperature_2m,
                WeatherEnsembleHourlyOptionsParameter.precipitation,
                WeatherEnsembleHourlyOptionsParameter.rain,
                WeatherEnsembleHourlyOptionsParameter.snowfall
            ])
        };
        var parser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);

        var ensemble = await parser.ConvertFlatBuffersAsync(response, options);

        Assert.IsNotNull(ensemble);
        Assert.AreEqual(50.0f, ensemble.Latitude);
        Assert.AreEqual(-123.0f, ensemble.Longitude);
        Assert.AreEqual(1643.0f, ensemble.Elevation);
        Assert.AreEqual("GMT", ensemble.Timezone);
        Assert.IsNotNull(ensemble.Hourly);
        Assert.IsNotNull(ensemble.Hourly.Time);
        Assert.IsTrue(ensemble.Hourly.Time.Length > 0);
    }

    [TestMethod]
    public async Task FlatbufferAndJsonProduceIdenticalWeatherEnsembleObjects()
    {
        var jsonPath = Path.Combine("Weather", "Ensemble", "ExampleResponses", "GEM_GEPS_Whistler_20251214.json");
        var binPath = Path.Combine("Weather", "Ensemble", "ExampleResponses", "Gem_GEPS_Whistler_20251214");

        var json = await File.ReadAllTextAsync(jsonPath);
        var bin = await File.ReadAllBytesAsync(binPath);

        var jsonResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };
        var binResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(bin)
        };

        var options = new WeatherEnsembleOptions
        {
            Latitude = 50.0f,
            Longitude = -123.0f,
            Timezone = "GMT",
            Hourly = new WeatherEnsembleHourlyOptions([
                WeatherEnsembleHourlyOptionsParameter.temperature_2m,
                WeatherEnsembleHourlyOptionsParameter.precipitation,
                WeatherEnsembleHourlyOptionsParameter.rain,
                WeatherEnsembleHourlyOptionsParameter.snowfall
            ])
        };
        var parser = new WeatherEnsembleResponseParser(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var swJson = System.Diagnostics.Stopwatch.StartNew();
        var jsonEnsemble = await parser.DeserializeJsonAsync(jsonResponse, options);
        swJson.Stop();
        Console.WriteLine($"JSON parse time: {swJson.ElapsedMilliseconds} ms");

        var swFlat = System.Diagnostics.Stopwatch.StartNew();
        var flatbufferEnsemble = await parser.ConvertFlatBuffersAsync(binResponse, options);
        swFlat.Stop();
        Console.WriteLine($"FlatBuffer parse time: {swFlat.ElapsedMilliseconds} ms");

        Assert.IsNotNull(jsonEnsemble);
        Assert.IsNotNull(flatbufferEnsemble);

        // Compare basic properties
        Assert.AreEqual(jsonEnsemble.Latitude, flatbufferEnsemble.Latitude);
        Assert.AreEqual(jsonEnsemble.Longitude, flatbufferEnsemble.Longitude);
        Assert.AreEqual(jsonEnsemble.Elevation, flatbufferEnsemble.Elevation);
        Assert.AreEqual(jsonEnsemble.Timezone, flatbufferEnsemble.Timezone);
        Assert.AreEqual(jsonEnsemble.TimezoneAbbreviation, flatbufferEnsemble.TimezoneAbbreviation);

        // Compare hourly data
        Assert.IsNotNull(jsonEnsemble.Hourly);
        Assert.IsNotNull(flatbufferEnsemble.Hourly);
        Assert.AreEqual(jsonEnsemble.Hourly.Time!.Length, flatbufferEnsemble.Hourly.Time!.Length);

        // Compare ensemble members count
        Assert.IsNotNull(jsonEnsemble.Hourly.Temperature_2m);
        Assert.IsNotNull(flatbufferEnsemble.Hourly.Temperature_2m);
        Assert.AreEqual(jsonEnsemble.Hourly.Temperature_2m.Count, flatbufferEnsemble.Hourly.Temperature_2m.Count);

        Console.WriteLine($"JSON temperature members: {jsonEnsemble.Hourly.Temperature_2m.Count}");
        Console.WriteLine($"FlatBuffer temperature members: {flatbufferEnsemble.Hourly.Temperature_2m.Count}");
    }
}
