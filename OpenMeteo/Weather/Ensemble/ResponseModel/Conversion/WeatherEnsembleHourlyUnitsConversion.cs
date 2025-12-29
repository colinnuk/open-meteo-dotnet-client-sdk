using openmeteo_sdk;
using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Ensemble.Units;

namespace OpenMeteo.Weather.Ensemble.ResponseModel.Conversion;

internal static class WeatherEnsembleHourlyUnitsConversion
{
    public static WeatherEnsembleHourlyUnits? ConvertHourlyUnits(VariablesWithTime? fbHourly, WeatherEnsembleOptions? ensembleOptions)
    {
        if (fbHourly == null) return null;
        if (ensembleOptions?.Hourly == null || ensembleOptions.Hourly.Count == 0) return null;

        var hourlyUnits = new WeatherEnsembleHourlyUnits
        {
            Time = EnsembleConversionConstants.Iso8601TimeFormat,
            AdditionalData = []
        };

        // Extract units for all variables
        for (int i = 0; i < fbHourly.Value.VariablesLength; i++)
        {
            var variable = fbHourly.Value.Variables(i);
            if (variable == null) continue;

            var variableName = i < ensembleOptions.Hourly.Parameter.Count
                ? ensembleOptions.Hourly.Parameter[i].ToString()
                : string.Format(EnsembleConversionConstants.VariableNameFormat, i);

            hourlyUnits.AdditionalData[variableName] = variable.Value.Unit;
        }

        return hourlyUnits;
    }
}
