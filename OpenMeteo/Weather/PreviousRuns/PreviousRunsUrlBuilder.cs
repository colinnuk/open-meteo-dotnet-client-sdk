using OpenMeteo.Url;
using OpenMeteo.Weather.PreviousRuns.Options;
using OpenMeteo.Weather.Utilities;
using System;
using System.Globalization;
using System.Linq;

namespace OpenMeteo.Weather.PreviousRuns
{
    public class PreviousRunsUrlBuilder : ApiUrlBuilder
    {
        protected override string DefaultSubdomain => "previous-runs-api";
        protected override string ApiPath => "/v1/forecast";

        public PreviousRunsUrlBuilder() : base()
        {
        }

        public PreviousRunsUrlBuilder(Uri customBaseUri) : base(customBaseUri)
        {
        }

        public PreviousRunsUrlBuilder(string apiKey) : base(apiKey)
        {
        }

        public PreviousRunsUrlBuilder(Uri customBaseUri, string apiKey) : base(customBaseUri, apiKey)
        {
        }

        public PreviousRunsUrlBuilder WithOptions(PreviousRunsOptions options)
        {
            if (options.Previous_Days is < 1 or > 7)
                throw new ArgumentOutOfRangeException(nameof(options.Previous_Days), "Previous_Days must be between 1 and 7.");

            _urlBuilder.AddParameter("latitude", options.Latitude.ToString(CultureInfo.InvariantCulture));
            _urlBuilder.AddParameter("longitude", options.Longitude.ToString(CultureInfo.InvariantCulture));
            _urlBuilder.AddParameter("temperature_unit", options.Temperature_Unit.ToString());
            _urlBuilder.AddParameter("windspeed_unit", options.Windspeed_Unit.ToString());
            _urlBuilder.AddParameter("precipitation_unit", options.Precipitation_Unit.ToString());
            _urlBuilder.AddParameter("timezone", options.Timezone);
            _urlBuilder.AddParameter("timeformat", options.Timeformat.ToString());
            _urlBuilder.AddParameter("past_days", options.Past_Days?.ToString(CultureInfo.InvariantCulture));
            _urlBuilder.AddParameter("forecast_days", options.Forecast_Days?.ToString(CultureInfo.InvariantCulture));
            _urlBuilder.AddParameter("start_date", options.Start_date?.ToString(DateOnlyConverter.Format));
            _urlBuilder.AddParameter("end_date", options.End_date?.ToString(DateOnlyConverter.Format));
            _urlBuilder.AddParameter("cell_selection", options.Cell_Selection.ToString());

            if (options.Hourly.Count > 0)
            {
                var parameters = options.Hourly.Parameter.SelectMany(parameter =>
                    Enumerable.Range(1, options.Previous_Days).Select(day => $"{parameter}_previous_day{day}"));
                _urlBuilder.AddCollection("hourly", parameters);
            }

            if (options.Models.Count > 0)
                _urlBuilder.AddCollection("models", options.Models.Parameter.Select(model => model.ToString()));

            return this;
        }
    }
}
