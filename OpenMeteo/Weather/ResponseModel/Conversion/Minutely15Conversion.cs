using openmeteo_sdk;
using OpenMeteo.Helpers;
using OpenMeteo.Weather.Options;

namespace OpenMeteo.Weather.ResponseModel.Conversion
{
    internal static class Minutely15Conversion
    {
        public static Minutely15? ConvertMinutely15(VariablesWithTime? fbMinutely15, Minutely15Options? requestedOptions)
        {
            if (fbMinutely15 == null) return null;
            if (requestedOptions == null || requestedOptions.Count ==0) return null;
            var minutely = new Minutely15
            {
                time = WeatherConversionHelpers.BuildTimeArray(fbMinutely15)
            };
            for (int i =0; i < fbMinutely15?.VariablesLength && i < requestedOptions.Parameter.Count; i++)
            {
                MapMinutely15VariableByParameter(minutely, requestedOptions.Parameter[i], fbMinutely15?.Variables(i));
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
