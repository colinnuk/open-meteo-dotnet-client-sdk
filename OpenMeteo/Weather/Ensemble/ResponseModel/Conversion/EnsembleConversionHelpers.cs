using openmeteo_sdk;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System;

namespace OpenMeteo.Weather.Ensemble.ResponseModel.Conversion;

internal static class EnsembleConversionHelpers
{
    public static string BuildPropertyName(VariableWithValues variable)
    {
        var variableName = variable.Variable.ToString().ToLower();
        var aggregation = variable.Aggregation.ToString().ToLower();
        var altitude = variable.Altitude;
        var pressureLevel = variable.PressureLevel;
        var depth = variable.Depth;
        var depthTo = variable.DepthTo;

        var propertyName = variableName;

        if (altitude > 0)
        {
            propertyName += $"{EnsembleConversionConstants.UnderscoreSeparator}{altitude}{EnsembleConversionConstants.UnitSuffixMeters}";
        }

        if (pressureLevel > 0)
        {
            propertyName += $"{EnsembleConversionConstants.UnderscoreSeparator}{pressureLevel}{EnsembleConversionConstants.UnitSuffixHectoPascals}";
        }

        if (depth > 0 || depthTo > 0)
        {
            if (depthTo > 0)
            {
                propertyName += $"{EnsembleConversionConstants.UnderscoreSeparator}{depth}{EnsembleConversionConstants.UnderscoreSeparator}{EnsembleConversionConstants.ToSeparator}{EnsembleConversionConstants.UnderscoreSeparator}{depthTo}{EnsembleConversionConstants.UnitSuffixCentimeters}";
            }
            else
            {
                propertyName += $"{EnsembleConversionConstants.UnderscoreSeparator}{depth}{EnsembleConversionConstants.UnitSuffixCentimeters}";
            }
        }

        if (aggregation != EnsembleConversionConstants.AggregationNone && !string.IsNullOrEmpty(aggregation))
        {
            propertyName += $"{EnsembleConversionConstants.UnderscoreSeparator}{aggregation}";
        }

        return ToPascalCase(propertyName);
    }

    public static string ToPascalCase(string variableName)
    {
        var parts = variableName.Split(EnsembleConversionConstants.UnderscoreSeparator);
        var result = new StringBuilder();
        
        for (int i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            if (part.Length == 0) continue;
            
            result.Append(char.ToUpper(part[0]));
            if (part.Length > 1)
            {
                result.Append(part.AsSpan(1));
            }
            
            if (i < parts.Length - 1)
            {
                var nextPart = parts[i + 1];
                if (nextPart.Length > 0 && ShouldInsertUnderscore(nextPart))
                {
                    result.Append(EnsembleConversionConstants.UnderscoreSeparator);
                }
            }
        }
        
        return result.ToString();
    }

    private static bool ShouldInsertUnderscore(string nextPart)
    {
        return char.IsDigit(nextPart[0]) || 
               nextPart == EnsembleConversionConstants.AggregationMean || 
               nextPart == EnsembleConversionConstants.AggregationMin || 
               nextPart == EnsembleConversionConstants.AggregationMax || 
               nextPart == EnsembleConversionConstants.AggregationSum || 
               nextPart == EnsembleConversionConstants.AggregationDominant || 
               nextPart == EnsembleConversionConstants.AggregationHours ||
               nextPart == EnsembleConversionConstants.ToSeparator || 
               nextPart.EndsWith(EnsembleConversionConstants.UnitSuffixHectoPascals) || 
               nextPart.EndsWith(EnsembleConversionConstants.UnitSuffixMeters) || 
               nextPart.EndsWith(EnsembleConversionConstants.UnitSuffixCentimeters);
    }

    public static void AssignProperties<T>(
        T target,
        Dictionary<string, Dictionary<int, float?[]?>> floatMembers,
        Dictionary<string, Dictionary<int, int?[]?>> intMembers)
    {
        var type = typeof(T);
        
        foreach (var (propertyName, members) in floatMembers)
        {
            var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property != null && property.PropertyType == typeof(Dictionary<int, float?[]?>))
            {
                property.SetValue(target, members);
            }
        }
        
        foreach (var (propertyName, members) in intMembers)
        {
            var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property != null && property.PropertyType == typeof(Dictionary<int, int?[]?>))
            {
                property.SetValue(target, members);
            }
        }
    }

    public static void ProcessVariables(
        VariablesWithTime fbData,
        Dictionary<string, Dictionary<int, float?[]?>> floatMembers,
        Dictionary<string, Dictionary<int, int?[]?>> intMembers)
    {
        for (int i = 0; i < fbData.VariablesLength; i++)
        {
            var variable = fbData.Variables(i);
            if (variable == null) continue;

            int memberNumber = variable.Value.EnsembleMember;
            string propertyName = BuildPropertyName(variable.Value);

            var int64Values = variable.Value.GetValuesInt64Array();
            bool isIntVariable = int64Values != null && int64Values.Length > 0;

            if (isIntVariable)
            {
                var intValues = EnsembleMemberParser.ToNullableIntArray(variable.Value);
                if (!intMembers.ContainsKey(propertyName))
                    intMembers[propertyName] = [];
                intMembers[propertyName][memberNumber] = intValues;
            }
            else
            {
                var floatValues = EnsembleMemberParser.ToNullableFloatArray(variable.Value);
                if (!floatMembers.ContainsKey(propertyName))
                    floatMembers[propertyName] = [];
                floatMembers[propertyName][memberNumber] = floatValues;
            }
        }
    }
}
