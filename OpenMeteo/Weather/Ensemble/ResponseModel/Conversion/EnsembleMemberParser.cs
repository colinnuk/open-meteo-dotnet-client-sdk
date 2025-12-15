using System.Collections.Generic;
using System.Text.RegularExpressions;
using openmeteo_sdk;
using OpenMeteo.Helpers;

namespace OpenMeteo.Weather.Ensemble.ResponseModel.Conversion;

/// <summary>
/// Helper for parsing ensemble member data from variable names
/// </summary>
public static class EnsembleMemberParser
{
    private static readonly Regex _memberPattern = new(@"^(.+)_member(\d+)$", RegexOptions.Compiled);

    /// <summary>
    /// Parses a variable name to determine if it's an ensemble member or base variable (member 0)
    /// </summary>
    /// <param name="variableName">The variable name to parse</param>
    /// <param name="baseName">The base variable name without member suffix</param>
    /// <param name="memberNumber">The member number (0 for base variable, 1+ for members)</param>
    /// <returns>True if the variable is valid</returns>
    public static bool TryParseMember(string variableName, out string baseName, out int memberNumber)
    {
        var match = _memberPattern.Match(variableName);
        if (match.Success)
        {
            // This is a member variable like "temperature_2m_member01"
            baseName = match.Groups[1].Value;
            memberNumber = int.Parse(match.Groups[2].Value);
            return true;
        }

        // This is the base variable (member 0) like "temperature_2m"
        baseName = variableName;
        memberNumber = 0;
        return true;
    }

    /// <summary>
    /// Converts a VariableWithValues to a nullable float array
    /// </summary>
    public static float?[]? ToNullableFloatArray(VariableWithValues variable)
    {
        var values = variable.GetValuesArray();
        return values?.ToNullableFloatArray();
    }

    /// <summary>
    /// Converts a VariableWithValues to a nullable int array
    /// </summary>
    public static int?[]? ToNullableIntArray(VariableWithValues variable)
    {
        var int64Values = variable.GetValuesInt64Array();
        if (int64Values != null && int64Values.Length > 0)
        {
            return int64Values.ToNullableIntArray();
        }
        
        var floatValues = variable.GetValuesArray();
        if (floatValues != null && floatValues.Length > 0)
        {
            var intArray = new int?[floatValues.Length];
            for (int i = 0; i < floatValues.Length; i++)
            {
                intArray[i] = float.IsNaN(floatValues[i]) ? null : (int?)System.Math.Round(floatValues[i]);
            }
            return intArray;
        }
        
        return null;
    }
}
