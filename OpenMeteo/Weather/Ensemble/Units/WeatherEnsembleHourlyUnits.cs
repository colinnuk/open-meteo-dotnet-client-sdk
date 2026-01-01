using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenMeteo.Weather.Ensemble.Units;

/// <summary>
/// Units for hourly ensemble weather variables
/// </summary>
public class WeatherEnsembleHourlyUnits
{
    public string? Time { get; set; }

    /// <summary>
    /// Additional unit data for ensemble members stored as a dictionary for dynamic member access
    /// Property names follow the pattern: {variable}_member{number}
    /// Example: "temperature_2m_member01", "precipitation_member05"
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalData { get; set; }
}
