using openmeteo_sdk;
using OpenMeteo.Helpers;
using OpenMeteo.Weather.Options;

namespace OpenMeteo.Weather.ResponseModel.Conversion
{
    internal static class CurrentConversion
    {
        public static Current? ConvertCurrent(VariablesWithTime? fbCurrent, CurrentOptions? requestedOptions)
        {
            if (fbCurrent == null) return null;
            if (requestedOptions == null || requestedOptions.Count == 0) return null;

            var current = new Current
            {
                Time = WeatherConversionHelpers.ConvertUnixToIso8601(fbCurrent?.Time),
                Interval = fbCurrent?.Interval
            };

            for (int i = 0; i < fbCurrent?.VariablesLength && i < requestedOptions.Parameter.Count; i++)
            {
                MapCurrentVariableByParameter(current, requestedOptions.Parameter[i], fbCurrent?.Variables(i));
            }

            return current;
        }

        private static void MapCurrentVariableByParameter(Current current, CurrentOptionsParameter parameter, VariableWithValues? variable)
        {
            if (!variable.HasValue || variable.Value.ValuesLength == 0) return;
            PropertyMappingHelper.MapVariableToProperty(current, parameter.ToString(), variable.Value);
        }
    }
}
