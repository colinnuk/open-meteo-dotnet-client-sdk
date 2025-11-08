using System;


namespace OpenMeteo
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

        public WeatherForecastMetadataUrlBuilder WithModel(WeatherModelOptionsParameter weatherModel)
        {
            var metaName = MetadataNameHelper.GetMetadataUrlName(weatherModel);
            _urlBuilder.WithPath($"/data/{metaName}/static/meta.json");
            return this;
        }
    }
}
