using System;
using System.Globalization;
using System.Linq;
using OpenMeteo.Url;
using OpenMeteo.Weather.Seasonal.Options;
using OpenMeteo.Weather.Utilities;

namespace OpenMeteo.Weather.Seasonal
{
    public class SeasonalUrlBuilder : ApiUrlBuilder
    {
        protected override string DefaultSubdomain => "seasonal-api";
        protected override string ApiPath => "/v1/seasonal";

        public SeasonalUrlBuilder() : base()
        {
        }

        public SeasonalUrlBuilder(Uri customBaseUri) : base(customBaseUri)
        {
        }

        public SeasonalUrlBuilder(string apiKey) : base(apiKey)
        {
        }

        public SeasonalUrlBuilder(Uri customBaseUri, string apiKey) : base(customBaseUri, apiKey)
        {
        }

        public SeasonalUrlBuilder WithOptions(SeasonalOptions options)
        {
            if (options.Hourly.Count + options.Daily.Count + options.Weekly.Count + options.Monthly.Count == 0)
                throw new ArgumentException("At least one hourly, daily, weekly or monthly variable must be specified.", nameof(options));

            _urlBuilder.AddParameter(nameof(options.Latitude), options.Latitude.ToString(CultureInfo.InvariantCulture));
            _urlBuilder.AddParameter(nameof(options.Longitude), options.Longitude.ToString(CultureInfo.InvariantCulture));
            _urlBuilder.AddParameter(nameof(options.Temperature_Unit), options.Temperature_Unit.ToString());
            _urlBuilder.AddParameter(nameof(options.Windspeed_Unit), options.Windspeed_Unit.ToString());
            _urlBuilder.AddParameter(nameof(options.Precipitation_Unit), options.Precipitation_Unit.ToString());
            _urlBuilder.AddParameter(nameof(options.Timezone), options.Timezone);
            _urlBuilder.AddParameter(nameof(options.Timeformat), options.Timeformat.ToString());
            _urlBuilder.AddParameter(nameof(options.Past_Days), options.Past_Days?.ToString(CultureInfo.InvariantCulture));
            _urlBuilder.AddParameter(nameof(options.Forecast_Days), options.Forecast_Days?.ToString(CultureInfo.InvariantCulture));
            _urlBuilder.AddParameter(nameof(options.Start_date), options.Start_date?.ToString(DateOnlyConverter.Format));
            _urlBuilder.AddParameter(nameof(options.End_date), options.End_date?.ToString(DateOnlyConverter.Format));
            _urlBuilder.AddParameter(nameof(options.Cell_Selection), options.Cell_Selection.ToString());

            _urlBuilder.AddCollection(nameof(options.Hourly), options.Hourly.Select(x => x.ToString()));
            _urlBuilder.AddCollection(nameof(options.Daily), options.Daily.Select(x => x.ToString()));
            _urlBuilder.AddCollection(nameof(options.Weekly), options.Weekly.Select(x => x.ToString()));
            _urlBuilder.AddCollection(nameof(options.Monthly), options.Monthly.Select(x => x.ToString()));
            _urlBuilder.AddCollection(nameof(options.Models), options.Models.Select(x => x.ToString()));

            return this;
        }

        public SeasonalUrlBuilder WithFlatbuffers(bool useFlatbuffers)
        {
            _urlBuilder.WithFlatbuffers(useFlatbuffers);
            return this;
        }
    }
}
