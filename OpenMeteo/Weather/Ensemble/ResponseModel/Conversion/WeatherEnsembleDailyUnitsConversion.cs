using openmeteo_sdk;
using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Ensemble.Units;

namespace OpenMeteo.Weather.Ensemble.ResponseModel.Conversion;

internal static class WeatherEnsembleDailyUnitsConversion
{
    public static WeatherEnsembleDailyUnits? ConvertDailyUnits(VariablesWithTime? fbDaily, WeatherEnsembleOptions? ensembleOptions)
    {
        if (fbDaily == null) return null;
        if (ensembleOptions?.Daily == null || ensembleOptions.Daily.Count == 0) return null;

        var dailyUnits = new WeatherEnsembleDailyUnits
        {
            Time = EnsembleConversionConstants.Iso8601TimeFormat,
            AdditionalData = []
        };

        // Extract units for all variables
        for (int i = 0; i < fbDaily.Value.VariablesLength; i++)
        {
            var variable = fbDaily.Value.Variables(i);
            if (variable == null) continue;

            var variableName = i < ensembleOptions.Daily.Parameter.Count
                ? ensembleOptions.Daily.Parameter[i].ToString()
                : string.Format(EnsembleConversionConstants.VariableNameFormat, i);

            dailyUnits.AdditionalData[variableName] = variable.Value.Unit;
        }

        return dailyUnits;
    }
}
