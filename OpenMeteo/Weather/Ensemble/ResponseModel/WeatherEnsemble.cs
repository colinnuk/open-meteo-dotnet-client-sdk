using OpenMeteo.Weather.Ensemble.Units;
using System.Text.Json.Serialization;

namespace OpenMeteo.Weather.Ensemble.ResponseModel;

/// <summary>
/// Ensemble Weather API response
/// </summary>
public class WeatherEnsemble
{
    /// <summary>
    /// WGS84 of the center of the weather grid-cell which was used to generate this forecast. 
    /// This coordinate might be up to 5 km away.
    /// </summary>
    public float Latitude { get; set; }

    /// <summary>
    /// WGS84 of the center of the weather grid-cell which was used to generate this forecast. 
    /// This coordinate might be up to 5 km away.
    /// </summary>
    public float Longitude { get; set; }

    /// <summary>
    /// The elevation in meters of the selected weather grid-cell.
    /// </summary>
    public float Elevation { get; set; }

    /// <summary>
    /// Generation time of the weather forecast in milliseconds.
    /// </summary>
    [JsonPropertyName("generationtime_ms")]
    public float GenerationTime { get; set; }

    /// <summary>
    /// Applied timezone offset from the timezone parameter.
    /// </summary>
    [JsonPropertyName("utc_offset_seconds")]
    public int UtcOffset { get; set; }

    /// <summary>
    /// Timezone identifier
    /// </summary>
    /// <example>Europe/Berlin</example>
    public string? Timezone { get; set; }

    /// <summary>
    /// Timezone abbreviation
    /// </summary>
    /// <example>CEST</example>
    [JsonPropertyName("timezone_abbreviation")]
    public string? TimezoneAbbreviation { get; set; }

    /// <summary>
    /// For each selected hourly ensemble weather variable, the unit will be listed here
    /// </summary>
    [JsonPropertyName("hourly_units")]
    public WeatherEnsembleHourlyUnits? HourlyUnits { get; set; }

    /// <summary>
    /// For each selected hourly ensemble weather variable, data will be returned as arrays. 
    /// Additionally a time array will be returned with ISO8601 timestamps.
    /// Each ensemble member has a separate array (e.g., temperature_2m_member01, temperature_2m_member02, etc.)
    /// </summary>
    [JsonPropertyName("hourly")]
    public WeatherEnsembleHourly? Hourly { get; set; }

    /// <summary>
    /// For each selected daily ensemble weather variable, the unit will be listed here
    /// </summary>
    [JsonPropertyName("daily_units")]
    public WeatherEnsembleDailyUnits? DailyUnits { get; set; }

    /// <summary>
    /// For each selected daily ensemble weather variable, data will be returned as arrays. 
    /// Additionally a time array will be returned with date values.
    /// Each ensemble member has a separate array (e.g., temperature_2m_max_member01, temperature_2m_max_member02, etc.)
    /// </summary>
    [JsonPropertyName("daily")]
    public WeatherEnsembleDaily? Daily { get; set; }
}
