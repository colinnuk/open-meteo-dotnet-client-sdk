using OpenMeteo.Weather.Utilities;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenMeteo.Weather.Forecast.Options
{
    /// <summary>
    /// Options for requesting forecasts for multiple geographical WGS84 coordinates.
    /// </summary>
    public class MultipleWeatherForecastOptions : IWeatherForecastOptions
    {
        /// <summary>
        /// Geographical WGS84 coordinates of the requested locations.
        /// </summary>
        public List<WeatherForecastCoordinate> Coordinates { get; } = [];

        /// <summary>
        /// Default is "celsius". Use "fahrenheit" to convert temperature to fahrenheit.
        /// </summary>
        public TemperatureUnitType Temperature_Unit { get; set; } = TemperatureUnitType.celsius;

        /// <summary>
        /// Default is "kmh". Other options: "ms", "mph", "kn".
        /// </summary>
        public WindspeedUnitType Windspeed_Unit { get; set; } = WindspeedUnitType.kmh;

        /// <summary>
        /// Default is "mm". Other options: "inch".
        /// </summary>
        public PrecipitationUnitType Precipitation_Unit { get; set; } = PrecipitationUnitType.mm;

        /// <summary>
        /// Default is "land". Other options: "sea", "nearest".
        /// </summary>
        public CellSelectionType Cell_Selection { get; set; } = CellSelectionType.land;

        /// <summary>
        /// Default is "GMT".
        /// </summary>
        public string Timezone { get; set; } = "GMT";

        public HourlyOptions Hourly { get; set; } = [];
        public DailyOptions Daily { get; set; } = [];
        public WeatherModelOptions Models { get; set; } = [];
        public CurrentOptions Current { get; set; } = [];
        public Minutely15Options Minutely_15 { get; set; } = [];

        /// <summary>
        /// Default is "iso8601". Other options: "unixtime".
        /// </summary>
        public TimeformatType Timeformat { get; set; } = TimeformatType.iso8601;

        /// <summary>
        /// Number of days to get the past forecast for. Mutually exclusive with start_date and end_date.
        /// </summary>
        public int? Past_Days { get; set; }

        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly? Start_date { get; set; }

        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly? End_date { get; set; }

        public MultipleWeatherForecastOptions()
        {
        }

        public MultipleWeatherForecastOptions(IEnumerable<WeatherForecastCoordinate> coordinates)
        {
            Coordinates.AddRange(coordinates ?? throw new ArgumentNullException(nameof(coordinates)));
        }
    }
}
