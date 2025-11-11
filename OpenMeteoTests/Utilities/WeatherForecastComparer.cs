using OpenMeteo.Weather.ResponseModel;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace OpenMeteoTests.Utilities;

public record WeatherForecastComparisonResult(List<string> UnequalFields)
{
    public bool IsEqual => UnequalFields.Count == 0;
}

public static class WeatherForecastComparer
{
    public static WeatherForecastComparisonResult Compare(WeatherForecast a, WeatherForecast b)
    {
        var unequalFields = new List<string>();
        if (a == null || b == null)
        {
            unequalFields.Add("WeatherForecast (null)");
            return new WeatherForecastComparisonResult(unequalFields);
        }
        if (a.Latitude != b.Latitude) unequalFields.Add(nameof(a.Latitude));
        if (a.Longitude != b.Longitude) unequalFields.Add(nameof(a.Longitude));
        if (a.Elevation != b.Elevation) unequalFields.Add(nameof(a.Elevation));
        if (a.UtcOffset != b.UtcOffset) unequalFields.Add(nameof(a.UtcOffset));
        //if (a.Timezone != b.Timezone) unequalFields.Add(nameof(a.Timezone));
        //if (a.TimezoneAbbreviation != b.TimezoneAbbreviation) unequalFields.Add(nameof(a.TimezoneAbbreviation));
        unequalFields.AddRange(HourlyMetricsUnequalFields(a.Hourly, b.Hourly));
        return new WeatherForecastComparisonResult(unequalFields);
    }

    private static List<string> HourlyMetricsUnequalFields(Hourly? a, Hourly? b)
    {
        var fields = new List<string>();
        if (a == null || b == null)
        {
            if (a != b) fields.Add("Hourly (null)");
            return fields;
        }
        var props = typeof(Hourly).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            var aValue = prop.GetValue(a);
            var bValue = prop.GetValue(b);
            if (!CompareArraysByType(prop.PropertyType, aValue, bValue))
                fields.Add($"Hourly.{prop.Name}");
        }
        return fields;
    }

    private static bool CompareArraysByType(Type type, object? a, object? b)
    {
        if (type == typeof(int[])) return IntArrayEqual((int[]?)a, (int[]?)b);
        if (type == typeof(float[])) return FloatArrayEqual((float[]?)a, (float[]?)b);
        if (type == typeof(int?[])) return NullableIntArrayEqual((int?[]?)a, (int?[]?)b);
        if (type == typeof(float?[])) return NullableFloatArrayEqual((float?[]?)a, (float?[]?)b);
        if (type == typeof(DateTimeOffset[])) return DateTimeOffsetArrayEqual((DateTimeOffset[]?)a, (DateTimeOffset[]?)b);
        // If not an array, treat as equal (or add more types as needed)
        return true;
    }

    private static bool IntArrayEqual(int[]? a, int[]? b)
    {
        if (a == null || b == null) return a == b;
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return false;
        return true;
    }

    private static bool FloatArrayEqual(float[]? a, float[]? b)
    {
        if (a == null || b == null) return a == b;
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
            if (!a[i].Equals(b[i])) return false;
        return true;
    }

    private static bool NullableIntArrayEqual(int?[]? a, int?[]? b)
    {
        if (a == null || b == null) return a == b;
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return false;
        return true;
    }

    private static bool NullableFloatArrayEqual(float?[]? a, float?[]? b)
    {
        if (a == null || b == null) return a == b;
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
            if (!Nullable.Equals(a[i], b[i])) return false;
        return true;
    }

    private static bool DateTimeOffsetArrayEqual(DateTimeOffset[]? a, DateTimeOffset[]? b)
    {
        if (a == null || b == null) return a == b;
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return false;
        return true;
    }
}
