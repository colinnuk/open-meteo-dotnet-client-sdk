using openmeteo_sdk;
using OpenMeteo.Weather.Ensemble.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenMeteo.Weather.Utilities;

namespace OpenMeteo.Weather.Ensemble.ResponseModel.Conversion;

internal static class WeatherEnsembleDailyConversion
{
    public static EnsembleDaily? ConvertDaily(VariablesWithTime? fbDaily, WeatherEnsembleOptions? ensembleOptions)
    {
        if (fbDaily == null) return null;
        if (ensembleOptions?.Daily == null || ensembleOptions.Daily.Count == 0) return null;

        var daily = new EnsembleDaily
        {
            Time = WeatherConversionHelpers.BuildTimeStringArray(fbDaily)?
                .Select(str => DateOnly.Parse(str)).ToArray()
        };

        // Temporary dictionaries to accumulate data before assigning to properties
        var floatMembers = new Dictionary<string, Dictionary<int, float?[]?>>();
        var intMembers = new Dictionary<string, Dictionary<int, int?[]?>>();

        // Convert all variables and group by ensemble member
        for (int i = 0; i < fbDaily.Value.VariablesLength; i++)
        {
            var variable = fbDaily.Value.Variables(i);
            if (variable == null) continue;

            // Get the member number from the FlatBuffer field
            int memberNumber = variable.Value.EnsembleMember;
            
            // Build the property name from the variable's metadata
            string propertyName = BuildPropertyName(variable.Value);

            // Determine if this is an integer or float variable
            var int64Values = variable.Value.GetValuesInt64Array();
            bool isIntVariable = int64Values != null && int64Values.Length > 0;

            if (isIntVariable)
            {
                var intValues = EnsembleMemberParser.ToNullableIntArray(variable.Value);
                if (!intMembers.ContainsKey(propertyName))
                    intMembers[propertyName] = new Dictionary<int, int?[]>();
                intMembers[propertyName][memberNumber] = intValues;
            }
            else
            {
                var floatValues = EnsembleMemberParser.ToNullableFloatArray(variable.Value);
                if (!floatMembers.ContainsKey(propertyName))
                    floatMembers[propertyName] = new Dictionary<int, float?[]>();
                floatMembers[propertyName][memberNumber] = floatValues;
            }
        }

        // Assign to properties using reflection
        AssignProperties(daily, floatMembers, intMembers);

        return daily;
    }

    private static string BuildPropertyName(VariableWithValues variable)
    {
        var variableName = variable.Variable.ToString().ToLower();
        var aggregation = variable.Aggregation.ToString().ToLower();
        var altitude = variable.Altitude;
        var pressureLevel = variable.PressureLevel;
        var depth = variable.Depth;
        var depthTo = variable.DepthTo;

        // Start with the base variable name
        var propertyName = variableName;

        // Add altitude if present (e.g., temperature_2m)
        if (altitude > 0)
        {
            propertyName += $"_{altitude}m";
        }

        // Add pressure level if present (e.g., temperature_500hPa)
        if (pressureLevel > 0)
        {
            propertyName += $"_{pressureLevel}hPa";
        }

        // Add depth range if present (e.g., soil_temperature_0_to_10cm)
        if (depth > 0 || depthTo > 0)
        {
            if (depthTo > 0)
            {
                propertyName += $"_{depth}_to_{depthTo}cm";
            }
            else
            {
                propertyName += $"_{depth}cm";
            }
        }

        // Add aggregation suffix if not "none" (e.g., temperature_2m_max)
        if (aggregation != "none" && !string.IsNullOrEmpty(aggregation))
        {
            propertyName += $"_{aggregation}";
        }

        return ToPascalCase(propertyName);
    }

    private static string ToPascalCase(string variableName)
    {
        // Convert snake_case to PascalCase while preserving numeric suffixes
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
                    nextPart == "to" || nextPart.EndsWith("hPa") || nextPart.EndsWith("m") || nextPart.EndsWith("cm")))
                {
                    result.Append('_');
                }
            }
        }
        
        return result.ToString();
    }

    private static void AssignProperties(
        EnsembleDaily daily,
        Dictionary<string, Dictionary<int, float?[]?>> floatMembers,
        Dictionary<string, Dictionary<int, int?[]?>> intMembers)
    {
        var type = typeof(EnsembleDaily);
        
        // Map float properties
        foreach (var (propertyName, members) in floatMembers)
        {
            var property = type.GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
            if (property != null && property.PropertyType == typeof(Dictionary<int, float?[]?>))
            {
                property.SetValue(daily, members);
            }
        }
        
        // Map int properties
        foreach (var (propertyName, members) in intMembers)
        {
            var property = type.GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
            if (property != null && property.PropertyType == typeof(Dictionary<int, int?[]?>))
            {
                property.SetValue(daily, members);
            }
        }
    }
}
