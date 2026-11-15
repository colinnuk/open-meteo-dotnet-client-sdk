using System;
using System.Text.Json.Serialization;
using OpenMeteo.Weather.Forecast.Options;
using OpenMeteo.Weather.Utilities;

namespace OpenMeteo.Weather.Seasonal.Options;

/// <summary>
/// Options for the Seasonal Forecast API (https://open-meteo.com/en/docs/seasonal-forecast-api)
/// </summary>
public class SeasonalOptions
{
    /// <summary>
    /// Geographical WGS84 coordinate of the location
    /// </summary>
    public float Latitude { get; set; }

    /// <summary>
    /// Geographical WGS84 coordinate of the location
    /// </summary>
    public float Longitude { get; set; }

    /// <summary>
    /// Default is "celsius". Use "fahrenheit" to convert temperature to fahrenheit
    /// </summary>
    public TemperatureUnitType Temperature_Unit { get; set; } = TemperatureUnitType.celsius;

    /// <summary>
    /// Default is "kmh". Other options: "ms", "mph", "kn"
    /// </summary>
    public WindspeedUnitType Windspeed_Unit { get; set; } = WindspeedUnitType.kmh;

    /// <summary>
    /// Default is "mm". Other options: "inch"
    /// </summary>
    public PrecipitationUnitType Precipitation_Unit { get; set; } = PrecipitationUnitType.mm;

    /// <summary>
    /// Default is "land". Other options: "sea": prefers grid-cells on sea level, "nearest": nearest grid cell
    /// </summary>
    public CellSelectionType Cell_Selection { get; set; } = CellSelectionType.land;

    /// <summary>
    /// Default is "GMT". Any time zone name from the time zone database is supported. (eg. Europe/Berlin, America/New_York)
    /// </summary>
    public string Timezone { get; set; } = "GMT";

    /// <summary>
    /// Default is "iso8601". Other options: "unixtime".
    /// </summary>
    public TimeformatType Timeformat { get; set; } = TimeformatType.iso8601;

    /// <summary>
    /// 6-hourly variables (requested from the API through the "hourly" parameter)
    /// </summary>
    public SeasonalVariableOptions<SeasonalHourlyOptionsParameter> Hourly
    {
        get => _hourly;
        set { if (value != null) _hourly = value; }
    }

    public SeasonalVariableOptions<SeasonalDailyOptionsParameter> Daily
    {
        get => _daily;
        set { if (value != null) _daily = value; }
    }

    public SeasonalVariableOptions<SeasonalWeeklyOptionsParameter> Weekly
    {
        get => _weekly;
        set { if (value != null) _weekly = value; }
    }

    public SeasonalVariableOptions<SeasonalMonthlyOptionsParameter> Monthly
    {
        get => _monthly;
        set { if (value != null) _monthly = value; }
    }

    /// <summary>
    /// Models to query. If empty, the API default (ecmwf_seasonal_seamless) is used.
    /// </summary>
    public SeasonalVariableOptions<SeasonalModelOptionsParameter> Models
    {
        get => _models;
        set { if (value != null) _models = value; }
    }

    /// <summary>
    /// Number of past days to include. Mutually exclusive with start_date and end_date.
    /// </summary>
    public int? Past_Days { get; set; }

    /// <summary>
    /// Number of forecast days (0 to 217 allowed by the API). Mutually exclusive with start_date and end_date.
    /// </summary>
    public int? Forecast_Days { get; set; }

    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly? Start_date { get; set; }

    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly? End_date { get; set; }

    private SeasonalVariableOptions<SeasonalHourlyOptionsParameter> _hourly = [];
    private SeasonalVariableOptions<SeasonalDailyOptionsParameter> _daily = [];
    private SeasonalVariableOptions<SeasonalWeeklyOptionsParameter> _weekly = [];
    private SeasonalVariableOptions<SeasonalMonthlyOptionsParameter> _monthly = [];
    private SeasonalVariableOptions<SeasonalModelOptionsParameter> _models = [];

    public SeasonalOptions()
    {
    }

    public SeasonalOptions(float latitude, float longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}
