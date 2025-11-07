using System;
using System.Globalization;

namespace OpenMeteo
{
    public class ElevationUrlBuilder
    {
        private readonly UrlBuilder _urlBuilder;
        private const string DefaultSubdomain = "api";
        private const string ApiPath = "/v1/elevation";

        public ElevationUrlBuilder()
        {
            _urlBuilder = new UrlBuilder()
                .WithSubdomain(DefaultSubdomain)
                .WithPath(ApiPath);
        }

        public ElevationUrlBuilder(Uri customBaseUri)
        {
            _urlBuilder = new UrlBuilder()
                .WithBaseUri(customBaseUri)
                .WithPath(ApiPath);
        }

        public ElevationUrlBuilder(string apiKey)
        {
            _urlBuilder = new UrlBuilder()
                .WithApiKey(apiKey)
                .WithSubdomain($"customer-{DefaultSubdomain}")
                .WithPath(ApiPath);
        }

        public ElevationUrlBuilder WithApiKey(string apiKey)
        {
            _urlBuilder.WithApiKey(apiKey);
            return this;
        }

        public ElevationUrlBuilder WithOptions(ElevationOptions options)
        {
            _urlBuilder.AddParameter(nameof(options.Latitude).ToLower(), options.Latitude.ToString(CultureInfo.InvariantCulture));
            _urlBuilder.AddParameter(nameof(options.Longitude).ToLower(), options.Longitude.ToString(CultureInfo.InvariantCulture));

            return this;
        }

        public string Build()
        {
            return _urlBuilder.Build();
        }
    }
}
