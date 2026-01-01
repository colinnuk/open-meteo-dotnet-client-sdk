using System;
using System.Text.Json.Serialization;
using OpenMeteo.Weather.Forecast.Options;
using OpenMeteo.Weather.Utilities;

namespace OpenMeteo.Weather.Ensemble.Options;

public class WeatherEnsembleOptions
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
    public TemperatureUnitType Temperature_Unit { get; set; }

    /// <summary>
    /// Default is "kmh". Other options: "ms", "mph", "kn"
    /// </summary>
    public WindspeedUnitType Windspeed_Unit { get; set; }

    /// <summary>
    /// Default is "mm". Other options: "inch"
    /// </summary>
    public PrecipitationUnitType Precipitation_Unit { get; set; }

    /// <summary>
    /// Default is "land". Other options: "sea": prefers grid-cells on sea level, "nearest": nearest grid cell
    /// </summary>
    public CellSelectionType Cell_Selection { get; set; }

    /// <summary>
    /// Default is "GMT". Any time zone name from the time zone database is supported. (eg . Europe/Berlin, America/New_York)
    /// </summary>
    public string Timezone { get; set; }

    public WeatherEnsembleHourlyOptions Hourly { get { return _hourly; } set { if (value != null) _hourly = value; } }
    public WeatherEnsembleDailyOptions Daily { get { return _daily; } set { if (value != null) _daily = value; } }
    public EnsembleModelOptions Models { get { return _models; } set { if (value != null) _models = value; } }

    /// <summary>
    /// Default is "iso8601". Other options: "unixtime". 
    /// See https://open-meteo.com/en/docs/ensemble-api for more info
    /// </summary>
    public TimeformatType Timeformat { get; set; }

    /// <summary>
    /// Number of days to get the past forecast for. Mutually exclusive with start_date & end_date.
    /// </summary>
    public int? Past_Days { get; set; }

    /// <summary>
    /// Per default, only 7 days are returned. Up to 35 days of forecast are possible.
    /// </summary>
    public int? Forecast_Days { get; set; }

    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly? Start_date { get; set; }
    
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly? End_date { get; set; }

    private WeatherEnsembleHourlyOptions _hourly = [];
    private WeatherEnsembleDailyOptions _daily = [];
    private EnsembleModelOptions _models = [];

    public WeatherEnsembleOptions(float latitude, float longitude, TemperatureUnitType temperature_Unit, WindspeedUnitType windspeed_Unit, PrecipitationUnitType precipitation_Unit, string timezone, WeatherEnsembleHourlyOptions hourly, WeatherEnsembleDailyOptions daily, TimeformatType timeformat, int? past_Days, int? forecast_Days, DateOnly? start_date, DateOnly? end_date, EnsembleModelOptions models, CellSelectionType cell_selection)
    {
        Latitude = latitude;
        Longitude = longitude;
        Temperature_Unit = temperature_Unit;
        Windspeed_Unit = windspeed_Unit;
        Precipitation_Unit = precipitation_Unit;
        Timezone = timezone;

        if (hourly != null)
            Hourly = hourly;
        if (daily != null)
            Daily = daily;
        if (models != null)
            Models = models;

        Timeformat = timeformat;
        Past_Days = past_Days;
        Forecast_Days = forecast_Days;
        Start_date = start_date;
        End_date = end_date;
        Cell_Selection = cell_selection;
    }

    public WeatherEnsembleOptions(float latitude, float longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        Temperature_Unit = TemperatureUnitType.celsius;
        Windspeed_Unit = WindspeedUnitType.kmh;
        Precipitation_Unit = PrecipitationUnitType.mm;
        Timeformat = TimeformatType.iso8601;
        Cell_Selection = CellSelectionType.land;
        Timezone = "GMT";
        
        Start_date = null;
        End_date = null;
    }

    public WeatherEnsembleOptions()
    {
        Latitude = 0f;
        Longitude = 0f;
        Temperature_Unit = TemperatureUnitType.celsius;
        Windspeed_Unit = WindspeedUnitType.kmh;
        Precipitation_Unit = PrecipitationUnitType.mm;
        Timeformat = TimeformatType.iso8601;
        Cell_Selection = CellSelectionType.land;
        Timezone = "GMT";
        
        Start_date = null;
        End_date = null;
    }
}
