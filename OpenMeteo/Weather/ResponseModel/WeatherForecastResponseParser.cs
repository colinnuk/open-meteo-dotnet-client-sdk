using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Google.FlatBuffers;
using OpenMeteo.Weather.Options;
using OpenMeteo.Weather.ResponseModel.Conversion;

namespace OpenMeteo.Weather.ResponseModel
{
    /// <summary>
    /// Handles parsing of weather forecast responses in different formats (JSON and FlatBuffers)
    /// </summary>
    public class WeatherForecastResponseParser(JsonSerializerOptions jsonSerializerOptions)
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;

        public async Task<WeatherForecast?> DeserializeJsonAsync(HttpResponseMessage response)
        {
            if (response == null || !response.IsSuccessStatusCode)
                return null;

            return await JsonSerializer.DeserializeAsync<WeatherForecast>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
        }

        public async Task<WeatherForecast?> ConvertFlatBuffersAsync(HttpResponseMessage response,
            WeatherForecastOptions? options)
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

        private WeatherForecast? ConvertFlatBuffers(byte[] bytes, WeatherForecastOptions? options)
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

        private static WeatherForecast ConvertFromFlatBuffers(openmeteo_sdk.WeatherApiResponse fbResponse, WeatherForecastOptions? options)
        {
            return new WeatherForecast
            {
                Latitude = fbResponse.Latitude,
                Longitude = fbResponse.Longitude,
                Elevation = fbResponse.Elevation,
                GenerationTime = fbResponse.GenerationTimeMilliseconds,
                UtcOffset = fbResponse.UtcOffsetSeconds,
                Timezone = fbResponse.Timezone,
                TimezoneAbbreviation = fbResponse.TimezoneAbbreviation,
                Current = CurrentConversion.ConvertCurrent(fbResponse.Current, options?.Current),
                Hourly = HourlyConversion.ConvertHourly(fbResponse.Hourly, options?.Hourly),
                Daily = DailyConversion.ConvertDaily(fbResponse.Daily, options?.Daily),
                Minutely15 = Minutely15Conversion.ConvertMinutely15(fbResponse.Minutely15, options?.Minutely_15)
            };
        }
    }
}
