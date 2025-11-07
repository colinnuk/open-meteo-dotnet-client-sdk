using System;
using System.Globalization;
using System.Linq;

namespace OpenMeteo
{
    public class AirQualityUrlBuilder
    {
        private readonly UrlBuilder _urlBuilder;
        private const string DefaultSubdomain = "air-quality-api";
        private const string ApiPath = "/v1/air-quality";

        public AirQualityUrlBuilder()
        {
            _urlBuilder = new UrlBuilder()
                .WithSubdomain(DefaultSubdomain)
                .WithPath(ApiPath);
        }

        public AirQualityUrlBuilder(Uri customBaseUri)
        {
            _urlBuilder = new UrlBuilder()
                .WithBaseUri(customBaseUri)
                .WithPath(ApiPath);
        }

        public AirQualityUrlBuilder(string apiKey)
        {
            _urlBuilder = new UrlBuilder()
                .WithApiKey(apiKey)
                .WithSubdomain($"customer-{DefaultSubdomain}")
                .WithPath(ApiPath);
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

        public string Build()
        {
            return _urlBuilder.Build();
        }
    }
}
