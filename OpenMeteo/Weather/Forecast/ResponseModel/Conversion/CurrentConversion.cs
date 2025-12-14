using openmeteo_sdk;
using OpenMeteo.Helpers;
using OpenMeteo.Weather.Forecast.Options;
using OpenMeteo.Weather.Forecast.ResponseModel;

namespace OpenMeteo.Weather.Forecast.ResponseModel.Conversion
{
    internal static class CurrentConversion
    {
        public static Current? ConvertCurrent(VariablesWithTime? fbCurrent, WeatherForecastOptions? forecastOptions)
        {
            if (fbCurrent == null) return null;
            if (forecastOptions?.Current == null || forecastOptions.Current.Count == 0) return null;

            var current = new Current
            {
                Time = WeatherConversionHelpers.ConvertUnixToDateTimeOffset(fbCurrent.Value.Time, forecastOptions.Timezone),
                Interval = fbCurrent?.Interval
            };

            for (int i = 0; i < fbCurrent?.VariablesLength && i < forecastOptions.Current.Parameter.Count; i++)
            {
                MapCurrentVariableByParameter(current, forecastOptions.Current.Parameter[i], fbCurrent?.Variables(i));
            }

            return current;
        }

        private static void MapCurrentVariableByParameter(Current current, CurrentOptionsParameter parameter, VariableWithValues? variable)
        {
            if (!variable.HasValue) return;
            PropertyMappingHelper.MapVariableToProperty(current, parameter.ToString(), variable.Value);
        }
    }
}
