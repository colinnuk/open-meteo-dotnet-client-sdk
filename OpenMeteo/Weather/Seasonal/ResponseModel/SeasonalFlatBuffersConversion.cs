using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using openmeteo_sdk;

namespace OpenMeteo.Weather.Seasonal.ResponseModel;

/// <summary>
/// Converts FlatBuffers responses of the Seasonal Forecast API into <see cref="WeatherSeasonal"/>.
/// The resulting variable names, units and timestamps match what the JSON format produces.
/// </summary>
internal static class SeasonalFlatBuffersConversion
{
    public static WeatherSeasonal Convert(WeatherApiResponse response, string? requestedTimezone)
    {
        int utcOffset = response.UtcOffsetSeconds;

        return new WeatherSeasonal
        {
            Latitude = response.Latitude,
            Longitude = response.Longitude,
            Elevation = response.Elevation,
            GenerationTime = response.GenerationTimeMilliseconds,
            UtcOffset = utcOffset,
            Timezone = string.IsNullOrEmpty(response.Timezone) ? requestedTimezone : response.Timezone,
            TimezoneAbbreviation = string.IsNullOrEmpty(response.TimezoneAbbreviation) ? requestedTimezone : response.TimezoneAbbreviation,
            Hourly = ConvertTimeSeries(response.Hourly, utcOffset),
            Daily = ConvertTimeSeries(response.Daily, utcOffset),
            Monthly = ConvertMonthly(response.Monthly)
        };
    }

    private static SeasonalSeries? ConvertTimeSeries(VariablesWithTime? data, int utcOffset)
    {
        if (data == null) return null;
        var fb = data.Value;

        var series = new SeasonalSeries();
        int count = fb.VariablesLength > 0 ? fb.Variables(0)!.Value.ValuesLength : 0;

        // Timestamps are UTC instants of the local wall clock time; adding the UTC offset gives the local time like the JSON format does
        series.Time = new DateTime[count];
        for (int i = 0; i < count; i++)
            series.Time[i] = DateTimeOffset.FromUnixTimeSeconds(fb.Time + (long)i * fb.Interval + utcOffset).DateTime;

        ReadVariables(series, fb.VariablesLength, fb.Variables);
        return series;
    }

    private static SeasonalSeries? ConvertMonthly(VariablesWithMonth? data)
    {
        if (data == null) return null;
        var fb = data.Value;

        var series = new SeasonalSeries { Time = new DateTime[fb.Count] };
        var first = new DateTime(fb.Year, fb.Month, 1);
        for (int i = 0; i < fb.Count; i++)
            series.Time[i] = first.AddMonths(i);

        ReadVariables(series, fb.VariablesLength, fb.Variables);
        return series;
    }

    private static void ReadVariables(SeasonalSeries series, int length, Func<int, VariableWithValues?> getVariable)
    {
        for (int i = 0; i < length; i++)
        {
            var variable = getVariable(i);
            if (variable == null) continue;

            string name = BuildVariableName(variable.Value);
            series.Variables[name] = ReadValues(variable.Value);
            series.Units[name] = UnitToString(variable.Value.Unit);
        }
    }

    private static float?[] ReadValues(VariableWithValues variable)
    {
        if (variable.ValuesLength == 0 && variable.ValuesInt64Length > 0)
        {
            var ints = new float?[variable.ValuesInt64Length];
            for (int i = 0; i < ints.Length; i++)
                ints[i] = variable.ValuesInt64(i);
            return ints;
        }

        // The API sends NaN for missing values; the JSON format sends null
        var values = new float?[variable.ValuesLength];
        for (int i = 0; i < values.Length; i++)
        {
            float value = variable.Values(i);
            values[i] = float.IsNaN(value) ? null : value;
        }
        return values;
    }

    /// <summary>
    /// Builds the API variable name, e.g. "temperature_2m_max", "soil_temperature_0_to_7cm" or "temperature_2m_max_member03"
    /// </summary>
    internal static string BuildVariableName(VariableWithValues variable)
    {
        var name = new StringBuilder(variable.Variable.ToString());

        // The API names apparent_temperature without a height, although it is derived for 2 m
        if (variable.Altitude > 0 && variable.Variable != Variable.apparent_temperature)
            name.Append('_').Append(variable.Altitude.ToString(CultureInfo.InvariantCulture)).Append('m');

        if (variable.DepthTo > 0)
            name.Append('_').Append(variable.Depth.ToString(CultureInfo.InvariantCulture)).Append("_to_").Append(variable.DepthTo.ToString(CultureInfo.InvariantCulture)).Append("cm");
        else if (variable.Depth > 0)
            name.Append('_').Append(variable.Depth.ToString(CultureInfo.InvariantCulture)).Append("cm");

        string? aggregation = variable.Aggregation switch
        {
            Aggregation.none => null,
            Aggregation.minimum => "min",
            Aggregation.maximum => "max",
            _ => variable.Aggregation.ToString()
        };
        if (aggregation != null)
            name.Append('_').Append(aggregation);

        if (variable.Probability != Probability.none)
            name.Append('_').Append(variable.Probability.ToString());

        if (variable.EnsembleMember > 0)
            name.Append("_member").Append(variable.EnsembleMember.ToString("D2", CultureInfo.InvariantCulture));

        return name.ToString();
    }

    private static readonly Dictionary<Unit, string> UnitStrings = new()
    {
        [Unit.celsius] = "°C",
        [Unit.fahrenheit] = "°F",
        [Unit.kelvin] = "K",
        [Unit.kilometres_per_hour] = "km/h",
        [Unit.metre_per_second] = "m/s",
        [Unit.miles_per_hour] = "mp/h",
        [Unit.knots] = "kn",
        [Unit.millimetre] = "mm",
        [Unit.inch] = "inch",
        [Unit.centimetre] = "cm",
        [Unit.metre] = "m",
        [Unit.feet] = "ft",
        [Unit.percentage] = "%",
        [Unit.hectopascal] = "hPa",
        [Unit.pascal] = "Pa",
        [Unit.kilopascal] = "kPa",
        [Unit.degree_direction] = "°",
        [Unit.wmo_code] = "wmo code",
        [Unit.megajoule_per_square_metre] = "MJ/m²",
        [Unit.watt_per_square_metre] = "W/m²",
        [Unit.seconds] = "s",
        [Unit.hours] = "h",
        [Unit.fraction] = "fraction",
        [Unit.dimensionless] = "",
        [Unit.dimensionless_integer] = "",
        [Unit.cubic_metre_per_cubic_metre] = "m³/m³",
        [Unit.kilogram_per_square_metre] = "kg/m²",
        [Unit.joule_per_kilogram] = "J/kg",
        [Unit.gram_per_kilogram] = "g/kg",
        [Unit.per_second] = "s⁻¹",
        [Unit.cubic_metre_per_second] = "m³/s",
        [Unit.kilogram_per_cubic_metre] = "kg/m³",
        [Unit.undefined] = "undefined", // the JSON format reports this for variables the selected model does not provide
        [Unit.iso8601] = "iso8601",
        [Unit.unix_time] = "unixtime"
    };

    private static string UnitToString(Unit unit) =>
        UnitStrings.TryGetValue(unit, out var text) ? text : string.Empty;
}
