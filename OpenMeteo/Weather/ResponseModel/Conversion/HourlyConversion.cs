using openmeteo_sdk;
using OpenMeteo.Helpers;
using OpenMeteo.Weather.Options;

namespace OpenMeteo.Weather.ResponseModel.Conversion
{
    internal static class HourlyConversion
    {
        public static Hourly? ConvertHourly(VariablesWithTime? fbHourly, HourlyOptions? requestedOptions)
        {
            if (fbHourly == null) return null;
            if (requestedOptions == null || requestedOptions.Count == 0) return null;

            var hourly = new Hourly
            {
                Time = WeatherConversionHelpers.BuildTimeArray(fbHourly)
            };

            for (int i = 0; i < fbHourly?.VariablesLength && i < requestedOptions.Parameter.Count; i++)
            {
                MapHourlyVariableByParameter(hourly, requestedOptions.Parameter[i], fbHourly?.Variables(i));
            }

            return hourly;
        }

        private static void MapHourlyVariableByParameter(Hourly hourly, HourlyOptionsParameter parameter, VariableWithValues? variable)
        {
            if (!variable.HasValue) return;
            PropertyMappingHelper.MapVariableToProperty(hourly, parameter.ToString(), variable.Value);
        }
    }
}
