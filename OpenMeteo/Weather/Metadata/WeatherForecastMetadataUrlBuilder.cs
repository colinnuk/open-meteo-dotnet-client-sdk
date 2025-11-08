using OpenMeteo.Url;
using OpenMeteo.Weather.Options;
using System;


namespace OpenMeteo.Weather.Metadata
{
    public class WeatherForecastMetadataUrlBuilder : ApiUrlBuilder
    {
        protected override string DefaultSubdomain => "api";
        protected override string ApiPath => string.Empty;  // Ignored here

        public WeatherForecastMetadataUrlBuilder() : base()
        {
        }

        public WeatherForecastMetadataUrlBuilder(Uri customBaseUri) : base(customBaseUri)
        {
        }

        public WeatherForecastMetadataUrlBuilder(string apiKey) : base(apiKey)
        {
        }

        public WeatherForecastMetadataUrlBuilder(Uri customBaseUri, string apiKey) : base(customBaseUri, apiKey)
        {
        }

        public WeatherForecastMetadataUrlBuilder WithModel(WeatherModelOptionsParameter weatherModel)
        {
            var metaName = MetadataNameHelper.GetMetadataUrlName(weatherModel);
            _urlBuilder.WithPath($"/data/{metaName}/static/meta.json");
            return this;
        }
    }
}
