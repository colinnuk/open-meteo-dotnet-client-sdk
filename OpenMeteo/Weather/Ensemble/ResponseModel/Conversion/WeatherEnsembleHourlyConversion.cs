using openmeteo_sdk;
using OpenMeteo.Helpers;
using OpenMeteo.Weather.Ensemble.Options;
using System.Collections.Generic;
using System.Text.Json;
using OpenMeteo.Weather.Utilities;

namespace OpenMeteo.Weather.Ensemble.ResponseModel.Conversion;

internal static class WeatherEnsembleHourlyConversion
{
    public static WeatherEnsembleHourly? ConvertHourly(VariablesWithTime? fbHourly, WeatherEnsembleOptions? ensembleOptions)
    {
        if (fbHourly == null) return null;
        if (ensembleOptions?.Hourly == null || ensembleOptions.Hourly.Count == 0) return null;

        var hourly = new WeatherEnsembleHourly
        {
            Time = WeatherConversionHelpers.BuildTimeStringArray(fbHourly),
            AdditionalData = new Dictionary<string, object>()
        };

        // Convert all variables to the AdditionalData dictionary
        for (int i = 0; i < fbHourly.Value.VariablesLength; i++)
        {
            var variable = fbHourly.Value.Variables(i);
            if (variable == null) continue;

            var values = variable.Value.GetValuesArray();
            if (values != null && values.Length > 0)
            {
                // Get the variable name - for ensemble data, this will include the member suffix
                var variableName = i < ensembleOptions.Hourly.Parameter.Count 
                    ? ensembleOptions.Hourly.Parameter[i].ToString() 
                    : $"variable_{i}";
                
                // Store as JsonElement to match JSON deserialization behavior
                var jsonArray = JsonSerializer.SerializeToElement(values.ToNullableFloatArray());
                hourly.AdditionalData[variableName] = jsonArray;
            }
        }

        return hourly;
    }
}
