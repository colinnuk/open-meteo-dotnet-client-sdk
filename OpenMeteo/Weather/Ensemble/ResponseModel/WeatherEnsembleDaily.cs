using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenMeteo.Weather.Ensemble.ResponseModel;

/// <summary>
/// Daily ensemble weather data with individual member forecasts
/// </summary>
public class WeatherEnsembleDaily
{
    /// <summary>
    /// Array of dates for daily data
    /// </summary>
    public DateOnly[]? Time { get; set; }

    /// <summary>
    /// Additional ensemble member data stored as a dictionary for dynamic member access
    /// Property names follow the pattern: {variable}_member{number}
    /// Example: "temperature_2m_max_member01", "precipitation_sum_member05"
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalData { get; set; }
}
