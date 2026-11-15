using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// Deserializes a JSON response into a WeatherEnsemble object.
    /// When several models were requested, returns the data for the first one; use <see cref="DeserializeJsonListAsync"/> to get all of them.
    /// </summary>
    /// <param name="response">HttpResponseMessage containing the JSON response</param>
    /// <param name="options">WeatherEnsembleOptions object containing the options for the request</param>
    /// <returns></returns>
    public async Task<WeatherEnsemble?> DeserializeJsonAsync(HttpResponseMessage response, WeatherEnsembleOptions options)
    {
        var ensembles = await DeserializeJsonListAsync(response, options);
        return ensembles?.FirstOrDefault();
    }

    /// <summary>
    /// Deserializes a JSON response into one <see cref="WeatherEnsemble"/> per requested model, in the order of <see cref="WeatherEnsembleOptions.Models"/>.
    /// </summary>
    /// <param name="response">HttpResponseMessage containing the JSON response</param>
    /// <param name="options">WeatherEnsembleOptions object containing the options for the request</param>
    public async Task<IReadOnlyList<WeatherEnsemble>?> DeserializeJsonListAsync(HttpResponseMessage response, WeatherEnsembleOptions options)
    {
        if (response == null || !response.IsSuccessStatusCode)
            return null;

        var dto = await JsonSerializer.DeserializeAsync<WeatherEnsembleDto>(
            await response.Content.ReadAsStreamAsync(),
            _jsonSerializerOptions);
        if (dto == null)
            return null;

        // With one model the keys carry no model suffix
        if (options == null || options.Models.Count <= 1)
            return [ConvertDtoToPublicModel(dto, options, ModelAt(options, 0))];

        return EnsembleModelSplitter.Split(dto, options)
            .Select((modelDto, index) => ConvertDtoToPublicModel(modelDto, options, ModelAt(options, index)))
            .ToList();
    }

    /// <summary>
    /// Converts a FlatBuffers response into a WeatherEnsemble object.
    /// When several models were requested, returns the data for the first one; use <see cref="ConvertFlatBuffersListAsync"/> to get all of them.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<WeatherEnsemble?> ConvertFlatBuffersAsync(HttpResponseMessage response,
        WeatherEnsembleOptions? options)
    {
        var ensembles = await ConvertFlatBuffersListAsync(response, options);
        return ensembles?.FirstOrDefault();
    }

    /// <summary>
    /// Converts a FlatBuffers response into one <see cref="WeatherEnsemble"/> per requested model, in the order of <see cref="WeatherEnsembleOptions.Models"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<IReadOnlyList<WeatherEnsemble>?> ConvertFlatBuffersListAsync(HttpResponseMessage response,
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

    private static IReadOnlyList<WeatherEnsemble>? ConvertFlatBuffers(byte[] bytes, WeatherEnsembleOptions? options)
    {
        if (bytes == null || bytes.Length == 0)
            return null;

        try
        {
            // The API sends one size-prefixed FlatBuffers message per model, in the requested order
            var ensembles = new List<WeatherEnsemble>();
            var offset = 0;
            while (offset < bytes.Length)
            {
                if (bytes.Length - offset < sizeof(int))
                    throw new InvalidDataException("The FlatBuffers response contains an incomplete size prefix.");

                var messageLength = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(offset, sizeof(int)));
                if (messageLength <= 0 || messageLength > bytes.Length - offset - sizeof(int))
                    throw new InvalidDataException("The FlatBuffers response contains an invalid message size.");

                var byteBuffer = new ByteBuffer(bytes, offset + sizeof(int));
                var fbResponse = openmeteo_sdk.WeatherApiResponse.GetRootAsWeatherApiResponse(byteBuffer);
                ensembles.Add(ConvertFromFlatBuffers(fbResponse, options, ModelAt(options, ensembles.Count)));
                offset += sizeof(int) + messageLength;
            }
            return ensembles;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to convert FlatBuffers response: {ex.Message}", ex);
        }
    }

    private static EnsembleModelOptionsParameter? ModelAt(WeatherEnsembleOptions? options, int index) =>
        options != null && index < options.Models.Count ? options.Models[index] : null;

    private static WeatherEnsemble ConvertFromFlatBuffers(openmeteo_sdk.WeatherApiResponse fbResponse, WeatherEnsembleOptions? options, EnsembleModelOptionsParameter? model)
    {
        return new WeatherEnsemble
        {
            Model = model,
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

    private static WeatherEnsemble ConvertDtoToPublicModel(WeatherEnsembleDto dto, WeatherEnsembleOptions? options, EnsembleModelOptionsParameter? model)
    {
        return new WeatherEnsemble
        {
            Model = model,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Elevation = dto.Elevation,
            GenerationTime = dto.GenerationTime,
            UtcOffset = dto.UtcOffset,
            Timezone = dto.Timezone,
            TimezoneAbbreviation = dto.TimezoneAbbreviation,
            Hourly = EnsembleHourlyConverter.ToPublicModel(dto.Hourly, options?.Timezone),
            HourlyUnits = dto.HourlyUnits,
            Daily = EnsembleDailyConverter.ToPublicModel(dto.Daily),
            DailyUnits = dto.DailyUnits
        };
    }
}
