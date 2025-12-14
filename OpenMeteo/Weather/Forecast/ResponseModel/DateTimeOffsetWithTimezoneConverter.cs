using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace OpenMeteo.Weather.Forecast.ResponseModel
{
    public class DateTimeOffsetWithTimezoneConverter(string timezone) : JsonConverter<DateTimeOffset>
    {
        private readonly string _timezone = timezone;

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
                throw new JsonException("Date value cannot be null or empty");

            if (!DateTime.TryParse(value, null, DateTimeStyles.None, out var parsedTime))
                throw new JsonException($"Invalid date format: {value}");

            // Treat parsedTime as being in the specified timezone
            var unspecified = DateTime.SpecifyKind(parsedTime, DateTimeKind.Unspecified);
            var tzInfo = TimeZoneInfo.FindSystemTimeZoneById(_timezone);
            var offset = tzInfo.GetUtcOffset(unspecified);
            return new DateTimeOffset(unspecified, offset);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm"));
        }
    }
}
