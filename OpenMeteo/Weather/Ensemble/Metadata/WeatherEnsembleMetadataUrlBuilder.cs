using OpenMeteo.Url;
using OpenMeteo.Weather.Ensemble.Options;
using System;


namespace OpenMeteo.Weather.Ensemble.Metadata
{
    public class WeatherEnsembleMetadataUrlBuilder : ApiUrlBuilder
    {
        protected override string DefaultSubdomain => "ensemble-api";
        protected override string ApiPath => string.Empty;  // Ignored here

        public WeatherEnsembleMetadataUrlBuilder() : base()
        {
        }

        public WeatherEnsembleMetadataUrlBuilder(Uri customBaseUri) : base(customBaseUri)
        {
        }

        public WeatherEnsembleMetadataUrlBuilder(string apiKey) : base(apiKey)
        {
        }

        public WeatherEnsembleMetadataUrlBuilder(Uri customBaseUri, string apiKey) : base(customBaseUri, apiKey)
        {
        }

        public WeatherEnsembleMetadataUrlBuilder WithModel(EnsembleModelOptionsParameter weatherModel)
        {
            var metaName = MetadataNameHelper.GetMetadataUrlName(weatherModel);
            _urlBuilder.WithPath($"/data/{metaName}/static/meta.json");
            return this;
        }
    }
}
