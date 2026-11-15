using OpenMeteo.Weather.Forecast.Options;
using System;

namespace OpenMeteo.Weather.SingleRuns.Options
{
    /// <summary>
    /// Options for retrieving the forecast produced by one specific model initialization run.
    /// </summary>
    public class SingleRunsOptions : WeatherForecastOptions
    {
        /// <summary>
        /// UTC initialization time of the model run. Seconds and fractional seconds are not supported by the API.
        /// </summary>
        public DateTime Run { get; set; }

        /// <summary>
        /// Number of forecast days to return from the run. When <see langword="null"/>, the API default (7 days) is used.
        /// </summary>
        public int? Forecast_Days { get; set; }
    }
}