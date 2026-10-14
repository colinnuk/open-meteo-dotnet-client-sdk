using OpenMeteo.Weather.Forecast.Options;
using OpenMeteo.Weather.Utilities;
using System;
using System.Text.Json.Serialization;

namespace OpenMeteo.Weather.PreviousRuns.Options
{
    /// <summary>
    /// Options for retrieving forecasts from previous model runs.
    /// </summary>
    public class PreviousRunsOptions
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public TemperatureUnitType Temperature_Unit { get; set; } = TemperatureUnitType.celsius;
        public WindspeedUnitType Windspeed_Unit { get; set; } = WindspeedUnitType.kmh;
        public PrecipitationUnitType Precipitation_Unit { get; set; } = PrecipitationUnitType.mm;
        public CellSelectionType Cell_Selection { get; set; } = CellSelectionType.land;
        public string Timezone { get; set; } = "GMT";
        public TimeformatType Timeformat { get; set; } = TimeformatType.iso8601;
        public int? Past_Days { get; set; }
        public int? Forecast_Days { get; set; }

        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly? Start_date { get; set; }

        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly? End_date { get; set; }

        /// <summary>
        /// The number of previous model runs to retrieve, from one to seven days before valid time.
        /// </summary>
        public int Previous_Days { get; set; } = 7;

        public HourlyOptions Hourly { get; set; } = [];
        public WeatherModelOptions Models { get; set; } = [];
    }
}
