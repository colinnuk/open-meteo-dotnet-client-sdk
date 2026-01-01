using System;
using openmeteo_sdk;

namespace OpenMeteo.Weather.Utilities;

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

    public static DateTimeOffset[]? BuildTimeArray(VariablesWithTime? variablesWithTime, string timezoneId)
    {
        if (variablesWithTime == null || variablesWithTime.Value.Time <= 0)
            return null;
        int timeCount = GetTimeCount(variablesWithTime);
        var result = new DateTimeOffset[timeCount];
        for (int i = 0; i < timeCount; i++)
        {
            result[i] = ConvertUnixToDateTimeOffset(variablesWithTime.Value.Time + (i * variablesWithTime.Value.Interval), timezoneId);
        }
        return result;
    }

    public static string[]? BuildTimeStringArray(VariablesWithTime? variablesWithTime)
    {
        if (variablesWithTime == null || variablesWithTime.Value.Time <= 0)
            return null;
        int timeCount = GetTimeCount(variablesWithTime);
        var result = new string[timeCount];
        for (int i = 0; i < timeCount; i++)
        {
            var unixTime = variablesWithTime.Value.Time + (i * variablesWithTime.Value.Interval);
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTime);
            // Format based on interval - hourly vs daily
            result[i] = variablesWithTime.Value.Interval >= 86400
                ? dateTime.ToString("yyyy-MM-dd")
                : dateTime.ToString("yyyy-MM-ddTHH:mm");
        }
        return result;
    }

    public static DateTimeOffset ConvertUnixToDateTimeOffset(long epochSeconds, string timezoneId)
    {
        var utcDateTime = DateTimeOffset.FromUnixTimeSeconds(epochSeconds);
        var tzInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        var localDateTime = TimeZoneInfo.ConvertTime(utcDateTime, tzInfo);
        var offset = tzInfo.GetUtcOffset(localDateTime);
        return new DateTimeOffset(localDateTime.DateTime, offset);
    }
}
