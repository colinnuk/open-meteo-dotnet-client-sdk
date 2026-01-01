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

        var floatMembers = new Dictionary<string, Dictionary<int, float?[]?>>();
        var intMembers = new Dictionary<string, Dictionary<int, int?[]?>>();

        EnsembleConversionHelpers.ProcessVariables(fbDaily.Value, floatMembers, intMembers);
        EnsembleConversionHelpers.AssignProperties(daily, floatMembers, intMembers);

        return daily;
    }
}
