using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenMeteo.Weather.Seasonal.ResponseModel;

/// <summary>
/// Parses JSON responses of the Seasonal Forecast API
/// </summary>
public class SeasonalResponseParser
{
    /// <summary>
    /// Deserializes a JSON response into a <see cref="WeatherSeasonal"/> object
    /// </summary>
    public async Task<WeatherSeasonal?> DeserializeJsonAsync(HttpResponseMessage response)
    {
        if (response == null || !response.IsSuccessStatusCode)
            return null;

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        return Parse(document.RootElement);
    }

    /// <summary>
    /// Converts a FlatBuffers response into a <see cref="WeatherSeasonal"/> object.
    /// Weekly data is not supported in this format, see <see cref="WeatherSeasonal.Weekly"/>.
    /// </summary>
    /// <param name="response">HttpResponseMessage containing the FlatBuffers response</param>
    /// <param name="requestedTimezone">Timezone used for the request, used when the response carries none</param>
    public async Task<WeatherSeasonal?> ConvertFlatBuffersAsync(HttpResponseMessage response, string? requestedTimezone)
    {
        if (response == null || !response.IsSuccessStatusCode)
            return null;

        try
        {
            var bytes = await response.Content.ReadAsByteArrayAsync();
            if (bytes == null || bytes.Length == 0)
                return null;

            // Open-Meteo API sends FlatBuffers with a 4-byte size prefix
            var byteBuffer = new Google.FlatBuffers.ByteBuffer(bytes, 4);
            var fbResponse = openmeteo_sdk.WeatherApiResponse.GetRootAsWeatherApiResponse(byteBuffer);
            return SeasonalFlatBuffersConversion.Convert(fbResponse, requestedTimezone);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to deserialize FlatBuffers response: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Parses a JSON string into a <see cref="WeatherSeasonal"/> object
    /// </summary>
    public WeatherSeasonal? Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        return Parse(document.RootElement);
    }

    private static WeatherSeasonal? Parse(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object)
            return null;

        return new WeatherSeasonal
        {
            Latitude = GetFloat(root, "latitude"),
            Longitude = GetFloat(root, "longitude"),
            Elevation = GetFloat(root, "elevation"),
            GenerationTime = GetFloat(root, "generationtime_ms"),
            UtcOffset = root.TryGetProperty("utc_offset_seconds", out var offset) && offset.ValueKind == JsonValueKind.Number ? offset.GetInt32() : 0,
            Timezone = GetString(root, "timezone"),
            TimezoneAbbreviation = GetString(root, "timezone_abbreviation"),
            Hourly = ParseSeries(root, "hourly"),
            Daily = ParseSeries(root, "daily"),
            Weekly = ParseSeries(root, "weekly"),
            Monthly = ParseSeries(root, "monthly")
        };
    }

    private static SeasonalSeries? ParseSeries(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var data) || data.ValueKind != JsonValueKind.Object)
            return null;

        var series = new SeasonalSeries();

        foreach (var property in data.EnumerateObject())
        {
            if (property.Value.ValueKind != JsonValueKind.Array)
                continue;

            if (property.Name == "time")
            {
                var times = new List<DateTime>();
                foreach (var item in property.Value.EnumerateArray())
                    times.Add(ParseTime(item));
                series.Time = [.. times];
                continue;
            }

            var values = new List<float?>();
            foreach (var item in property.Value.EnumerateArray())
                values.Add(item.ValueKind == JsonValueKind.Number ? item.GetSingle() : null);
            series.Variables[property.Name] = [.. values];
        }

        if (root.TryGetProperty(name + "_units", out var units) && units.ValueKind == JsonValueKind.Object)
        {
            foreach (var unit in units.EnumerateObject())
            {
                if (unit.Name != "time" && unit.Value.ValueKind == JsonValueKind.String)
                    series.Units[unit.Name] = unit.Value.GetString() ?? string.Empty;
            }
        }

        return series;
    }

    // iso8601 timestamps are local to the requested timezone, unixtime values are UTC instants
    private static DateTime ParseTime(JsonElement item)
    {
        if (item.ValueKind == JsonValueKind.Number)
            return DateTimeOffset.FromUnixTimeSeconds(item.GetInt64()).UtcDateTime;

        return DateTime.Parse(item.GetString() ?? string.Empty, CultureInfo.InvariantCulture, DateTimeStyles.None);
    }

    private static float GetFloat(JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.Number ? value.GetSingle() : 0f;

    private static string? GetString(JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
}
