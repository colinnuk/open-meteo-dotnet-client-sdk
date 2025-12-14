using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenMeteo.Weather.Ensemble.ResponseModel;

/// <summary>
/// Hourly ensemble weather data with individual member forecasts
/// </summary>
public class WeatherEnsembleHourly
{
    /// <summary>
    /// Array of timestamps for hourly data
    /// </summary>
    public string[]? Time { get; set; }

    /// <summary>
    /// Additional ensemble member data stored as a dictionary for dynamic member access
    /// Property names follow the pattern: {variable}_member{number}
    /// Example: "temperature_2m_member01", "precipitation_member05"
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalData { get; set; }
}
