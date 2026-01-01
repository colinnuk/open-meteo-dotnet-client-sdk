using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenMeteo.Weather.Ensemble.ResponseModel;

/// <summary>
/// Internal hourly ensemble weather data used for JSON deserialization
/// </summary>
public class WeatherEnsembleHourlyDto
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
