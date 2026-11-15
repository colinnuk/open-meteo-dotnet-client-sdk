using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMeteo.Weather.Seasonal.ResponseModel;

/// <summary>
/// Response of the Seasonal Forecast API (https://open-meteo.com/en/docs/seasonal-forecast-api)
/// </summary>
public class WeatherSeasonal
{
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public float Elevation { get; set; }
    public float GenerationTime { get; set; }
    public int UtcOffset { get; set; }
    public string? Timezone { get; set; }
    public string? TimezoneAbbreviation { get; set; }

    /// <summary>
    /// 6-hourly data (requested through <see cref="Options.SeasonalOptions.Hourly"/>)
    /// </summary>
    public SeasonalSeries? Hourly { get; set; }
    public SeasonalSeries? Daily { get; set; }
    /// <summary>
    /// Weekly data. Only available from JSON responses.
    /// </summary>
    public SeasonalSeries? Weekly { get; set; }
    public SeasonalSeries? Monthly { get; set; }
}

/// <summary>
/// A time series with one value array per variable. Seasonal models return ensemble members as extra
/// variables named {variable}_member{nn}, e.g. "temperature_2m_max_member01".
/// </summary>
public class SeasonalSeries
{
    private const string MemberInfix = "_member";

    /// <summary>
    /// Timestamps of the series. For weekly and monthly series this is the first day of the period.
    /// </summary>
    public DateTime[] Time { get; set; } = [];

    /// <summary>
    /// All variables of the series by their API name, including ensemble members.
    /// Missing values are null.
    /// </summary>
    public Dictionary<string, float?[]> Variables { get; set; } = [];

    /// <summary>
    /// Unit of every variable by its API name
    /// </summary>
    public Dictionary<string, string> Units { get; set; } = [];

    /// <summary>
    /// Gets the values of a variable (control run or ensemble mean), or null if it was not returned.
    /// </summary>
    /// <param name="variable">API name of the variable, e.g. "temperature_2m_max". The enum values' ToString() can be used.</param>
    public float?[]? Get(string variable) => Variables.TryGetValue(variable, out var values) ? values : null;

    /// <summary>
    /// Gets the values of every ensemble member of a variable, ordered by member number.
    /// Empty if the request used an ensemble mean model or the variable was not returned.
    /// </summary>
    public IReadOnlyList<float?[]> GetMembers(string variable)
    {
        string prefix = variable + MemberInfix;
        return Variables
            .Where(kvp => kvp.Key.StartsWith(prefix, StringComparison.Ordinal))
            .OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
            .Select(kvp => kvp.Value)
            .ToList();
    }
}
