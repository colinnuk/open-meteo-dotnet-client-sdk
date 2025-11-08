using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Google.FlatBuffers;
using OpenMeteo.Helpers;

namespace OpenMeteo.Weather.ResponseModel
{
    /// <summary>
    /// Handles parsing of weather forecast responses in different formats (JSON and FlatBuffers)
    /// </summary>
    internal class WeatherForecastResponseParser(JsonSerializerOptions jsonSerializerOptions)
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;

        /// <summary>
        /// Deserializes a JSON response to a WeatherForecast object
        /// </summary>
        /// <param name="response">HTTP response message</param>
        /// <returns>Deserialized WeatherForecast object or null</returns>
        public async Task<WeatherForecast?> DeserializeJsonAsync(HttpResponseMessage response)
        {
            if (response == null || !response.IsSuccessStatusCode)
                return null;

            return await JsonSerializer.DeserializeAsync<WeatherForecast>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
        }

        /// <summary>
        /// Deserializes a FlatBuffers response to a WeatherForecast object
        /// </summary>
        /// <param name="response">HTTP response message</param>
        /// <returns>Deserialized WeatherForecast object or null</returns>
        /// <remarks>
        /// Converts FlatBuffers format from openmeteo_sdk to our WeatherForecast model
        /// </remarks>
        public async Task<WeatherForecast?> ConvertFlatBuffersAsync(HttpResponseMessage response)
        {
            if (response == null || !response.IsSuccessStatusCode)
                return null;

            try
            {
                var bytes = await response.Content.ReadAsByteArrayAsync();
                var byteBuffer = new ByteBuffer(bytes);
                var fbResponse = openmeteo_sdk.WeatherApiResponse.GetRootAsWeatherApiResponse(byteBuffer);
                return ConvertFromFlatBuffers(fbResponse);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to deserialize FlatBuffers response: {ex.Message}", ex);
            }
        }

        private static WeatherForecast ConvertFromFlatBuffers(openmeteo_sdk.WeatherApiResponse fbResponse)
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
                Current = ConvertCurrent(fbResponse.Current),
                Hourly = ConvertHourly(fbResponse.Hourly),
                Daily = ConvertDaily(fbResponse.Daily),
                Minutely15 = ConvertMinutely15(fbResponse.Minutely15)
            };
        }

        #region Current Weather Conversion

        private static Current? ConvertCurrent(openmeteo_sdk.VariablesWithTime? fbCurrent)
        {
            if (fbCurrent == null) return null;

            var current = new Current
            {
                Time = ConvertUnixToIso8601(fbCurrent?.Time),
                Interval = fbCurrent?.Interval
            };

            for (int i = 0; i < fbCurrent?.VariablesLength; i++)
            {
                MapCurrentVariable(current, fbCurrent?.Variables(i));
            }

            return current;
        }

        private static void MapCurrentVariable(Current current, openmeteo_sdk.VariableWithValues? variable)
        {
            if (variable is null || variable.Value.ValuesLength == 0) return;

            var value = variable.Value.Values(0);
            if (float.IsNaN(value)) return;

            switch (variable.Value.Variable)
            {
                case openmeteo_sdk.Variable.temperature:
                    current.Temperature_2m = value;
                    return;
                case openmeteo_sdk.Variable.weather_code:
                    current.Weathercode = (int)value;
                    return;
                case openmeteo_sdk.Variable.apparent_temperature:
                    current.Apparent_temperature = value;
                    return;
                case openmeteo_sdk.Variable.is_day:
                    current.Is_day = (int)value;
                    return;
                case openmeteo_sdk.Variable.precipitation:
                    current.Precipitation = value;
                    return;
                case openmeteo_sdk.Variable.rain:
                    current.Rain = value;
                    return;
                case openmeteo_sdk.Variable.showers:
                    current.Showers = value;
                    return;
                case openmeteo_sdk.Variable.snowfall:
                    current.Snowfall = value;
                    return;
                case openmeteo_sdk.Variable.pressure_msl:
                    current.Pressure_msl = value;
                    return;
                case openmeteo_sdk.Variable.surface_pressure:
                    current.Surface_pressure = value;
                    return;
                case openmeteo_sdk.Variable.wind_speed:
                    current.Windspeed_10m = value;
                    return;
                case openmeteo_sdk.Variable.wind_direction:
                    current.Winddirection_10m = (int)value;
                    return;
                case openmeteo_sdk.Variable.wind_gusts:
                    current.Windgusts_10m = value;
                    return;
                case openmeteo_sdk.Variable.relative_humidity:
                    current.Relativehumidity_2m = (int)value;
                    return;
                case openmeteo_sdk.Variable.cloud_cover:
                    current.Cloudcover = (int)value;
                    return;
            }
        }

        #endregion

        #region Hourly Data Conversion

        private static Hourly? ConvertHourly(openmeteo_sdk.VariablesWithTime? fbHourly)
        {
            if (fbHourly == null) return null;
            var hourly = new Hourly
            {
                Time = BuildTimeArray(fbHourly)
            };
            for (int i = 0; i < fbHourly?.VariablesLength; i++)
            {
                MapHourlyVariable(hourly, fbHourly?.Variables(i));
            }
            return hourly;
        }

        private static void MapHourlyVariable(Hourly hourly, openmeteo_sdk.VariableWithValues? variable)
        {
            if (variable is null) return;

            switch (variable.Value.Variable)
            {
                case openmeteo_sdk.Variable.temperature:
                    hourly.Temperature_2m = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.apparent_temperature:
                    hourly.Apparent_temperature = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.precipitation:
                    hourly.Precipitation = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.rain:
                    hourly.Rain = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.showers:
                    hourly.Showers = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.snowfall:
                    hourly.Snowfall = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.snow_depth:
                    hourly.Snow_depth = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.weather_code:
                    hourly.Weathercode = variable.Value.GetValuesInt64Array().ToIntArray().ToNullableIntArray();
                    return;
                case openmeteo_sdk.Variable.pressure_msl:
                    hourly.Pressure_msl = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.surface_pressure:
                    hourly.Surface_pressure = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.visibility:
                    hourly.Visibility = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.evapotranspiration:
                    hourly.Evapotranspiration = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.cape:
                    hourly.Cape = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.wind_speed:
                    hourly.Windspeed_10m = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.wind_direction:
                    hourly.Winddirection_10m = variable.Value.GetValuesInt64Array().ToIntArray().ToNullableIntArray();
                    return;
                case openmeteo_sdk.Variable.wind_gusts:
                    hourly.Windgusts_10m = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.shortwave_radiation:
                    hourly.Shortwave_radiation = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.direct_radiation:
                    hourly.Direct_radiation = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.diffuse_radiation:
                    hourly.Diffuse_radiation = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.terrestrial_radiation:
                    hourly.Terrestrial_radiation = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.uv_index:
                    hourly.Uv_index = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.is_day:
                    hourly.Is_day = variable.Value.GetValuesInt64Array().ToIntArray().ToNullableIntArray();
                    return;
                case openmeteo_sdk.Variable.precipitation_probability:
                    hourly.Precipitation_probability = variable.Value.GetValuesInt64Array().ToIntArray().ToNullableIntArray();
                    return;
            }
        }

        #endregion

        #region Daily Data Conversion

        private static Daily? ConvertDaily(openmeteo_sdk.VariablesWithTime? fbDaily)
        {
            if (fbDaily == null) return null;
            var daily = new Daily
            {
                Time = BuildTimeArray(fbDaily)
            };
            for (int i = 0; i < fbDaily?.VariablesLength; i++)
            {
                var variable = fbDaily?.Variables(i);
                if (variable.HasValue)
                {
                    MapDailyVariable(daily, variable.Value);
                }
            }
            return daily;
        }

        private static void MapDailyVariable(Daily daily, openmeteo_sdk.VariableWithValues? variable)
        {
            if (variable is null) return;

            switch (variable.Value.Variable)
            {
                case openmeteo_sdk.Variable.weather_code:
                    daily.Weathercode = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.precipitation_hours:
                    daily.Precipitation_hours = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
            }
        }

        #endregion

        #region Minutely15 Data Conversion

        private static Minutely15? ConvertMinutely15(openmeteo_sdk.VariablesWithTime? fbMinutely15)
        {
            if (fbMinutely15 == null) return null;
            var minutely = new Minutely15
            {
                time = BuildTimeArray(fbMinutely15)
            };
            for (int i = 0; i < fbMinutely15?.VariablesLength; i++)
            {
                MapMinutely15Variable(minutely, fbMinutely15?.Variables(i));
            }
            return minutely;
        }

        private static void MapMinutely15Variable(Minutely15 minutely, openmeteo_sdk.VariableWithValues? variable)
        {
            if (variable is null) return;

            switch (variable.Value.Variable)
            {
                case openmeteo_sdk.Variable.temperature:
                    minutely.temperature_2m = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.relative_humidity:
                    minutely.relativehumidity_2m = variable.Value.GetValuesInt64Array().ToIntArray();
                    return;
                case openmeteo_sdk.Variable.dew_point:
                    minutely.dewpoint_2m = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.apparent_temperature:
                    minutely.apparent_temperature = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.precipitation:
                    minutely.precipitation = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.rain:
                    minutely.rain = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.snowfall:
                    minutely.snowfall = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.snowfall_height:
                    minutely.snowfall_height = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.freezing_level_height:
                    minutely.freezing_level_height = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.weather_code:
                    minutely.weathercode = variable.Value.GetValuesInt64Array().ToIntArray();
                    return;
                case openmeteo_sdk.Variable.wind_speed:
                    minutely.windspeed_10m = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.wind_direction:
                    minutely.winddirection_10m = variable.Value.GetValuesInt64Array().ToIntArray();
                    return;
                case openmeteo_sdk.Variable.wind_gusts:
                    minutely.windgusts_10m = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.visibility:
                    minutely.visibility = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.cape:
                    minutely.cape = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.lightning_potential:
                    minutely.lightning_potential = variable.Value.GetValuesArray().ToNullableFloatArray();
                    return;
                case openmeteo_sdk.Variable.shortwave_radiation:
                    minutely.shortwave_radiation = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.direct_radiation:
                    minutely.direct_radiation = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.diffuse_radiation:
                    minutely.diffuse_radiation = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.direct_normal_irradiance:
                    minutely.direct_normal_irradiance = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.terrestrial_radiation:
                    minutely.terrestrial_radiation = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.shortwave_radiation_instant:
                    minutely.shortwave_radiation_instant = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.direct_radiation_instant:
                    minutely.direct_radiation_instant = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.diffuse_radiation_instant:
                    minutely.diffuse_radiation_instant = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.direct_normal_irradiance_instant:
                    minutely.direct_normal_irradiance_instant = variable.Value.GetValuesArray();
                    return;
                case openmeteo_sdk.Variable.terrestrial_radiation_instant:
                    minutely.terrestrial_radiation_instant = variable.Value.GetValuesArray();
                    return;
            }
        }

        #endregion

        private static int GetTimeCount(openmeteo_sdk.VariablesWithTime? variablesWithTime)
        {
            if (variablesWithTime?.VariablesLength > 0)
            {
                var variable = variablesWithTime?.Variables(0);
                if (variable.HasValue)
                {
                    return variable.Value.ValuesLength;
                }
            }
            return 0;
        }

        private static string[]? BuildTimeArray(openmeteo_sdk.VariablesWithTime? variablesWithTime)
        {
            if (variablesWithTime == null || variablesWithTime.Value.Time <= 0)
                return null;
            int timeCount = GetTimeCount(variablesWithTime);
            var result = new string[timeCount];
            for (int i = 0; i < timeCount; i++)
            {
                // This does the maths to calculate the time of this index based on start time and interval
                result[i] = ConvertUnixToIso8601(variablesWithTime.Value.Time + (i * variablesWithTime.Value.Interval));
            }
            return result;
        }

        public static string ConvertUnixToIso8601(long? epochSeconds)
        {
            if (epochSeconds == null) return string.Empty;

            var dateTime = DateTimeOffset.FromUnixTimeSeconds(epochSeconds.Value).UtcDateTime;
            return dateTime.ToString("yyyy-MM-dd'T'HH:mm");
        }
    }
}
