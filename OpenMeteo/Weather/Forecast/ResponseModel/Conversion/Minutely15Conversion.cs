using openmeteo_sdk;
using OpenMeteo.Helpers;
using OpenMeteo.Weather.Forecast.Options;
using OpenMeteo.Weather.Forecast.ResponseModel;
using OpenMeteo.Weather.Utilities;

namespace OpenMeteo.Weather.Forecast.ResponseModel.Conversion
{
    internal static class Minutely15Conversion
    {
        public static Minutely15? ConvertMinutely15(VariablesWithTime? fbMinutely15, WeatherForecastOptions? forecastOptions)
        {
            if (fbMinutely15 == null) return null;
            if (forecastOptions?.Minutely_15 == null || forecastOptions.Minutely_15.Count ==0) return null;
            var minutely = new Minutely15
            {
                Time = WeatherConversionHelpers.BuildTimeArray(fbMinutely15, forecastOptions.Timezone)
            };
            for (int i =0; i < fbMinutely15?.VariablesLength && i < forecastOptions.Minutely_15.Parameter.Count; i++)
            {
                MapMinutely15VariableByParameter(minutely, forecastOptions.Minutely_15.Parameter[i], fbMinutely15?.Variables(i));
            }
            return minutely;
        }

        private static void MapMinutely15VariableByParameter(Minutely15 minutely, Minutely15OptionsParameter parameter, VariableWithValues? variable)
        {
            if (!variable.HasValue) return;
            PropertyMappingHelper.MapVariableToProperty(minutely, parameter.ToString(), variable.Value);
        }
    }
}
