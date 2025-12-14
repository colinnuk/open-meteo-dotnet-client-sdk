using OpenMeteo.Weather.Utilities;
using System;
using System.Text.Json.Serialization;

namespace OpenMeteo.Weather.Forecast.Options
{
    public class WeatherForecastOptions
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

        public HourlyOptions Hourly { get { return _hourly; } set { if (value != null) _hourly = value; } }
        public DailyOptions Daily { get { return _daily; } set { if (value != null) _daily = value; } }
        public WeatherModelOptions Models { get { return _models; } set { if (value != null) _models = value; } }

        /// <summary>
        /// Default is an empty string array.
        /// Include current weather conditions in API response.
        /// </summary>
        public CurrentOptions Current { get { return _current; } set { if (value != null) _current = value; } }
        public Minutely15Options Minutely_15 { get { return _minutely15; } set { if (value != null) _minutely15 = value; } }

        /// <summary>
        /// Default is "iso8601". Other options: "unixtime". 
        /// See https://open-meteo.com/en/docs for more info
        /// </summary>
        public TimeformatType Timeformat { get; set; }

        /// <summary>
        /// Number of days to get the past forecast for. Mutually exclusive with start_date & end_date.
        /// </summary>
        /// <value></value>
        public int? Past_Days { get; set; }

        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly? Start_date { get; set; }
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly? End_date { get; set; }

        private HourlyOptions _hourly = [];
        private DailyOptions _daily = [];
        private WeatherModelOptions _models = [];
        private CurrentOptions _current = [];
        private Minutely15Options _minutely15 = [];

        public WeatherForecastOptions(float latitude, float longitude, TemperatureUnitType temperature_Unit, WindspeedUnitType windspeed_Unit, PrecipitationUnitType precipitation_Unit, string timezone, HourlyOptions hourly, DailyOptions daily, CurrentOptions current, Minutely15Options minutely15, TimeformatType timeformat, int past_Days, DateOnly? start_date, DateOnly? end_date, WeatherModelOptions models, CellSelectionType cell_selection)
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
            if (current != null)
                Current = current;
            if (minutely15 != null)
                Minutely_15 = minutely15;

            Timeformat = timeformat;
            Past_Days = past_Days;
            Start_date = start_date;
            End_date = end_date;
            Cell_Selection = cell_selection;
        }
        public WeatherForecastOptions(float latitude, float longitude)
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
        public WeatherForecastOptions()
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

    public enum TemperatureUnitType
    {
        celsius,
        fahrenheit
    }

    public enum WindspeedUnitType
    {
        kmh,
        ms,
        mph,
        kn
    }

    public enum PrecipitationUnitType
    {
        mm,
        inch
    }

    public enum TimeformatType
    {
        iso8601,
        unixtime
    }

    public enum CellSelectionType
    {
        land,
        sea,
        nearest
    }
}
