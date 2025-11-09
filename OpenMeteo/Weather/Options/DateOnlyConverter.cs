using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenMeteo.Weather.Options
{
    public class DateOnlyConverter : JsonConverter<DateOnly?>
    {
        public const string Format = "yyyy-MM-dd";

        public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value)) return null;
            return DateOnly.ParseExact(value, Format);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString(Format));
            else
                writer.WriteNullValue();
        }
    }
}
