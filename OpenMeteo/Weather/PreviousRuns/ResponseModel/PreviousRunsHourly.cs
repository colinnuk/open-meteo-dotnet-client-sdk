using OpenMeteo.Weather.Forecast.ResponseModel;
using OpenMeteo.Weather.Forecast.Units;
using System.Collections.Generic;

namespace OpenMeteo.Weather.PreviousRuns.ResponseModel
{
    /// <summary>
    /// Hourly forecasts grouped by the number of days before valid time that each model run was produced.
    /// </summary>
    public class PreviousRunsHourly
    {
        public IReadOnlyDictionary<int, Hourly> Runs { get; internal set; } = new Dictionary<int, Hourly>();
        public IReadOnlyDictionary<int, HourlyUnits> Units { get; internal set; } = new Dictionary<int, HourlyUnits>();
    }
}
