using openmeteo_sdk;
using OpenMeteo.Helpers;
using System.Linq;
using System;
using OpenMeteo.Weather.Forecast.Options;
using OpenMeteo.Weather.Forecast.ResponseModel;
using OpenMeteo.Weather.Utilities;

namespace OpenMeteo.Weather.Forecast.ResponseModel.Conversion
{
    internal static class DailyConversion
    {
        public static Daily? ConvertDaily(VariablesWithTime? fbDaily, IWeatherForecastOptions? forecastOptions)
        {
            if (fbDaily == null) return null;
            if (forecastOptions?.Daily == null || forecastOptions.Daily.Count == 0) return null;

            var daily = new Daily
            {
                Time = WeatherConversionHelpers.BuildTimeArray(fbDaily, forecastOptions.Timezone)?
                    .Select(dto => DateOnly.FromDateTime(dto.DateTime)).ToArray()
            };
            for (int i = 0; i < fbDaily?.VariablesLength && i < forecastOptions.Daily.Parameter.Count; i++)
            {
                MapDailyVariableByParameter(daily, forecastOptions.Daily.Parameter[i], fbDaily?.Variables(i));
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
