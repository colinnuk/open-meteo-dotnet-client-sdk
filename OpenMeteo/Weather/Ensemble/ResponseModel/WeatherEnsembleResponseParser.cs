using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Google.FlatBuffers;
using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Ensemble.ResponseModel.Conversion;

namespace OpenMeteo.Weather.Ensemble.ResponseModel;

/// <summary>
/// Handles parsing of weather ensemble responses in different formats (JSON and FlatBuffers)
/// </summary>
public class WeatherEnsembleResponseParser(JsonSerializerOptions jsonSerializerOptions)
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;

    /// <summary>
    /// Deserializes a JSON response into a WeatherEnsemble object
    /// </summary>
    /// <param name="response">HttpResponseMessage containing the JSON response</param>
    /// <param name="options">WeatherEnsembleOptions object containing the options for the request</param>
    /// <returns></returns>
    public async Task<WeatherEnsemble?> DeserializeJsonAsync(HttpResponseMessage response, WeatherEnsembleOptions options)
    {
        if (response == null || !response.IsSuccessStatusCode)
            return null;

        return await JsonSerializer.DeserializeAsync<WeatherEnsemble>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
    }

    /// <summary>
    /// Converts a FlatBuffers response into a WeatherEnsemble object.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<WeatherEnsemble?> ConvertFlatBuffersAsync(HttpResponseMessage response,
        WeatherEnsembleOptions? options)
    {
        if (response == null || !response.IsSuccessStatusCode)
            return null;

        try
        {
            var bytes = await response.Content.ReadAsByteArrayAsync();
            return ConvertFlatBuffers(bytes, options);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to deserialize FlatBuffers response: {ex.Message}", ex);
        }
    }

    private WeatherEnsemble? ConvertFlatBuffers(byte[] bytes, WeatherEnsembleOptions? options)
    {
        if (bytes == null || bytes.Length == 0)
            return null;

        try
        {
            // Open-Meteo API sends FlatBuffers with a 4-byte size prefix
            // We need to skip the first 4 bytes to get to the actual FlatBuffer data
            var byteBuffer = new ByteBuffer(bytes, 4);
            var fbResponse = openmeteo_sdk.WeatherApiResponse.GetRootAsWeatherApiResponse(byteBuffer);
            return ConvertFromFlatBuffers(fbResponse, options);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to convert FlatBuffers response: {ex.Message}", ex);
        }
    }

    private static WeatherEnsemble ConvertFromFlatBuffers(openmeteo_sdk.WeatherApiResponse fbResponse, WeatherEnsembleOptions? options)
    {
        return new WeatherEnsemble
        {
            Latitude = fbResponse.Latitude,
            Longitude = fbResponse.Longitude,
            Elevation = fbResponse.Elevation,
            GenerationTime = fbResponse.GenerationTimeMilliseconds,
            UtcOffset = fbResponse.UtcOffsetSeconds,
            Timezone = string.IsNullOrEmpty(fbResponse.Timezone) ? options?.Timezone : fbResponse.Timezone,
            TimezoneAbbreviation = string.IsNullOrEmpty(fbResponse.TimezoneAbbreviation) ? options?.Timezone : fbResponse.TimezoneAbbreviation,
            Hourly = WeatherEnsembleHourlyConversion.ConvertHourly(fbResponse.Hourly, options),
            HourlyUnits = WeatherEnsembleHourlyUnitsConversion.ConvertHourlyUnits(fbResponse.Hourly, options),
            Daily = WeatherEnsembleDailyConversion.ConvertDaily(fbResponse.Daily, options),
            DailyUnits = WeatherEnsembleDailyUnitsConversion.ConvertDailyUnits(fbResponse.Daily, options)
        };
    }
}
