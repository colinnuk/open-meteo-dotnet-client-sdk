using System;
using System.Globalization;

namespace OpenMeteo
{
    public class ElevationUrlBuilder : ApiUrlBuilder
    {
        protected override string DefaultSubdomain => "api";
        protected override string ApiPath => "/v1/elevation";

        public ElevationUrlBuilder() : base()
        {
        }

        public ElevationUrlBuilder(Uri customBaseUri) : base(customBaseUri)
        {
        }

        public ElevationUrlBuilder(string apiKey) : base(apiKey)
        {
        }

        public ElevationUrlBuilder(Uri customBaseUri, string apiKey) : base(customBaseUri, apiKey)
        {
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
    }
}
