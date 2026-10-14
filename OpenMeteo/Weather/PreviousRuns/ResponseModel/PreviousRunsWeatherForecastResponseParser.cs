using OpenMeteo.Weather.Forecast.ResponseModel;
using OpenMeteo.Weather.Forecast.Units;
using OpenMeteo.Weather.PreviousRuns.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenMeteo.Weather.PreviousRuns.ResponseModel
{
    /// <summary>
    /// Parses flattened previous-run hourly fields into hourly forecasts grouped by lead time.
    /// </summary>
    public class PreviousRunsWeatherForecastResponseParser(JsonSerializerOptions jsonSerializerOptions)
    {
        private const string PreviousDaySuffix = "_previous_day";
        private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;

        public async Task<PreviousRunsWeatherForecast?> DeserializeJsonAsync(HttpResponseMessage response, PreviousRunsOptions options)
        {
            if (response == null || !response.IsSuccessStatusCode)
                return null;

            var serializerOptions = new JsonSerializerOptions(_jsonSerializerOptions);
            serializerOptions.Converters.Add(new DateTimeOffsetWithTimezoneConverter(options.Timezone));

            using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var root = document.RootElement;
            return new PreviousRunsWeatherForecast
            {
                Latitude = GetSingle(root, "latitude"),
                Longitude = GetSingle(root, "longitude"),
                Elevation = GetSingle(root, "elevation"),
                GenerationTime = GetSingle(root, "generationtime_ms"),
                UtcOffset = GetInt32(root, "utc_offset_seconds"),
                Timezone = GetString(root, "timezone"),
                TimezoneAbbreviation = GetString(root, "timezone_abbreviation"),
                Hourly = DeserializeHourly(root, serializerOptions)
            };
        }

        private static PreviousRunsHourly? DeserializeHourly(JsonElement root, JsonSerializerOptions serializerOptions)
        {
            if (!root.TryGetProperty("hourly", out var hourlyElement) || hourlyElement.ValueKind != JsonValueKind.Object)
                return null;

            var dataByDay = GroupVariables(hourlyElement, out var time);
            var unitsByDay = root.TryGetProperty("hourly_units", out var unitsElement) && unitsElement.ValueKind == JsonValueKind.Object
                ? GroupVariables(unitsElement, out _)
                : [];

            var runs = new Dictionary<int, Hourly>();
            var units = new Dictionary<int, HourlyUnits>();
            foreach (var (day, variables) in dataByDay)
            {
                runs[day] = Deserialize<Hourly>(variables, time, serializerOptions);
                if (unitsByDay.TryGetValue(day, out var unitVariables))
                    units[day] = Deserialize<HourlyUnits>(unitVariables, null, serializerOptions);
            }

            return new PreviousRunsHourly { Runs = runs, Units = units };
        }

        private static Dictionary<int, Dictionary<string, JsonElement>> GroupVariables(JsonElement element, out JsonElement? time)
        {
            time = null;
            var variablesByDay = new Dictionary<int, Dictionary<string, JsonElement>>();
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("time"))
                {
                    time = property.Value;
                    continue;
                }

                if (!TryGetPreviousDay(property.Name, out var variableName, out var day))
                    continue;

                if (!variablesByDay.TryGetValue(day, out var variables))
                {
                    variables = [];
                    variablesByDay.Add(day, variables);
                }

                variables.Add(variableName, property.Value);
            }

            return variablesByDay;
        }

        private static bool TryGetPreviousDay(string name, out string variableName, out int day)
        {
            var suffixIndex = name.LastIndexOf(PreviousDaySuffix, StringComparison.Ordinal);
            if (suffixIndex > 0 && int.TryParse(name[(suffixIndex + PreviousDaySuffix.Length)..], out day) && day is >= 1 and <= 7)
            {
                variableName = name[..suffixIndex];
                return true;
            }

            variableName = string.Empty;
            day = 0;
            return false;
        }

        private static T Deserialize<T>(Dictionary<string, JsonElement> variables, JsonElement? time, JsonSerializerOptions serializerOptions)
        {
            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                if (time.HasValue)
                {
                    writer.WritePropertyName("time");
                    time.Value.WriteTo(writer);
                }

                foreach (var (name, value) in variables)
                {
                    writer.WritePropertyName(name);
                    value.WriteTo(writer);
                }

                writer.WriteEndObject();
            }

            return JsonSerializer.Deserialize<T>(stream.ToArray(), serializerOptions)!;
        }

        private static float GetSingle(JsonElement element, string propertyName) =>
            element.TryGetProperty(propertyName, out var property) && property.TryGetSingle(out var value) ? value : default;

        private static int GetInt32(JsonElement element, string propertyName) =>
            element.TryGetProperty(propertyName, out var property) && property.TryGetInt32(out var value) ? value : default;

        private static string? GetString(JsonElement element, string propertyName) =>
            element.TryGetProperty(propertyName, out var property) ? property.GetString() : null;
    }
}
