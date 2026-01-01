using OpenMeteo.Weather.Ensemble.Units;

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
    public float GenerationTime { get; set; }

    /// <summary>
    /// Applied timezone offset from the timezone parameter.
    /// </summary>
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
    public string? TimezoneAbbreviation { get; set; }

    /// <summary>
    /// For each selected hourly ensemble weather variable, the unit will be listed here
    /// </summary>
    public WeatherEnsembleHourlyUnits? HourlyUnits { get; set; }

    /// <summary>
    /// For each selected hourly ensemble weather variable, data will be returned as arrays. 
    /// Additionally a time array will be returned with ISO8601 timestamps.
    /// </summary>
    public EnsembleHourly? Hourly { get; set; }

    /// <summary>
    /// For each selected daily ensemble weather variable, the unit will be listed here
    /// </summary>
    public WeatherEnsembleDailyUnits? DailyUnits { get; set; }

    /// <summary>
    /// For each selected daily ensemble weather variable, data will be returned as arrays. 
    /// Additionally a time array will be returned with date values.
    /// </summary>
    public EnsembleDaily? Daily { get; set; }
}
