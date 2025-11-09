using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace OpenMeteo.Weather.ResponseModel
{
    public class DateTimeOffsetWithTimezoneConverter(string timezone) : JsonConverter<DateTimeOffset?>
    {
        private readonly string _timezone = timezone;

        public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value)) return null;

            if (!DateTime.TryParse(value, null, DateTimeStyles.None, out var localTime))
                throw new JsonException($"Invalid date format: {value}");

            var tzInfo = TimeZoneInfo.FindSystemTimeZoneById(_timezone);
            var offset = tzInfo.GetUtcOffset(localTime);
            return new DateTimeOffset(localTime, offset);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mm"));
            else
                writer.WriteNullValue();
        }
    }
}
