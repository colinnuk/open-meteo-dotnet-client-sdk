using OpenMeteo.Url;
using System;
using System.Globalization;
using System.Linq;

namespace OpenMeteo.AirQuality
{
    public class AirQualityUrlBuilder : ApiUrlBuilder
    {
        protected override string DefaultSubdomain => "air-quality-api";
        protected override string ApiPath => "/v1/air-quality";

        public AirQualityUrlBuilder() : base()
        {
        }

        public AirQualityUrlBuilder(Uri customBaseUri) : base(customBaseUri)
        {
        }

        public AirQualityUrlBuilder(string apiKey) : base(apiKey)
        {
        }

        public AirQualityUrlBuilder(Uri customBaseUri, string apiKey) : base(customBaseUri, apiKey)
        {
        }

        public AirQualityUrlBuilder WithApiKey(string apiKey)
        {
            _urlBuilder.WithApiKey(apiKey);
            return this;
        }

        public AirQualityUrlBuilder WithOptions(AirQualityOptions options)
        {
            _urlBuilder.AddParameter(nameof(options.Latitude).ToLower(), options.Latitude.ToString(CultureInfo.InvariantCulture));
            _urlBuilder.AddParameter(nameof(options.Longitude).ToLower(), options.Longitude.ToString(CultureInfo.InvariantCulture));
            _urlBuilder.AddParameter(nameof(options.Domains).ToLower(), options.Domains);
            _urlBuilder.AddParameter(nameof(options.Timeformat).ToLower(), options.Timeformat);
            _urlBuilder.AddParameter(nameof(options.Timezone).ToLower(), options.Timezone);

            if (options.Hourly.Count > 0)
                _urlBuilder.AddCollection(nameof(options.Hourly).ToLower(), options.Hourly.Parameter.Select(x => x.ToString()));

            return this;
        }
    }
}
