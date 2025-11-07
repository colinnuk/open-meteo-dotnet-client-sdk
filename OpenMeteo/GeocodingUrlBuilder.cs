using System;

namespace OpenMeteo
{
    public class GeocodingUrlBuilder
    {
        private readonly UrlBuilder _urlBuilder;
        private const string DefaultSubdomain = "geocoding-api";
        private const string ApiPath = "/v1/search";

        public GeocodingUrlBuilder()
        {
            _urlBuilder = new UrlBuilder()
                .WithSubdomain(DefaultSubdomain)
                .WithPath(ApiPath);
        }

        public GeocodingUrlBuilder(Uri customBaseUri)
        {
            _urlBuilder = new UrlBuilder()
                 .WithBaseUri(customBaseUri)
                 .WithPath(ApiPath);
        }

        public GeocodingUrlBuilder(string apiKey)
        {
            _urlBuilder = new UrlBuilder()
                .WithApiKey(apiKey)
                .WithSubdomain($"customer-{DefaultSubdomain}")
                .WithPath(ApiPath);
        }

        public GeocodingUrlBuilder WithApiKey(string apiKey)
        {
            _urlBuilder.WithApiKey(apiKey);
            return this;
        }

        public GeocodingUrlBuilder WithOptions(GeocodingOptions options)
        {
            _urlBuilder.AddParameter(nameof(options.Name).ToLower(), options.Name);
            _urlBuilder.AddParameter(nameof(options.Count).ToLower(), options.Count > 0 ? options.Count.ToString() : "1");
            _urlBuilder.AddParameter(nameof(options.Format).ToLower(), options.Format);
            _urlBuilder.AddParameter(nameof(options.Language).ToLower(), options.Language);

            return this;
        }

        public string Build()
        {
            return _urlBuilder.Build();
        }
    }
}
