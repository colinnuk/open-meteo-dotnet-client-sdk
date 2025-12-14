using openmeteo_sdk;
using OpenMeteo.Helpers;
using OpenMeteo.Weather.Ensemble.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using OpenMeteo.Weather.Utilities;

namespace OpenMeteo.Weather.Ensemble.ResponseModel.Conversion;

internal static class WeatherEnsembleDailyConversion
{
    public static WeatherEnsembleDaily? ConvertDaily(VariablesWithTime? fbDaily, WeatherEnsembleOptions? ensembleOptions)
    {
        if (fbDaily == null) return null;
        if (ensembleOptions?.Daily == null || ensembleOptions.Daily.Count == 0) return null;

        var daily = new WeatherEnsembleDaily
        {
            Time = WeatherConversionHelpers.BuildTimeStringArray(fbDaily)?
                .Select(str => DateOnly.Parse(str)).ToArray(),
            AdditionalData = new Dictionary<string, object>()
        };

        // Convert all variables to the AdditionalData dictionary
        for (int i = 0; i < fbDaily.Value.VariablesLength; i++)
        {
            var variable = fbDaily.Value.Variables(i);
            if (variable == null) continue;

            var values = variable.Value.GetValuesArray();
            if (values != null && values.Length > 0)
            {
                // Get the variable name - for ensemble data, this will include the member suffix
                var variableName = i < ensembleOptions.Daily.Parameter.Count
                    ? ensembleOptions.Daily.Parameter[i].ToString()
                    : $"variable_{i}";

                // Store as JsonElement to match JSON deserialization behavior
                var jsonArray = JsonSerializer.SerializeToElement(values.ToNullableFloatArray());
                daily.AdditionalData[variableName] = jsonArray;
            }
        }

        return daily;
    }
}
