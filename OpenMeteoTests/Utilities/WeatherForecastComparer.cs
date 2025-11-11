using OpenMeteo.Weather.ResponseModel;
using System;
using System.Reflection;

namespace OpenMeteoTests.Utilities;

public static class WeatherForecastComparer
{
    public static bool WeatherForecastsAreEqual(WeatherForecast a, WeatherForecast b)
    {
        if (a == null || b == null) return false;
        if (a.Latitude != b.Latitude) return false;
        if (a.Longitude != b.Longitude) return false;
        if (a.Elevation != b.Elevation) return false;
        if (a.GenerationTime != b.GenerationTime) return false;
        if (a.UtcOffset != b.UtcOffset) return false;
        if (a.Timezone != b.Timezone) return false;
        if (a.TimezoneAbbreviation != b.TimezoneAbbreviation) return false;
        if (!HourlyMetricsAreEqual(a.Hourly, b.Hourly)) return false;
        return true;
    }

    private static bool HourlyMetricsAreEqual(Hourly? a, Hourly? b)
    {
        if (a == null || b == null) return a == b;
        var props = typeof(Hourly).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            var aValue = prop.GetValue(a);
            var bValue = prop.GetValue(b);
            if (!CompareArraysByType(prop.PropertyType, aValue, bValue))
                return false;
        }
        return true;
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
