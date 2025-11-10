using openmeteo_sdk;
using OpenMeteo.Helpers;
using OpenMeteo.Weather.Options;

namespace OpenMeteo.Weather.ResponseModel.Conversion
{
    internal static class HourlyConversion
    {
        public static Hourly? ConvertHourly(VariablesWithTime? fbHourly, WeatherForecastOptions? forecastOptions)
        {
            if (fbHourly == null) return null;
            if (forecastOptions?.Hourly == null || forecastOptions.Hourly.Count == 0) return null;

            var hourly = new Hourly
            {
                Time = WeatherConversionHelpers.BuildTimeArray(fbHourly, forecastOptions.Timezone)
            };

            for (int i =0; i < fbHourly?.VariablesLength && i < forecastOptions.Hourly.Parameter.Count; i++)
            {
                MapHourlyVariableByParameter(hourly, forecastOptions.Hourly.Parameter[i], fbHourly?.Variables(i));
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
