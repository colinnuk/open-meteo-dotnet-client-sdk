using OpenMeteo.Url;
using OpenMeteo.Weather.Seasonal.Options;
using System;

namespace OpenMeteo.Weather.Seasonal.Metadata
{
    public class SeasonalMetadataUrlBuilder : ApiUrlBuilder
    {
        protected override string DefaultSubdomain => "seasonal-api";
        protected override string ApiPath => string.Empty;  // Ignored here

        public SeasonalMetadataUrlBuilder() : base()
        {
        }

        public SeasonalMetadataUrlBuilder(Uri customBaseUri) : base(customBaseUri)
        {
        }

        public SeasonalMetadataUrlBuilder(string apiKey) : base(apiKey)
        {
        }

        public SeasonalMetadataUrlBuilder(Uri customBaseUri, string apiKey) : base(customBaseUri, apiKey)
        {
        }

        public SeasonalMetadataUrlBuilder WithModel(SeasonalModelOptionsParameter weatherModel)
        {
            var metaName = MetadataNameHelper.GetMetadataUrlName(weatherModel);
            _urlBuilder.WithPath($"/data/{metaName}/static/meta.json");
            return this;
        }
    }
}
