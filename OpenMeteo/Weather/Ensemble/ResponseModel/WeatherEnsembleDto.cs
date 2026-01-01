using OpenMeteo.Weather.Ensemble.Units;
using System.Text.Json.Serialization;

namespace OpenMeteo.Weather.Ensemble.ResponseModel;

/// <summary>
/// Internal DTO for Ensemble Weather API response deserialization
/// </summary>
public class WeatherEnsembleDto
{
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public float Elevation { get; set; }

    [JsonPropertyName("generationtime_ms")]
    public float GenerationTime { get; set; }

    [JsonPropertyName("utc_offset_seconds")]
    public int UtcOffset { get; set; }

    public string? Timezone { get; set; }

    [JsonPropertyName("timezone_abbreviation")]
    public string? TimezoneAbbreviation { get; set; }

    [JsonPropertyName("hourly_units")]
    public WeatherEnsembleHourlyUnits? HourlyUnits { get; set; }

    [JsonPropertyName("hourly")]
    public WeatherEnsembleHourlyDto? Hourly { get; set; }

    [JsonPropertyName("daily_units")]
    public WeatherEnsembleDailyUnits? DailyUnits { get; set; }

    [JsonPropertyName("daily")]
    public WeatherEnsembleDailyDto? Daily { get; set; }
}
