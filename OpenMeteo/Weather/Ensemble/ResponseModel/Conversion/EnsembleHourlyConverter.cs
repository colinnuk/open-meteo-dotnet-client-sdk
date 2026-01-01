using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace OpenMeteo.Weather.Ensemble.ResponseModel.Conversion;

/// <summary>
/// Converts internal hourly DTO to public API model
/// </summary>
internal static class EnsembleHourlyConverter
{
    public static EnsembleHourly? ToPublicModel(WeatherEnsembleHourlyDto? dto, string? timezone)
    {
        if (dto == null) return null;

        var hourly = new EnsembleHourly
        {
            Time = dto.Time?.Select(t => DateTimeOffset.Parse(t)).ToArray()
        };

        if (dto.AdditionalData == null || dto.AdditionalData.Count == 0)
            return hourly;

        // Temporary dictionaries to accumulate member data
        var floatMembers = new Dictionary<string, Dictionary<int, float?[]?>>();
        var intMembers = new Dictionary<string, Dictionary<int, int?[]?>>();

        // Parse all additional data
        foreach (var (key, value) in dto.AdditionalData)
        {
            if (value is not JsonElement jsonElement || jsonElement.ValueKind != JsonValueKind.Array)
                continue;

            // Parse the member pattern
            if (!EnsembleMemberParser.TryParseMember(key, out string baseName, out int memberNumber))
                continue;

            var arrayLength = jsonElement.GetArrayLength();
            if (arrayLength == 0) continue;

            // Determine if this is int or float data
            var firstElement = jsonElement[0];
            bool isInt = firstElement.ValueKind == JsonValueKind.Number && firstElement.TryGetInt32(out _);

            if (isInt)
            {
                var intArray = ParseIntArray(jsonElement);
                if (!intMembers.ContainsKey(baseName))
                    intMembers[baseName] = new Dictionary<int, int?[]?>();
                intMembers[baseName][memberNumber] = intArray;
            }
            else
            {
                var floatArray = ParseFloatArray(jsonElement);
                if (!floatMembers.ContainsKey(baseName))
                    floatMembers[baseName] = new Dictionary<int, float?[]?>();
                floatMembers[baseName][memberNumber] = floatArray;
            }
        }

        // Assign to public model properties - using reflection for maintainability
        AssignProperties(hourly, floatMembers, intMembers);

        return hourly;
    }

    private static void AssignProperties(
        EnsembleHourly hourly,
        Dictionary<string, Dictionary<int, float?[]?>> floatMembers,
        Dictionary<string, Dictionary<int, int?[]?>> intMembers)
    {
        var type = typeof(EnsembleHourly);
        
        // Map float properties
        foreach (var (baseName, members) in floatMembers)
        {
            var propertyName = ToPropertyName(baseName);
            var property = type.GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
            if (property != null && property.PropertyType == typeof(Dictionary<int, float?[]?>))
            {
                property.SetValue(hourly, members);
            }
        }
        
        // Map int properties
        foreach (var (baseName, members) in intMembers)
        {
            var propertyName = ToPropertyName(baseName);
            var property = type.GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
            if (property != null && property.PropertyType == typeof(Dictionary<int, int?[]?>))
            {
                property.SetValue(hourly, members);
            }
        }
    }

    private static string ToPropertyName(string variableName)
    {
        // Convert snake_case to PascalCase while preserving numeric suffixes
        // e.g., "temperature_2m" -> "Temperature_2m"
        // e.g., "windspeed_10m" -> "Windspeed_10m"
        // e.g., "cloudcover_mean" -> "Cloudcover_mean"
        // e.g., "soil_temperature_0_to_10cm" -> "SoilTemperature_0_to_10cm"
        
        var parts = variableName.Split('_');
        var result = new System.Text.StringBuilder();
        
        for (int i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            if (part.Length == 0) continue;
            
            // Capitalize first letter
            result.Append(char.ToUpper(part[0]));
            if (part.Length > 1)
            {
                result.Append(part.Substring(1));
            }
            
            // Add underscore if next part starts with a digit or is a known suffix
            if (i < parts.Length - 1)
            {
                var nextPart = parts[i + 1];
                if (nextPart.Length > 0 && (char.IsDigit(nextPart[0]) || 
                    nextPart == "mean" || nextPart == "min" || nextPart == "max" || 
                    nextPart == "sum" || nextPart == "dominant" || nextPart == "hours" ||
                    nextPart == "to" || nextPart.EndsWith("hPa")))
                {
                    result.Append('_');
                }
            }
        }
        
        return result.ToString();
    }

    private static float?[]? ParseFloatArray(JsonElement jsonElement)
    {
        var arrayLength = jsonElement.GetArrayLength();
        var result = new float?[arrayLength];
        int idx = 0;
        
        foreach (var element in jsonElement.EnumerateArray())
        {
            if (element.ValueKind == JsonValueKind.Number)
            {
                result[idx++] = (float)element.GetDouble();
            }
            else
            {
                result[idx++] = null;
            }
        }
        
        return result;
    }

    private static int?[]? ParseIntArray(JsonElement jsonElement)
    {
        var arrayLength = jsonElement.GetArrayLength();
        var result = new int?[arrayLength];
        int idx = 0;
        
        foreach (var element in jsonElement.EnumerateArray())
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out var val))
            {
                result[idx++] = val;
            }
            else
            {
                result[idx++] = null;
            }
        }
        
        return result;
    }
}
