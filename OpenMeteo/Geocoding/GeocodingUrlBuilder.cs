using OpenMeteo.Url;
using System;

namespace OpenMeteo.Geocoding
{
    public class GeocodingUrlBuilder : ApiUrlBuilder
    {
        protected override string DefaultSubdomain => "geocoding-api";
        protected override string ApiPath => "/v1/search";

        public GeocodingUrlBuilder() : base()
        {
        }

        public GeocodingUrlBuilder(Uri customBaseUri) : base(customBaseUri)
        {
        }

        public GeocodingUrlBuilder(string apiKey) : base(apiKey)
        {
        }

        public GeocodingUrlBuilder(Uri customBaseUri, string apiKey) : base(customBaseUri, apiKey)
        {
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
    }
}
