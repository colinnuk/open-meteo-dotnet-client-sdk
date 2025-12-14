using System;
using System.Reflection;
using System.Collections.Generic;
using OpenMeteo.Weather.Forecast.ResponseModel;

namespace OpenMeteoTests.Utilities;

public record WeatherForecastComparisonResult(List<string> UnequalFields)
{
    public bool IsEqual => UnequalFields.Count == 0;
}

public static class WeatherForecastComparer
{
    // JSON values are often rounded to 1 decimal place, while FlatBuffers preserves more precision
    // Allow 0.1 difference to account for this (negligible for weather data)
    private const float FloatTolerance = 0.1f;

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
        if (a.Timezone != b.Timezone) unequalFields.Add(nameof(a.Timezone));
        if (a.TimezoneAbbreviation != b.TimezoneAbbreviation) unequalFields.Add(nameof(a.TimezoneAbbreviation));
        unequalFields.AddRange(CurrentMetricsUnequalFields(a.Current, b.Current));
        unequalFields.AddRange(HourlyMetricsUnequalFields(a.Hourly, b.Hourly));
        unequalFields.AddRange(DailyMetricsUnequalFields(a.Daily, b.Daily));
        unequalFields.AddRange(Minutely15MetricsUnequalFields(a.Minutely15, b.Minutely15));
        return new WeatherForecastComparisonResult(unequalFields);
    }

    private static List<string> CurrentMetricsUnequalFields(Current? a, Current? b)
    {
        var fields = new List<string>();
        if (a == null || b == null)
        {
            if (a != b) fields.Add("Current (null)");
            return fields;
        }
        var props = typeof(Current).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            var aValue = prop.GetValue(a);
            var bValue = prop.GetValue(b);
            var type = prop.PropertyType;
            if (type == typeof(float?))
            {
                var fa = (float?)aValue;
                var fb = (float?)bValue;
                if (!NullableFloatEqual(fa, fb))
                    fields.Add($"Current.{prop.Name}");
            }
            else if (type == typeof(int?))
            {
                var ia = (int?)aValue;
                var ib = (int?)bValue;
                if (ia != ib)
                    fields.Add($"Current.{prop.Name}");
            }
        }
        return fields;
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
            if (!CompareArraysByType(prop.PropertyType, aValue, bValue, prop.Name, "Hourly."))
                fields.Add($"Hourly.{prop.Name}");
        }
        return fields;
    }

    private static List<string> DailyMetricsUnequalFields(Daily? a, Daily? b)
    {
        var fields = new List<string>();
        if (a == null || b == null)
        {
            if (a != b) fields.Add("Daily (null)");
            return fields;
        }
        var props = typeof(Daily).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            var aValue = prop.GetValue(a);
            var bValue = prop.GetValue(b);
            if (!CompareArraysByType(prop.PropertyType, aValue, bValue, prop.Name, "Daily."))
                fields.Add($"Daily.{prop.Name}");
        }
        return fields;
    }

    private static List<string> Minutely15MetricsUnequalFields(Minutely15? a, Minutely15? b)
    {
        var fields = new List<string>();
        if (a == null || b == null)
        {
            if (a != b) fields.Add("Minutely15 (null)");
            return fields;
        }
        var props = typeof(Minutely15).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            var aValue = prop.GetValue(a);
            var bValue = prop.GetValue(b);
            if (!CompareArraysByType(prop.PropertyType, aValue, bValue, prop.Name, "Minutely15."))
                fields.Add($"Minutely15.{prop.Name}");
        }
        return fields;
    }

    private static bool CompareArraysByType(Type type, object? a, object? b, string propertyName, string sectionName = "")
    {
        if (type == typeof(int[])) return IntArrayEqual((int[]?)a, (int[]?)b, propertyName, sectionName);
        if (type == typeof(float[])) return FloatArrayEqual((float[]?)a, (float[]?)b, propertyName, sectionName);
        if (type == typeof(int?[])) return NullableIntArrayEqual((int?[]?)a, (int?[]?)b, propertyName, sectionName);
        if (type == typeof(float?[])) return NullableFloatArrayEqual((float?[]?)a, (float?[]?)b, propertyName, sectionName);
        if (type == typeof(DateTimeOffset[])) return DateTimeOffsetArrayEqual((DateTimeOffset[]?)a, (DateTimeOffset[]?)b, propertyName, sectionName);

        return true;
    }

    private static bool IntArrayEqual(int[]? a, int[]? b, string propertyName, string sectionName = "")
    {
        if (a == null || b == null) return a == b;
        if (a.Length != b.Length)
        {
            Console.WriteLine($"  {sectionName}{propertyName}: Length mismatch - JSON: {a.Length}, FlatBuffers: {b.Length}");
            return false;
        }
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return false;
        return true;
    }

    private static bool FloatArrayEqual(float[]? a, float[]? b, string propertyName, string sectionName = "")
    {
        if (a == null || b == null) return a == b;
        if (a.Length != b.Length)
        {
            Console.WriteLine($"  {sectionName}{propertyName}: Length mismatch - JSON: {a.Length}, FlatBuffers: {b.Length}");
            return false;
        }
        for (int i = 0; i < a.Length; i++)
            if (!FloatsEqual(a[i], b[i])) return false;
        return true;
    }

    private static bool NullableIntArrayEqual(int?[]? a, int?[]? b, string propertyName, string sectionName = "")
    {
        if (a == null || b == null) return a == b;
        if (a.Length != b.Length)
        {
            Console.WriteLine($"  {sectionName}{propertyName}: Length mismatch - JSON: {a.Length}, FlatBuffers: {b.Length}");
            return false;
        }
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return false;
        return true;
    }

    private static bool NullableFloatArrayEqual(float?[]? a, float?[]? b, string propertyName, string sectionName = "")
    {
        if (a == null || b == null)
        {
            if (a != b)
            {
                Console.WriteLine($"  {sectionName}{propertyName}: Null mismatch - JSON: {a == null}, FlatBuffers: {b == null}");
            }
            return a == b;
        }
        if (a.Length != b.Length)
        {
            Console.WriteLine($"  {sectionName}{propertyName}: Length mismatch - JSON: {a.Length}, FlatBuffers: {b.Length}");
            return false;
        }

        int diffCount = 0;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] == null && b[i] == null) continue;
            if (a[i] == null || b[i] == null)
            {
                if (diffCount < 5)
                    Console.WriteLine($"  {sectionName}{propertyName}[{i}]: Null mismatch - JSON: {a[i]}, FlatBuffers: {b[i]}");
                diffCount++;
                continue;
            }
            if (!FloatsEqual(a[i]!.Value, b[i]!.Value))
            {
                if (diffCount < 5)
                    Console.WriteLine($"  {sectionName}{propertyName}[{i}]: Value mismatch - JSON: {a[i]!.Value}, FlatBuffers: {b[i]!.Value}, Diff: {Math.Abs(a[i]!.Value - b[i]!.Value)}");
                diffCount++;
            }
        }

        if (diffCount > 0)
        {
            Console.WriteLine($"  {sectionName}{propertyName}: Total differences: {diffCount} out of {a.Length} elements");
            return false;
        }
        return true;
    }

    private static bool DateTimeOffsetArrayEqual(DateTimeOffset[]? a, DateTimeOffset[]? b, string propertyName, string sectionName = "")
    {
        if (a == null || b == null) return a == b;
        if (a.Length != b.Length)
        {
            Console.WriteLine($"  {sectionName}{propertyName}: Length mismatch - JSON: {a.Length}, FlatBuffers: {b.Length}");
            return false;
        }
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return false;
        return true;
    }

    private static bool FloatsEqual(float a, float b)
    {
        // Handle NaN comparisons
        if (float.IsNaN(a) && float.IsNaN(b)) return true;
        if (float.IsNaN(a) || float.IsNaN(b)) return false;

        // Handle infinity comparisons
        if (float.IsInfinity(a) && float.IsInfinity(b)) return true;
        if (float.IsInfinity(a) || float.IsInfinity(b)) return false;

        // Compare with tolerance
        return Math.Abs(a - b) <= FloatTolerance;
    }

    private static bool NullableFloatEqual(float? a, float? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        return FloatsEqual(a.Value, b.Value);
    }
}
