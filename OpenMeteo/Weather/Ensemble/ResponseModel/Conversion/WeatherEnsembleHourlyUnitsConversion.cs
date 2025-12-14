using openmeteo_sdk;
using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Ensemble.Units;
using System.Collections.Generic;

namespace OpenMeteo.Weather.Ensemble.ResponseModel.Conversion;

internal static class WeatherEnsembleHourlyUnitsConversion
{
    public static WeatherEnsembleHourlyUnits? ConvertHourlyUnits(VariablesWithTime? fbHourly, WeatherEnsembleOptions? ensembleOptions)
    {
        if (fbHourly == null) return null;
        if (ensembleOptions?.Hourly == null || ensembleOptions.Hourly.Count == 0) return null;

        var hourlyUnits = new WeatherEnsembleHourlyUnits
        {
            Time = "iso8601",
            AdditionalData = new Dictionary<string, object>()
        };

        // Extract units for all variables
        for (int i = 0; i < fbHourly.Value.VariablesLength; i++)
        {
            var variable = fbHourly.Value.Variables(i);
            if (variable == null) continue;

            var variableName = i < ensembleOptions.Hourly.Parameter.Count
                ? ensembleOptions.Hourly.Parameter[i].ToString()
                : $"variable_{i}";

            hourlyUnits.AdditionalData[variableName] = variable.Value.Unit;
        }

        return hourlyUnits;
    }
}
