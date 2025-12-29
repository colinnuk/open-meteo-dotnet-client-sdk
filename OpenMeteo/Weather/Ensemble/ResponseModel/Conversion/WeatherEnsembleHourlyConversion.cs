using openmeteo_sdk;
using OpenMeteo.Weather.Ensemble.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenMeteo.Weather.Utilities;

namespace OpenMeteo.Weather.Ensemble.ResponseModel.Conversion;

internal static class WeatherEnsembleHourlyConversion
{
    public static EnsembleHourly? ConvertHourly(VariablesWithTime? fbHourly, WeatherEnsembleOptions? ensembleOptions)
    {
        if (fbHourly == null) return null;
        if (ensembleOptions?.Hourly == null || ensembleOptions.Hourly.Count == 0) return null;

        var hourly = new EnsembleHourly
        {
            Time = WeatherConversionHelpers.BuildTimeStringArray(fbHourly)?
                .Select(s => DateTimeOffset.Parse(s)).ToArray()
        };

        var floatMembers = new Dictionary<string, Dictionary<int, float?[]?>>();
        var intMembers = new Dictionary<string, Dictionary<int, int?[]?>>();

        EnsembleConversionHelpers.ProcessVariables(fbHourly.Value, floatMembers, intMembers);
        EnsembleConversionHelpers.AssignProperties(hourly, floatMembers, intMembers);

        return hourly;
    }
}
