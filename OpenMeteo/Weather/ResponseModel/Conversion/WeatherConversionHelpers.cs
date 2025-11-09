using System;
using openmeteo_sdk;

namespace OpenMeteo.Weather.ResponseModel.Conversion
{
    internal static class WeatherConversionHelpers
    {
        public static int GetTimeCount(VariablesWithTime? variablesWithTime)
        {
            if (variablesWithTime?.VariablesLength > 0)
            {
                var variable = variablesWithTime?.Variables(0);
                if (variable.HasValue)
                {
                    return variable.Value.ValuesLength;
                }
            }
            return 0;
        }

        public static string[]? BuildTimeArray(VariablesWithTime? variablesWithTime)
        {
            if (variablesWithTime == null || variablesWithTime.Value.Time <= 0)
                return null;
            int timeCount = GetTimeCount(variablesWithTime);
            var result = new string[timeCount];
            for (int i = 0; i < timeCount; i++)
            {
                result[i] = ConvertUnixToIso8601(variablesWithTime.Value.Time + (i * variablesWithTime.Value.Interval));
            }
            return result;
        }

        public static string ConvertUnixToIso8601(long? epochSeconds)
        {
            if (epochSeconds == null) return string.Empty;
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(epochSeconds.Value).UtcDateTime;
            return dateTime.ToString("yyyy-MM-dd'T'HH:mm");
        }
    }
}
