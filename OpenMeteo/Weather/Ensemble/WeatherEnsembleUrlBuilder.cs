using OpenMeteo.Url;
using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Utilities;
using System;
using System.Globalization;
using System.Linq;

namespace OpenMeteo.Weather.Ensemble
{
    public class WeatherEnsembleUrlBuilder : ApiUrlBuilder
    {
        protected override string DefaultSubdomain => "ensemble-api";
        protected override string ApiPath => "/v1/ensemble";

        public WeatherEnsembleUrlBuilder() : base()
        {
        }

        public WeatherEnsembleUrlBuilder(Uri customBaseUri) : base(customBaseUri)
        {
        }

        public WeatherEnsembleUrlBuilder(string apiKey) : base(apiKey)
        {
        }

        public WeatherEnsembleUrlBuilder(Uri customBaseUri, string apiKey) : base(customBaseUri, apiKey)
        {
        }

        public WeatherEnsembleUrlBuilder WithOptions(WeatherEnsembleOptions options)
        {
            _urlBuilder.AddParameter(nameof(options.Latitude).ToLower(), options.Latitude.ToString(CultureInfo.InvariantCulture));
            _urlBuilder.AddParameter(nameof(options.Longitude).ToLower(), options.Longitude.ToString(CultureInfo.InvariantCulture));
            _urlBuilder.AddParameter(nameof(options.Temperature_Unit).ToLower(), options.Temperature_Unit.ToString());
            _urlBuilder.AddParameter(nameof(options.Windspeed_Unit).ToLower(), options.Windspeed_Unit.ToString());
            _urlBuilder.AddParameter(nameof(options.Precipitation_Unit).ToLower(), options.Precipitation_Unit.ToString());
            _urlBuilder.AddParameter(nameof(options.Timezone).ToLower(), options.Timezone);
            _urlBuilder.AddParameter(nameof(options.Timeformat).ToLower(), options.Timeformat.ToString());
            _urlBuilder.AddParameter(nameof(options.Past_Days).ToLower(), options.Past_Days?.ToString());
            _urlBuilder.AddParameter(nameof(options.Forecast_Days).ToLower(), options.Forecast_Days?.ToString());
            _urlBuilder.AddParameter(nameof(options.Start_date).ToLower(), options.Start_date?.ToString(DateOnlyConverter.Format));
            _urlBuilder.AddParameter(nameof(options.End_date).ToLower(), options.End_date?.ToString(DateOnlyConverter.Format));
            _urlBuilder.AddParameter(nameof(options.Cell_Selection).ToLower(), options.Cell_Selection.ToString());

            if (options.Hourly.Count > 0)
                _urlBuilder.AddCollection(nameof(options.Hourly).ToLower(), options.Hourly.Parameter.Select(x => x.ToString()));
            if (options.Daily.Count > 0)
                _urlBuilder.AddCollection(nameof(options.Daily).ToLower(), options.Daily.Parameter.Select(x => x.ToString()));
            if (options.Models.Count > 0)
                _urlBuilder.AddCollection(nameof(options.Models).ToLower(), options.Models.Parameter.Select(x => x.ToString()));

            return this;
        }

        public WeatherEnsembleUrlBuilder WithFlatbuffers(bool useFlatbuffers)
        {
            _urlBuilder.WithFlatbuffers(useFlatbuffers);
            return this;
        }
    }
}
