using openmeteo_sdk;
using OpenMeteo.Helpers;
using OpenMeteo.Weather.Options;

namespace OpenMeteo.Weather.ResponseModel.Conversion
{
    internal static class DailyConversion
    {
        public static Daily? ConvertDaily(VariablesWithTime? fbDaily, DailyOptions? requestedOptions)
        {
            if (fbDaily == null) return null;
            if (requestedOptions == null || requestedOptions.Count == 0) return null;

            var daily = new Daily
            {
                Time = WeatherConversionHelpers.BuildTimeArray(fbDaily)
            };
            for (int i = 0; i < fbDaily?.VariablesLength && i < requestedOptions.Parameter.Count; i++)
            {
                MapDailyVariableByParameter(daily, requestedOptions.Parameter[i], fbDaily?.Variables(i));
            }
            return daily;
        }

        private static void MapDailyVariableByParameter(Daily daily, DailyOptionsParameter parameter, VariableWithValues? variable)
        {
            if (!variable.HasValue) return;
            PropertyMappingHelper.MapVariableToProperty(daily, parameter.ToString(), variable.Value);
        }
    }
}
